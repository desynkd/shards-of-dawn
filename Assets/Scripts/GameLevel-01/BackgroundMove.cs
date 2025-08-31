using UnityEngine;
using Photon.Pun;

public class BackgroundMove : MonoBehaviour
{
    [Header("Parallax Settings")]
    public float parallaxMultiplier = 0.1f; // Adjust for desired effect
    public GameObject fallbackPlayer; // Assign in Inspector for dev mode

    private Transform playerTransform;
    private float startX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Find player transform if not set
        if (playerTransform == null)
        {
            if (PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer != null && PhotonNetwork.LocalPlayer.TagObject is GameObject photonPlayer)
            {
                playerTransform = photonPlayer.transform;
            }
            else if (fallbackPlayer != null)
            {
                playerTransform = fallbackPlayer.transform;
            }
        }

        if (playerTransform != null)
        {
            float targetX = startX + playerTransform.position.x * parallaxMultiplier;
            Vector3 newPos = transform.position;
            newPos.x = targetX;
            transform.position = newPos;
        }
    }
}
