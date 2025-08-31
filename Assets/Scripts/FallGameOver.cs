using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class FallGameOver : MonoBehaviourPun
{
	public float fallY = -10f;
	public float delaySeconds = 1.5f;
    public bool useCameraBottom = true;
    public float cameraMargin = 1f;

	private bool triggered;

	void Update()
	{
		if (triggered) return;
		if (!photonView.IsMine) return;
		float threshold = fallY;
		if (useCameraBottom && Camera.main != null)
		{
			var bottom = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y;
			threshold = bottom - cameraMargin;
		}
		if (transform.position.y > threshold) return;
		triggered = true;
		ShowGameOverUI();
		Invoke(nameof(RequestLoad), delaySeconds);
	}

	void RequestLoad()
	{
		if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InRoom)
		{
			SceneManager.LoadScene("Loading");
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.LoadLevel("Loading");
		}
		else
		{
			photonView.RPC(nameof(RPC_RequestGameOver), RpcTarget.MasterClient);
		}
	}

	[PunRPC]
	void RPC_RequestGameOver()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.LoadLevel("Loading");
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


