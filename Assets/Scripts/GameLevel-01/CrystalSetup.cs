using UnityEngine;

public class CrystalSetup : MonoBehaviour
{
    [Header("Crystal Setup")]
    public Sprite crystalSprite;
    public Vector3 crystalPosition = new Vector3(10f, 0f, 0f);
    public Vector3 crystalScale = new Vector3(2f, 2f, 2f);
    
    [Header("Game End Settings")]
    public Vector2 endCameraPosition = new Vector2(0, 0);
    public float endCameraZoom = 15f;
    public float cameraMoveSpeed = 2f;
    public float cameraZoomSpeed = 1f;
    public float visionRevealSpeed = 1f;
    public float whiteFadeSpeed = 1f;
    public float sceneTransitionDelay = 2f;

    [ContextMenu("Create Game End Crystal")]
    public void CreateGameEndCrystal()
    {
        // Create the crystal GameObject
        GameObject crystalGO = new GameObject("GameEndCrystal");
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

        // Add the GameEndCrystal script
        GameEndCrystal gameEndCrystal = crystalGO.AddComponent<GameEndCrystal>();
        
        // Set the game end settings
        gameEndCrystal.endCameraPosition = endCameraPosition;
        gameEndCrystal.endCameraZoom = endCameraZoom;
        gameEndCrystal.cameraMoveSpeed = cameraMoveSpeed;
        gameEndCrystal.cameraZoomSpeed = cameraZoomSpeed;
        gameEndCrystal.visionRevealSpeed = visionRevealSpeed;
        gameEndCrystal.whiteFadeSpeed = whiteFadeSpeed;
        gameEndCrystal.sceneTransitionDelay = sceneTransitionDelay;

        Debug.Log("Game End Crystal created at position: " + crystalPosition);
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
                // Check if it already has the GameEndCrystal component
                GameEndCrystal existingCrystal = obj.GetComponent<GameEndCrystal>();
                if (existingCrystal == null)
                {
                    // Add the GameEndCrystal script
                    GameEndCrystal gameEndCrystal = obj.AddComponent<GameEndCrystal>();
                    
                    // Set the game end settings
                    gameEndCrystal.endCameraPosition = endCameraPosition;
                    gameEndCrystal.endCameraZoom = endCameraZoom;
                    gameEndCrystal.cameraMoveSpeed = cameraMoveSpeed;
                    gameEndCrystal.cameraZoomSpeed = cameraZoomSpeed;
                    gameEndCrystal.visionRevealSpeed = visionRevealSpeed;
                    gameEndCrystal.whiteFadeSpeed = whiteFadeSpeed;
                    gameEndCrystal.sceneTransitionDelay = sceneTransitionDelay;

                    Debug.Log("Added GameEndCrystal component to: " + obj.name);
                }
                else
                {
                    Debug.Log("GameEndCrystal component already exists on: " + obj.name);
                }
            }
        }
    }
}
