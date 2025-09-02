using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PushableWall : MonoBehaviour
{
    [Header("Physics Settings")]
    public float pushForce = 5f;
    public float tipForce = 8f; // Force applied when tipping the wall
    public float mass = 2f; // Mass of the wall
    public float friction = 0.6f; // Friction for wall
    public float gravityScale = 2f; // Gravity for wall
    public float linearDrag = 0.5f; // Linear drag to prevent sliding too much
    public float angularDrag = 3f; // Angular drag to control rotation

    [Header("Tip Settings")]
    public float tipThreshold = 0.3f; // How much horizontal movement needed to start tipping

    private PhysicsMaterial2D wallMaterial;
    private Rigidbody2D rb;
    private bool tipped = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.mass = mass;
        rb.gravityScale = gravityScale;
        rb.linearDamping = linearDrag;
        rb.angularDamping = angularDrag;

        // Start with rotation frozen so the wall behaves like a stable block until tipped
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Create and assign friction material
        wallMaterial = new PhysicsMaterial2D("WallFriction") { friction = friction, bounciness = 0.1f };
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.sharedMaterial = wallMaterial;
    }

    public void Push(Vector2 direction)
    {
        rb.AddForce(direction * pushForce, ForceMode2D.Impulse);
    }

    // Method to manually tip the wall (can be called from other scripts)
    public void TipWall(Vector2 direction)
    {
        if (!tipped)
        {
            rb.constraints = RigidbodyConstraints2D.None;
            Vector2 tipPoint = transform.position + Vector3.up * GetComponent<Collider2D>().bounds.size.y * 0.4f;
            rb.AddForceAtPosition(direction * tipForce, tipPoint, ForceMode2D.Impulse);
            tipped = true;
        }
    }

    // Reset the wall to its upright position (useful for game resets)
    public void ResetWall()
    {
        tipped = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        transform.rotation = Quaternion.identity;
        rb.angularVelocity = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    // Optional: Detect player collision and push automatically
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player == null)
                return;

            // Check contact points to see if player is on top of the wall
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // A contact normal with substantial upward Y means the player is standing on the platform
                if (contact.normal.y > 0.5f)
                {
                    // Player is on top. If they're moving horizontally, tip the wall.
                    if (Mathf.Abs(player.HorizontalMovement) > tipThreshold)
                    {
                        Vector2 playerDir = new Vector2(Mathf.Sign(player.HorizontalMovement), 0);

                        // If not already tipped, allow rotation and apply an impulse at the contact point to tip it over
                        if (!tipped)
                        {
                            // Unfreeze rotation so the wall can rotate
                            rb.constraints = RigidbodyConstraints2D.None;
                            // Apply an impulse at the contact point to create torque
                            rb.AddForceAtPosition(playerDir * tipForce, contact.point, ForceMode2D.Impulse);
                            tipped = true;

                            Debug.Log("Wall tipped by player standing on top!");
                        }
                        else
                        {
                            // If already tipping, continue applying small pushes so the player can finish the job
                            rb.AddForceAtPosition(playerDir * (tipForce * 0.3f), contact.point, ForceMode2D.Impulse);
                        }

                        // We already handled tipping from a top contact; exit the loop
                        return;
                    }
                }
            }

            // Fallback: side pushing behavior (player pushing the wall from its side)
            if (Mathf.Abs(player.HorizontalMovement) > 0.1f)
            {
                Vector2 playerDir = new Vector2(Mathf.Sign(player.HorizontalMovement), 0);

                // Check if the player is actually pushing against the wall (not moving away from it)
                Vector2 playerToWall = (transform.position - player.transform.position).normalized;
                float pushAlignment = Vector2.Dot(playerDir, playerToWall);

                if (pushAlignment > 0.1f) // Player is pushing towards the wall
                {
                    Push(playerDir);

                    // If pushing hard enough and wall isn't tipped yet, consider tipping it
                    if (!tipped && Mathf.Abs(player.HorizontalMovement) > tipThreshold)
                    {
                        TipWall(playerDir);
                    }
                }
            }
        }
    }

    // Visual debugging
    private void OnDrawGizmosSelected()
    {
        if (rb != null)
        {
            // Draw center of mass
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(rb.worldCenterOfMass, 0.1f);

            // Draw tip point
            if (GetComponent<Collider2D>() != null)
            {
                Gizmos.color = Color.yellow;
                Vector2 tipPoint = transform.position + Vector3.up * GetComponent<Collider2D>().bounds.size.y * 0.4f;
                Gizmos.DrawWireSphere(tipPoint, 0.05f);
            }
        }
    }
}
