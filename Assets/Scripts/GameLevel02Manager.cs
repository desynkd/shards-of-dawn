using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections;

public class GameLevel02Manager : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 originalCameraPosition;
    private float originalCameraSize;
    private bool isRestarting = false;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
            originalCameraSize = mainCamera.orthographicSize;
        }
    }

    public void RestartScene()
    {
        if (isRestarting) return;
        isRestarting = true;
        
        Debug.Log("[GameLevel02Manager] Player fell off map. Restarting scene...");
        
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // Use PhotonNetwork.LoadLevel to restart the current scene for all clients
                string currentSceneName = SceneManager.GetActiveScene().name;
                Debug.Log($"[GameLevel02Manager] Restarting scene '{currentSceneName}' for all clients via PhotonNetwork");
                PhotonNetwork.LoadLevel(currentSceneName);
            }
            else
            {
                Debug.Log("[GameLevel02Manager] Not master client, requesting restart via RPC");
                // Request restart from master client
                var photonView = GetComponent<PhotonView>();
                if (photonView != null)
                {
                    photonView.RPC(nameof(RPC_RequestRestart), RpcTarget.MasterClient);
                }
            }
        }
        else
        {
            // Single player mode - restart current scene
            Debug.Log("[GameLevel02Manager] Single player mode, restarting scene directly");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    [PunRPC]
    private void RPC_RequestRestart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            Debug.Log($"[GameLevel02Manager] Master client received restart request, restarting scene '{currentSceneName}'");
            PhotonNetwork.LoadLevel(currentSceneName);
        }
    }

    public void RestartSceneWithDelay(float delay = 1.5f)
    {
        StartCoroutine(RestartSceneDelayed(delay));
    }

    private IEnumerator RestartSceneDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        RestartScene();
    }
}
