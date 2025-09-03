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
		// Shift player position instead of restarting scene
		ShiftPlayerPosition(player);
		if (destroyBombOnTrigger) Destroy(gameObject);
	}

	void ShiftPlayerPosition(Player player)
	{
		// Set player position to x=-19, y=0
		Vector3 newPosition = new Vector3(-19f, 0f, player.transform.position.z);
		player.transform.position = newPosition;
		
		// Reset triggered state to allow future interactions
		triggered = false;
	}

	void RequestLoad()
	{
		if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InRoom)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			return;
		}
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


