using UnityEngine;
using TMPro;    
using Photon.Pun;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;

    public void CreateRoom()
    {
        Debug.Log("[CreateAndJoinRooms] CreateRoom called. Input: " + createInput.text);
        if (createInput.text.Length >= 1)
        {
            Debug.Log("[CreateAndJoinRooms] Creating room: " + createInput.text);
            PhotonNetwork.CreateRoom(createInput.text);
        }
    }

    public void JoinRoom()
    {
        Debug.Log("[CreateAndJoinRooms] JoinRoom called. Input: " + joinInput.text);
        if (joinInput.text.Length >= 1)
        {
            Debug.Log("[CreateAndJoinRooms] Joining room: " + joinInput.text);
            PhotonNetwork.JoinRoom(joinInput.text);
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("[CreateAndJoinRooms] OnJoinedRoom called. IsMasterClient: " + PhotonNetwork.IsMasterClient);
        // Let the MasterClient own scene loading. Others will auto-sync.
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[CreateAndJoinRooms] Loading WaitingRoom scene as MasterClient.");
            PhotonNetwork.LoadLevel("WaitingRoom");
        }
        // Non-master: do not load a scene here when AutomaticallySyncScene is true.
    }
}
