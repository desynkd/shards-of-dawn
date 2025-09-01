using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LevelExit : MonoBehaviourPun
{
	[Header("Next Scene")]
	public string nextSceneName = "Level03-1";
	public float delaySeconds = 0.5f;

	bool triggered;

	void OnTriggerEnter2D(Collider2D other)
	{
		TryComplete(other.GetComponent<Player>());
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		TryComplete(collision.collider.GetComponent<Player>());
	}

	void TryComplete(Player player)
	{
		if (triggered) return;
		if (player == null) return;
		if (player.pv != null && !player.pv.IsMine) return;
		triggered = true;
		ShowCompleteUI();
		Invoke(nameof(RequestLoad), delaySeconds);
	}

	void RequestLoad()
	{
		if (string.IsNullOrWhiteSpace(nextSceneName))
		{
			int idx = SceneManager.GetActiveScene().buildIndex + 1;
			if (idx < SceneManager.sceneCountInBuildSettings)
			{
				if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InRoom)
					SceneManager.LoadScene(idx);
				else if (PhotonNetwork.IsMasterClient)
					PhotonNetwork.LoadLevel(idx);
				else
					photonView.RPC(nameof(RPC_LoadIndex), RpcTarget.MasterClient, idx);
				return;
			}
		}

		if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InRoom)
		{
			SceneManager.LoadScene(nextSceneName);
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.LoadLevel(nextSceneName);
		}
		else
		{
			photonView.RPC(nameof(RPC_LoadName), RpcTarget.MasterClient, nextSceneName);
		}
	}

	[PunRPC]
	void RPC_LoadName(string scene)
	{
		if (PhotonNetwork.IsMasterClient)
			PhotonNetwork.LoadLevel(scene);
	}

	[PunRPC]
	void RPC_LoadIndex(int buildIndex)
	{
		if (PhotonNetwork.IsMasterClient)
			PhotonNetwork.LoadLevel(buildIndex);
	}

	void ShowCompleteUI()
	{
		var canvasGo = new GameObject("LevelCompleteCanvas");
		var canvas = canvasGo.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvasGo.AddComponent<GraphicRaycaster>();

		var textGo = new GameObject("CompleteText");
		textGo.transform.SetParent(canvasGo.transform, false);
		var text = textGo.AddComponent<Text>();
		text.text = "Level Complete";
		text.color = Color.white;
		text.alignment = TextAnchor.MiddleCenter;
		text.fontSize = 54;
		try
		{
			text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		}
		catch
		{
			text.font = Font.CreateDynamicFontFromOSFont("Arial", 54);
		}

		var rt = text.GetComponent<RectTransform>();
		rt.anchorMin = new Vector2(0, 0);
		rt.anchorMax = new Vector2(1, 1);
		rt.offsetMin = Vector2.zero;
		rt.offsetMax = Vector2.zero;
	}
}


