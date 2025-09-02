using UnityEngine;

public class YellowLauncher : MonoBehaviour
{
    public float shiftAmount = 3f; // Amount to shift in X, adjustable in Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // If Photon is present, only allow the local player to trigger
            bool isLocalPlayer = true;
#if PHOTON_UNITY_NETWORKING
            var photonView = other.GetComponent<Photon.Pun.PhotonView>();
            if (Photon.Pun.PhotonNetwork.CurrentRoom != null && photonView != null)
            {
                isLocalPlayer = photonView.IsMine;
            }
#endif
            if (!isLocalPlayer) return;

            // Shift player X position to the right
            Vector3 pos = other.transform.position;
            pos.x += shiftAmount;
            other.transform.position = pos;
        }
    }
}
