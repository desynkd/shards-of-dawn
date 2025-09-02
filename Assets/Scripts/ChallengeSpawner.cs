using UnityEngine;
using Photon.Pun;

/// <summary>
/// Simple Challenge spawner used by Level04. It uses the serialized scene values
/// (baseObstacleCount, obstaclePerPlayer) to decide how many obstacles to spawn.
/// Assumptions: obstacles can be spawned locally (non-networked). If you need
/// networked obstacles, switch Instantiate -> PhotonNetwork.Instantiate and
/// ensure prefabs are available to Photon (Resources or registered prefabs).
/// </summary>
public class ChallengeSpawner : MonoBehaviour
{
    [Tooltip("List of obstacle prefabs to pick from")]
    public GameObject[] obstaclePrefabs;

    [Tooltip("Base number of obstacles to spawn")]
    public int baseObstacleCount = 8;

    [Tooltip("Extra obstacles per connected player")]
    public int obstaclePerPlayer = 4;

    [Tooltip("Spawn area X min")]
    public float spawnMinX = -7f;
    [Tooltip("Spawn area X max")]
    public float spawnMaxX = 7f;
    [Tooltip("Spawn area Y min")]
    public float spawnMinY = 0f;
    [Tooltip("Spawn area Y max")]
    public float spawnMaxY = 5f;

    void Start()
    {
        int playerCount = PhotonNetwork.InRoom ? PhotonNetwork.PlayerList.Length : 1;
        int total = Mathf.Max(0, baseObstacleCount + obstaclePerPlayer * playerCount);

        for (int i = 0; i < total; i++)
        {
            SpawnOne();
        }
    }

    void SpawnOne()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
            return;

        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        Vector2 pos = new Vector2(Random.Range(spawnMinX, spawnMaxX), Random.Range(spawnMinY, spawnMaxY));
        Instantiate(prefab, pos, Quaternion.identity);
    }
}
