using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    void Awake()
    {
        Debug.Log("[ConnectToServer] Awake called. Setting AutomaticallySyncScene to true.");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SerializationRate = 20; // Increase serialization rate for smoother sync
    }

    void Start()
    {
        Debug.Log("[ConnectToServer] Start called. Connecting using settings...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("[ConnectToServer] OnConnectedToMaster called. Joining lobby...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("[ConnectToServer] OnJoinedLobby called. Loading Lobby scene...");
        SceneManager.LoadScene("Lobby");
    }
}
