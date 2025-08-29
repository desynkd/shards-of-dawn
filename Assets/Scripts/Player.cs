using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

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

        // Choose a sprite index on the owning client and sync to everyone
        if (pv.IsMine)
        {
            int chosenIndex = 0;

            if (possibleSprites != null && possibleSprites.Length > 0)
            {
                // Example strategy: based on join order
                chosenIndex = PhotonNetwork.LocalPlayer.ActorNumber % possibleSprites.Length;

                // Alternative strategies:
                // chosenIndex = Random.Range(0, possibleSprites.Length);
                // chosenIndex = YourSelectionFromMenu;
            }

            pv.RPC(nameof(SetPlayerSprite), RpcTarget.AllBuffered, chosenIndex);
        }
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
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundLayer))
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }

}
