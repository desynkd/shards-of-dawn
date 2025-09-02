using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class EnemyPatrol : MonoBehaviourPun
{
	[Header("Movement")]
	public float patrolDistance = 4f;
	public float speed = 2f;
	public float waitAtEndsSeconds = 0f;

	[Header("Kill")]
	public float killDelaySeconds = 0.1f;

	Rigidbody2D rb;
	float leftX;
	float rightX;
	int direction = 1;
	bool waiting;
	bool triggeredKill;

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
		rb.bodyType = RigidbodyType2D.Kinematic;
		rb.gravityScale = 0f;

		leftX = transform.position.x - Mathf.Abs(patrolDistance) * 0.5f;
		rightX = transform.position.x + Mathf.Abs(patrolDistance) * 0.5f;
		direction = 1;
	}

	void Update()
	{
		if (waiting) return;
		float targetX = direction > 0 ? rightX : leftX;
		float step = speed * Time.deltaTime * direction;
		float newX = transform.position.x + step;
		if ((direction > 0 && newX >= targetX) || (direction < 0 && newX <= targetX))
		{
			newX = targetX;
			if (waitAtEndsSeconds > 0f)
			{
				waiting = true;
				Invoke(nameof(EndWait), waitAtEndsSeconds);
			}
			direction *= -1;
		}
		transform.position = new Vector3(newX, transform.position.y, transform.position.z);

		var sr = GetComponent<SpriteRenderer>();
		if (sr != null) sr.flipX = direction < 0 ? true : false;
	}

	void EndWait()
	{
		waiting = false;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		var player = other.GetComponentInParent<Player>();
		if (player == null) player = other.GetComponent<Player>();
		TryKill(player);
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		var player = collision.collider.GetComponentInParent<Player>();
		if (player == null) player = collision.collider.GetComponent<Player>();
		TryKill(player);
	}

	void TryKill(Player player)
	{
		if (player == null) return;
		if (triggeredKill) return;
		if (player.pv != null && !player.pv.IsMine) return; // Only trigger for local player
		
		triggeredKill = true;
		// ShowGameOverUI(); // Removed to prevent Game Over text from flashing
		Invoke(nameof(RequestLoad), killDelaySeconds);
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

	void OnDrawGizmosSelected()
	{
		float half = Mathf.Abs(patrolDistance) * 0.5f;
		Vector3 a = transform.position + Vector3.left * half;
		Vector3 b = transform.position + Vector3.right * half;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(a, b);
	}
}


