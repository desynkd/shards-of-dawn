using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class DontPushButton : MonoBehaviourPun
{
    [Header("Death Settings")]
    public float deathDelay = 1.5f; // Delay before respawning player
    public bool showDeathMessage = true; // Whether to show a death message

    [Header("Respawn Settings")]
    public Vector3 respawnPosition = new Vector3(-19f, 0f, 0f); // Position to respawn player at

    private bool triggered = false;
    private Player triggeringPlayer = null;

    private void Awake()
    {
        // Make sure we have a collider and it's set as a trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning("[DontPushButton] Collider is not set as a trigger. Setting it to trigger mode.");
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHandle(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryHandle(collision.collider.gameObject);
    }

    private void TryHandle(GameObject other)
    {
        if (triggered) return;

        // Try to get player component in different ways
        var player = other.GetComponentInParent<Player>();
        if (player == null) player = other.GetComponent<Player>();
        if (player == null) return;

        // Only trigger for local player
        if (player.pv != null && !player.pv.IsMine) return;

        // Disable the DontPushButtonManager if it exists to prevent competing behavior
        var buttonManager = GetComponent<DontPushButtonManager>();
        if (buttonManager != null)
        {
            buttonManager.enabled = false;
            Debug.Log("[DontPushButton] Disabled DontPushButtonManager to prevent conflict");
        }

        // Store reference to the player
        triggeringPlayer = player;
        triggered = true;

        Debug.Log($"[DontPushButton] Player {player.pv?.OwnerActorNr} pushed the button! Triggering death sequence...");

        // Show death message for this client immediately
        if (showDeathMessage)
        {
            ShowDeathUI();
        }

        // Schedule player respawn after delay
        if (deathDelay > 0)
        {
            Invoke(nameof(RespawnPlayerDelayed), deathDelay);
        }
        else
        {
            // Immediately respawn if no delay
            RespawnPlayer(player);
        }
    }

    private void RespawnPlayerDelayed()
    {
        if (triggeringPlayer != null && triggeringPlayer.pv != null && triggeringPlayer.pv.IsMine)
        {
            // Use the stored reference to respawn the player
            RespawnPlayer(triggeringPlayer);
        }
        else
        {
            // Fallback to finding player if reference is lost
            Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
            foreach (Player player in players)
            {
                if (player.pv != null && player.pv.IsMine)
                {
                    RespawnPlayer(player);
                    break;
                }
            }
        }
    }

    private void RespawnPlayer(Player player)
    {
        // Shift player position to respawn point
        player.transform.position = respawnPosition;
        Debug.Log($"[DontPushButton] Player respawned at {respawnPosition}");

        // Reset variables to allow future interactions
        triggeringPlayer = null;
        triggered = false;
    }

    private void ShowDeathUI()
    {
        // Create a simple death message UI
        var canvasGo = new GameObject("DeathCanvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000; // Ensure it's on top
        canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        var textGo = new GameObject("DeathText");
        textGo.transform.SetParent(canvasGo.transform, false);
        var text = textGo.AddComponent<Text>();
        text.text = "DON'T PUSH THE BUTTON!\nRespawning...";
        text.color = Color.red;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontSize = 48;

        // Try to get a font
        try
        {
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        catch
        {
            text.font = Font.CreateDynamicFontFromOSFont("Arial", 48);
        }

        // Set text to fill screen
        var rt = text.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Debug.Log("[DontPushButton] Death UI displayed");

        // Destroy the UI after the respawn delay
        Destroy(canvasGo, deathDelay + 0.5f);
    }
}
