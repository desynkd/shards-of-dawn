using UnityEngine;

public class DontPushButton : MonoBehaviour
{
    private GameEndManager gameEndManager;

    private void Start()
    {
        // Find or create the GameEndManager
        gameEndManager = FindFirstObjectByType<GameEndManager>();
        if (gameEndManager == null)
        {
            GameObject managerGO = new GameObject("GameEndManager");
            gameEndManager = managerGO.AddComponent<GameEndManager>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Call the game over logic using the existing manager
            // Use the same parameters as GameEndCrystal for consistency
            gameEndManager.StartGameEndSequence(
                new Vector2(0, 0), // You can set this to your desired camera position
                10f,                // Camera zoom
                2f,                 // Camera move speed
                1f,                 // Camera zoom speed
                1f,                 // Vision reveal speed
                1f,                 // White fade speed
                2f                  // Scene transition delay
            );
        }
    }
}
