using UnityEngine;
using Photon.Pun;

public class CameraFollow : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 0, -10); // Default for 2D cameras
    private Transform target;

    void Start()
    {
        StartCoroutine(FindLocalPlayer());
    }

    private System.Collections.IEnumerator FindLocalPlayer()
    {
        while (target == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var p in players)
            {
                PhotonView pv = p.GetComponent<PhotonView>();
                if (pv != null && pv.IsMine) // only follow my own player
                {
                    target = p.transform;
                    break;
                }
            }
            yield return null; // wait for next frame
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
