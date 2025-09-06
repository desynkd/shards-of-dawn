using UnityEngine;

[System.Serializable]
public class PushableWallSetup : MonoBehaviour
{
    [Header("Setup Instructions")]
    [TextArea(5, 10)]
    public string instructions = @"
PushableWall Setup Instructions:

1. Ensure your wall GameObject has:
   - PushableWall script attached
   - Rigidbody2D component (will be auto-added)
   - BoxCollider2D or similar collider
   - Layer set to appropriate physics layer

2. Player Setup:
   - Player must have 'Player' tag
   - Player script with HorizontalMovement property

3. Recommended Settings:
   - Push Force: 5-10 (for side pushing)
   - Tip Force: 8-15 (for tipping when on top)
   - Mass: 2-5 (heavier = harder to push)
   - Friction: 0.4-0.8 (controls sliding)
   - Gravity Scale: 1-3 (affects falling speed)
   - Linear Drag: 0.5-1 (prevents excessive sliding)
   - Angular Drag: 2-5 (controls rotation damping)

4. Testing:
   - Stand on top of wall and move left/right to tip it
   - Push from the side to slide it
   - Wall should rotate and fall realistically
";

    [Header("Quick Setup")]
    public bool autoSetupWall = false;

    void Start()
    {
        if (autoSetupWall)
        {
            SetupWallAutomatically();
        }
    }

    private void SetupWallAutomatically()
    {
        PushableWall wall = GetComponent<PushableWall>();
        if (wall == null)
        {
            Debug.LogError("No PushableWall component found!");
            return;
        }

        // Ensure proper components exist
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }

        // Set recommended values
        wall.pushForce = 7f;
        wall.tipForce = 10f;
        wall.mass = 3f;
        wall.friction = 0.6f;
        wall.gravityScale = 2f;
        wall.linearDrag = 0.7f;
        wall.angularDrag = 3f;
        wall.tipThreshold = 0.3f;

        Debug.Log("PushableWall setup completed with recommended values!");
    }

    // Method to test the wall setup
    [ContextMenu("Test Wall Setup")]
    public void TestWallSetup()
    {
        PushableWall wall = GetComponent<PushableWall>();
        if (wall == null)
        {
            Debug.LogError("No PushableWall component found!");
            return;
        }

        // Test horizontal push
        wall.Push(Vector2.right);
        Debug.Log("Testing horizontal push to the right");

        // Test tip
        Invoke(nameof(TestTip), 2f);
    }

    private void TestTip()
    {
        PushableWall wall = GetComponent<PushableWall>();
        if (wall != null)
        {
            wall.TipWall(Vector2.right);
            Debug.Log("Testing wall tip to the right");
        }
    }
}
