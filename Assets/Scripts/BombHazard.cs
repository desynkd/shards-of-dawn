using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class BombHazard : MonoBehaviourPun
{
	public float delaySeconds = 0.3f;
	public bool destroyBombOnTrigger = false;

	private bool triggered;

	void OnTriggerEnter2D(Collider2D other)
	{
		TryHandle(other.gameObject);
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		TryHandle(collision.collider.gameObject);
	}

	void TryHandle(GameObject other)
	{
		if (triggered) return;
		var player = other.GetComponentInParent<Player>();
		if (player == null) player = other.GetComponent<Player>();
		if (player == null) return;
		if (player.pv != null && !player.pv.IsMine) return; // Only trigger for local player
		
		triggered = true;
		// ShowGameOverUI(); // Removed to prevent Game Over text from flashing
		Invoke(nameof(RequestLoad), delaySeconds);
		if (destroyBombOnTrigger) Destroy(gameObject);
	}

	void RequestLoad()
	{
		// Check if we're in GameLevel02 and should restart the scene
		GameLevel02Manager gameLevel02Manager = FindFirstObjectByType<GameLevel02Manager>();
		if (gameLevel02Manager != null)
		{
			// We're in GameLevel02, restart the scene for everyone
			gameLevel02Manager.RestartScene();
			return;
		}

		// Fallback behavior for other levels - restart current scene instead of going to "Loading"
		if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InRoom)
		{
			// Single player - restart current scene
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			return;
		}
		
		// Multiplayer - restart current scene for everyone
		if (PhotonNetwork.IsMasterClient)
		{
			string currentSceneName = SceneManager.GetActiveScene().name;
			PhotonNetwork.LoadLevel(currentSceneName);
		}
		else
		{
			photonView.RPC(nameof(RPC_RequestRestart), RpcTarget.MasterClient);
		}
	}

	[PunRPC]
	void RPC_RequestRestart()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			string currentSceneName = SceneManager.GetActiveScene().name;
			PhotonNetwork.LoadLevel(currentSceneName);
		}
	}

	void ShowGameOverUI()
	{
		var canvasGo = new GameObject("GameOverCanvas");
		var canvas = canvasGo.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvasGo.AddComponent<GraphicRaycaster>();

		var textGo = new GameObject("GameOverText");
		textGo.transform.SetParent(canvasGo.transform, false);
		var text = textGo.AddComponent<Text>();
		text.text = "Game Over";
		text.color = Color.white;
		text.alignment = TextAnchor.MiddleCenter;
		text.fontSize = 64;
		try
		{
			text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		}
		catch
		{
			text.font = Font.CreateDynamicFontFromOSFont("Arial", 64);
		}

		var rt = text.GetComponent<RectTransform>();
		rt.anchorMin = new Vector2(0, 0);
		rt.anchorMax = new Vector2(1, 1);
		rt.offsetMin = Vector2.zero;
		rt.offsetMax = Vector2.zero;
	}
}


