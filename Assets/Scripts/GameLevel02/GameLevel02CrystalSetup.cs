using UnityEngine;

public class GameLevel02CrystalSetup : MonoBehaviour
{
    [Header("Crystal Setup")]
    public Sprite crystalSprite;
    public Vector3 crystalPosition = new Vector3(44f, -9f, 0f);
    public Vector3 crystalScale = new Vector3(2f, 2f, 2f);
    
    [Header("Game End Settings")]
    public Vector2 endCameraPosition = new Vector2(22, -5);
    public float endCameraZoom = 12f;
    public float cameraMoveSpeed = 8f; // Increased from 4f for 2x faster movement
    public float cameraZoomSpeed = 4f; // Increased from 2f for 2x faster zoom
    public float visionRevealSpeed = 0.25f; // Reduced from 0.5f for 2x faster vision reveal
    public float whiteFadeSpeed = 0.25f; // Reduced from 0.5f for 2x faster fade
    public float sceneTransitionDelay = 0.5f; // Reduced from 1f for 2x faster transition

    [ContextMenu("Setup Complete GameLevel02 End")]
    public void SetupCompleteGameLevel02End()
    {
        // Remove old LevelExit components if they exist
        RemoveOldLevelExit();
        
        // Create the new GameLevel02EndCrystal
        CreateGameLevel02EndCrystal();
        
        Debug.Log("GameLevel02 end setup complete! Crystal will load Level03-1 when triggered.");
    }

    private void RemoveOldLevelExit()
    {
        // Find and remove old LevelExit components
        LevelExit[] oldExits = FindObjectsByType<LevelExit>(FindObjectsSortMode.None);
        foreach (var oldExit in oldExits)
        {
            Debug.Log("Removing old LevelExit component from: " + oldExit.gameObject.name);
            DestroyImmediate(oldExit);
        }
    }

    private void CreateGameLevel02EndCrystal()
    {
        // Create the crystal GameObject
        GameObject crystalGO = new GameObject("GameLevel02EndCrystal");
        crystalGO.transform.position = crystalPosition;
        crystalGO.transform.localScale = crystalScale;

        // Add SpriteRenderer
        SpriteRenderer spriteRenderer = crystalGO.AddComponent<SpriteRenderer>();
        if (crystalSprite != null)
        {
            spriteRenderer.sprite = crystalSprite;
        }
        else
        {
            // Try to find a crystal sprite in the project
            Sprite[] crystalSprites = Resources.FindObjectsOfTypeAll<Sprite>();
            foreach (Sprite sprite in crystalSprites)
            {
                if (sprite.name.ToLower().Contains("crystal"))
                {
                    spriteRenderer.sprite = sprite;
                    break;
                }
            }
        }

        // Add the GameEndCrystal2 script
        GameEndCrystal2 gameEndCrystal = crystalGO.AddComponent<GameEndCrystal2>();
        
        // Set the game end settings
        gameEndCrystal.endCameraPosition = endCameraPosition;
        gameEndCrystal.endCameraZoom = endCameraZoom;
        gameEndCrystal.cameraMoveSpeed = cameraMoveSpeed;
        gameEndCrystal.cameraZoomSpeed = cameraZoomSpeed;
        gameEndCrystal.visionRevealSpeed = visionRevealSpeed;
        gameEndCrystal.whiteFadeSpeed = whiteFadeSpeed;
        gameEndCrystal.sceneTransitionDelay = sceneTransitionDelay;

        // Set crystal settings
        gameEndCrystal.isTrigger = true;
        gameEndCrystal.triggerSize = new Vector2(1f, 1f);

        Debug.Log("GameLevel02 End Crystal created at position: " + crystalPosition);
        Debug.Log("Crystal will load Level03-1 when triggered");
    }

    [ContextMenu("Find and Setup Existing Crystal")]
    public void SetupExistingCrystal()
    {
        // Find existing crystal objects
        GameObject[] crystalObjects = GameObject.FindGameObjectsWithTag("Untagged");
        
        foreach (GameObject obj in crystalObjects)
        {
            if (obj.name.ToLower().Contains("crystal"))
            {
                // Check if it already has the GameEndCrystal2 component
                GameEndCrystal2 existingCrystal = obj.GetComponent<GameEndCrystal2>();
                if (existingCrystal == null)
                {
                    // Remove old components if they exist
                    LevelExit oldExit = obj.GetComponent<LevelExit>();
                    if (oldExit != null)
                    {
                        DestroyImmediate(oldExit);
                        Debug.Log("Removed old LevelExit component from: " + obj.name);
                    }

                    // Add the GameEndCrystal2 script
                    GameEndCrystal2 gameEndCrystal = obj.AddComponent<GameEndCrystal2>();
                    
                    // Set the game end settings
                    gameEndCrystal.endCameraPosition = endCameraPosition;
                    gameEndCrystal.endCameraZoom = endCameraZoom;
                    gameEndCrystal.cameraMoveSpeed = cameraMoveSpeed;
                    gameEndCrystal.cameraZoomSpeed = cameraZoomSpeed;
                    gameEndCrystal.visionRevealSpeed = visionRevealSpeed;
                    gameEndCrystal.whiteFadeSpeed = whiteFadeSpeed;
                    gameEndCrystal.sceneTransitionDelay = sceneTransitionDelay;

                    // Set crystal settings
                    gameEndCrystal.isTrigger = true;
                    gameEndCrystal.triggerSize = new Vector2(1f, 1f);

                    Debug.Log("Added GameEndCrystal2 component to: " + obj.name);
                    Debug.Log("Crystal will load Level03-1 when triggered");
                }
                else
                {
                    Debug.Log("GameEndCrystal2 component already exists on: " + obj.name);
                }
            }
        }
    }

    [ContextMenu("Update GameEndCrystal2 Prefab")]
    public void UpdateGameEndCrystal2Prefab()
    {
        Debug.Log("To update the GameEndCrystal2 prefab:");
        Debug.Log("1. Drag the GameEndCrystal2 prefab from Resources into your scene");
        Debug.Log("2. Right-click on this GameLevel02CrystalSetup component");
        Debug.Log("3. Select 'Setup Complete GameLevel02 End'");
        Debug.Log("4. This will create a new crystal with the correct GameEndCrystal2 script");
        Debug.Log("5. You can then replace the old prefab or use the new one directly");
    }
}
