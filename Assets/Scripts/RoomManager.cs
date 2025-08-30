using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button startButton;

    void Start()
    {
        // Hide the button for non-host players
        if (PhotonNetwork.IsMasterClient)
            startButton.gameObject.SetActive(true);
        else
            startButton.gameObject.SetActive(false);

        startButton.onClick.AddListener(OnStartGame);
    }

    private void OnStartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Cutscene01");
        }
    }

    // In case master client leaves, update button visibility
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
            startButton.gameObject.SetActive(true);
        else
            startButton.gameObject.SetActive(false);
    }
}
