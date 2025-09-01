using UnityEngine;
using Photon.Pun;

public class PressureButton03 : MonoBehaviour
{
    public GameObject yellowLauncherTilemap; // Assign in Inspector
    public Sprite pressedSprite;
    public Sprite unpressedSprite;
    private SpriteRenderer spriteRenderer;
    private int playerCount;
    private bool isPressed = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        playerCount = Photon.Pun.PhotonNetwork.CurrentRoom != null ? Photon.Pun.PhotonNetwork.CurrentRoom.PlayerCount : 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.sprite = pressedSprite;
            isPressed = true;

            if (playerCount <= 3)
            {
                yellowLauncherTilemap.SetActive(false); // Permanently deactivate
            }
            else if (playerCount >= 4)
            {
                yellowLauncherTilemap.SetActive(false); // Temporarily deactivate
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.sprite = unpressedSprite;
            isPressed = false;

            if (playerCount >= 4)
            {
                yellowLauncherTilemap.SetActive(true); // Reactivate when player leaves
            }
        }
    }
}
