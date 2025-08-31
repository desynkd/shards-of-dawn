using UnityEngine;
using Photon.Pun;

public class CameraFollow : MonoBehaviour
{
	public Vector3 offset = new Vector3(0f, 0f, -10f);
	public float followLerp = 10f;

	private Transform target;

	void LateUpdate()
	{
		if (target == null)
		{
			TryFindTarget();
			if (target == null) return;
		}

		Vector3 desired = target.position + offset;
		transform.position = Vector3.Lerp(transform.position, desired, Mathf.Clamp01(followLerp * Time.deltaTime));
	}

	private void TryFindTarget()
	{
		var players = FindObjectsOfType<Player>();
		for (int i = 0; i < players.Length; i++)
		{
			var view = players[i].pv;
			if (view != null && view.IsMine)
			{
				target = players[i].transform;
				break;
			}
		}
	}
}


