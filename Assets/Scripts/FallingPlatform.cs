using System.Collections;
using UnityEngine;
using Photon.Pun;

public class FallingPlatform : MonoBehaviourPun
{
	[Header("Trigger")]
	public float delayBeforeFall = 0.25f;
	public bool requireTopContact = true;

	[Header("Physics")]
	public float gravityScaleWhenFalling = 3f;
	public bool oneTime = true;

	Rigidbody2D rb;
	Collider2D col;
	bool triggered;
	float originalGravity;
	RigidbodyType2D originalType;
	Vector3 startPos;
	Quaternion startRot;

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		col = GetComponent<Collider2D>();
		if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
		originalType = rb.bodyType;
		originalGravity = rb.gravityScale;
		startPos = transform.position;
		startRot = transform.rotation;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (triggered) return;
		var player = collision.collider.GetComponent<Player>();
		if (player == null) return;

		if (requireTopContact && col != null)
		{
			var topThreshold = col.bounds.max.y - 0.05f;
			if (player.transform.position.y < topThreshold) return;
		}

		TriggerFall();
	}

	void TriggerFall()
	{
		if (triggered) return;
		triggered = true;

		if (PhotonNetwork.IsConnectedAndReady && photonView != null)
		{
			photonView.RPC(nameof(RPC_BeginFall), RpcTarget.AllBuffered);
		}
		else
		{
			StartCoroutine(BeginFall());
		}
	}

	[PunRPC]
	void RPC_BeginFall()
	{
		StartCoroutine(BeginFall());
	}

	IEnumerator BeginFall()
	{
		yield return new WaitForSeconds(delayBeforeFall);
		rb.bodyType = RigidbodyType2D.Dynamic;
		rb.gravityScale = gravityScaleWhenFalling;
		rb.constraints = RigidbodyConstraints2D.None;
		if (oneTime && col != null)
		{
			// keep collider so player can slide off while it falls
		}
	}
}


