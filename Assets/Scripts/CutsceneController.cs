using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviourPunCallbacks
{
    [Header("Cutscene Settings")]
    [Tooltip("Duration of cutscene in seconds")]
    public float cutsceneDuration = 5f;

    [Tooltip("Name of the next scene to load after the cutscene")]
    public string nextSceneName;

    private float startTime;
    private bool cutsceneEnded = false;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"[CutsceneController] MasterClient starting cutscene. Sending RPC_StartCutscene with nextSceneName: {nextSceneName} at network time: {PhotonNetwork.Time}");
            photonView.RPC("RPC_StartCutscene", RpcTarget.AllBuffered, (float)PhotonNetwork.Time, nextSceneName);
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && !cutsceneEnded)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("[CutsceneController] Enter pressed by MasterClient. Skipping cutscene.");
                cutsceneEnded = true;
                photonView.RPC("RPC_SkipCutscene", RpcTarget.AllBuffered, nextSceneName);
            }
        }
    }

    [PunRPC]
    void RPC_StartCutscene(float networkStartTime, string sceneName)
    {
        Debug.Log($"[CutsceneController] RPC_StartCutscene received. networkStartTime: {networkStartTime}, sceneName: {sceneName}, local PhotonNetwork.Time: {PhotonNetwork.Time}");
        startTime = networkStartTime;
        StartCoroutine(CutsceneTimer(sceneName));
    }

    private System.Collections.IEnumerator CutsceneTimer(string sceneName)
    {
        float timePassed = (float)(PhotonNetwork.Time - startTime);
        float timeLeft = Mathf.Max(0, cutsceneDuration - timePassed);

        Debug.Log($"[CutsceneController] CutsceneTimer started. timePassed: {timePassed}, timeLeft: {timeLeft}");

        yield return new WaitForSeconds(timeLeft);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"[CutsceneController] Cutscene finished. MasterClient loading scene: {sceneName}");
            PhotonNetwork.LoadLevel(sceneName);
        }
    }

    [PunRPC]
    void RPC_SkipCutscene(string sceneName)
    {
        if (!cutsceneEnded)
        {
            cutsceneEnded = true;
            Debug.Log($"[CutsceneController] RPC_SkipCutscene received. Loading scene: {sceneName}");
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(sceneName);
            }
        }
    }
}
