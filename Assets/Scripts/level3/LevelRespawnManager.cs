using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LevelRespawnManager : MonoBehaviour
{
    public static void RespawnCurrentLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(currentSceneName);
            }
        }
        else
        {
            SceneManager.LoadScene(currentSceneName);
        }
    }
}
