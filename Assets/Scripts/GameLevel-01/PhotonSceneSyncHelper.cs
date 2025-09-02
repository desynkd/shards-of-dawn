using UnityEngine;
using Photon.Pun;

/// <summary>
/// Helper script to ensure proper Photon scene synchronization
/// This should be added to a GameObject in the scene to verify settings
/// </summary>
public class PhotonSceneSyncHelper : MonoBehaviourPun
{
    [Header("Scene Sync Settings")]
    public bool enableAutoSyncScene = true;
    public bool logSyncStatus = true;

    void Start()
    {
        if (logSyncStatus)
        {
            Debug.Log($"[PhotonSceneSyncHelper] PhotonNetwork.AutomaticallySyncScene: {PhotonNetwork.AutomaticallySyncScene}");
            Debug.Log($"[PhotonSceneSyncHelper] IsMasterClient: {PhotonNetwork.IsMasterClient}");
            Debug.Log($"[PhotonSceneSyncHelper] InRoom: {PhotonNetwork.InRoom}");
            Debug.Log($"[PhotonSceneSyncHelper] PlayerCount: {PhotonNetwork.CurrentRoom?.PlayerCount ?? 0}");
        }

        // Ensure AutomaticallySyncScene is enabled
        if (enableAutoSyncScene && !PhotonNetwork.AutomaticallySyncScene)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            Debug.Log("[PhotonSceneSyncHelper] Enabled AutomaticallySyncScene");
        }
    }

    void Update()
    {
        // Monitor scene sync status
        if (logSyncStatus && Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log($"[PhotonSceneSyncHelper] Current Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
            Debug.Log($"[PhotonSceneSyncHelper] AutomaticallySyncScene: {PhotonNetwork.AutomaticallySyncScene}");
            Debug.Log($"[PhotonSceneSyncHelper] IsMasterClient: {PhotonNetwork.IsMasterClient}");
        }
    }

    // Callback when scene is loaded
    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Debug.Log($"[PhotonSceneSyncHelper] Scene loaded: {scene.name}");
    }
}
