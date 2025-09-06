using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using ExitGames.Client.Photon; // Needed for Hashtable
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public PhotonView pv;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;   // Assign in Inspector (or leave null to auto-find on Start)
    public Sprite[] possibleSprites;        // Assign available skins here

    [Header("Movement")]
    public float moveSpeed = 5f;
    private float horizontalMovement;
    public float HorizontalMovement => horizontalMovement;

    [Header("Jump")]
    public float jumpPower = 10f;

    [Header("Ground Check")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();

        // Auto-find SpriteRenderer if not set
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (pv.IsMine)
        {
            // Check if I already have a sprite stored in my CustomProperties
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("spriteIndex", out object spriteIndexObj))
            {
                int spriteIndex = (int)spriteIndexObj;
                pv.RPC(nameof(SetPlayerSprite), RpcTarget.AllBuffered, spriteIndex);
            }
            else
            {
                // First time assigning sprite
                int chosenIndex = 0;
                if (possibleSprites != null && possibleSprites.Length > 0)
                {
                    chosenIndex = PhotonNetwork.LocalPlayer.ActorNumber % possibleSprites.Length;
                }

                // Store it in my custom properties
                var props = new Hashtable { { "spriteIndex", chosenIndex } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                // Sync with everyone
                pv.RPC(nameof(SetPlayerSprite), RpcTarget.AllBuffered, chosenIndex);
            }
        }
    }

    public void ChangeSize(float scaleMultiplier)
    {
        transform.localScale *= scaleMultiplier;
        groundCheckSize *= scaleMultiplier;
    }

    [PunRPC]
    void RPC_ChangeSize(float scaleMultiplier)
    {
        // This method will be called on all clients
        Debug.Log($"[Player] RPC_ChangeSize called with factor: {scaleMultiplier}");
        ChangeSize(scaleMultiplier);
    }

    void Update()
    {
        if (!pv.IsMine) return;
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!pv.IsMine) return;
        if (!IsGrounded()) return;

        if (context.performed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        }
        else if (context.canceled)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower * 0.25f);
        }
    }

    [PunRPC]
    private void SetPlayerSprite(int index)
    {
        if (possibleSprites == null || possibleSprites.Length == 0) return;
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        index = Mathf.Clamp(index, 0, possibleSprites.Length - 1);
        spriteRenderer.sprite = possibleSprites[index];
    }

    private bool IsGrounded()
    {
        // Check if on ground
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundLayer))
        {
            return true;
        }

        // Check if standing on another player
        if (IsStandingOnPlayer())
        {
            return true;
        }

        return false;
    }

    private bool IsStandingOnPlayer()
    {
        // Cast a ray downward to check if we're standing on a player
        RaycastHit2D hit = Physics2D.Raycast(groundCheckPos.position, Vector2.down, groundCheckSize.y);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            // Make sure we're actually above the player (check if our feet are above their head)
            Player otherPlayer = hit.collider.GetComponent<Player>();
            if (otherPlayer != null && otherPlayer != this)
            {
                // Check if our ground check position is above the other player's collider
                float otherPlayerTop = otherPlayer.GetComponent<Collider2D>().bounds.max.y;
                if (groundCheckPos.position.y > otherPlayerTop)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if collided with Respawn layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Respawn"))
        {
            Debug.Log("[Player] Collided with Respawn layer. Restarting scene.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
