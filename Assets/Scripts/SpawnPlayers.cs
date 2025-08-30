using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public float minX, maxX, minY, maxY;

    private bool hasSpawned = false;

    private void OnEnable()
    {
        Debug.Log("[SpawnPlayers] OnEnable called. PhotonNetwork.InRoom: " + PhotonNetwork.InRoom + ", hasSpawned: " + hasSpawned);
        if (!hasSpawned)
        {
            StartCoroutine(WaitForJoinAndSpawn());
        }
    }

    private IEnumerator WaitForJoinAndSpawn()
    {
        while (!PhotonNetwork.InRoom || PhotonNetwork.NetworkClientState != ClientState.Joined)
        {
            yield return null; // Wait until next frame
        }
        if (!hasSpawned)
        {
            Debug.Log("[SpawnPlayers] Spawning after join process completed (coroutine).");
            SpawnMyPlayer();
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("[SpawnPlayers] OnJoinedRoom called. hasSpawned: " + hasSpawned);
        if (!hasSpawned)
        {
            Debug.Log("[SpawnPlayers] Attempting to spawn player from OnJoinedRoom.");
            SpawnMyPlayer();
        }
    }

    private void SpawnMyPlayer()
    {
        hasSpawned = true;
        Vector2 randomPosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        Debug.Log("[SpawnPlayers] Spawning player at position: " + randomPosition + " with prefab: " + playerPrefab.name);
        PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
    }
}
