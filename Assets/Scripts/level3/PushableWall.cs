using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PushableWall : MonoBehaviour
{
    public float pushForce = 10f;
    public float friction = 1f; // Friction for wall
    public float gravityScale = 1f; // Gravity for wall
    private PhysicsMaterial2D wallMaterial;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = gravityScale; // Wall will fall down when not supported
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent wall from rotating

        // Create and assign friction material
        wallMaterial = new PhysicsMaterial2D("WallFriction") { friction = friction, bounciness = 0f };
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.sharedMaterial = wallMaterial;
    }

    public void Push(Vector2 direction)
    {
        rb.AddForce(direction * pushForce, ForceMode2D.Impulse);
    }

    // Optional: Detect player collision and push automatically
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get direction from player to wall
            Vector2 pushDir = (transform.position - collision.transform.position).normalized;
            // Only push if player is moving towards the wall
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null && Mathf.Abs(player.HorizontalMovement) > 0.1f)
            {
                // Push in the direction player is moving
                Vector2 playerDir = new Vector2(Mathf.Sign(player.HorizontalMovement), 0);
                Push(playerDir);
            }
        }
    }
}
