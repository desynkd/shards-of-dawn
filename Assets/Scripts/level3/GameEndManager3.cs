using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using TMPro;

public class GameEndManager3 : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 originalCameraPosition;
    private float originalCameraSize;
    private bool isEndingSequence = false;

    // UI elements for white fade and victory text
    private Canvas fadeCanvas;
    private UnityEngine.UI.Image fadeImage;
    private Canvas victoryCanvas;
    private TextMeshProUGUI victoryText;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
            originalCameraSize = mainCamera.orthographicSize;
        }
    }

    public void StartGameEndSequence(Vector2 endPosition, float endZoom, float moveSpeed, float zoomSpeed, float visionRevealSpeed, float fadeSpeed, float transitionDelay, string victoryMessage, float victoryTextDisplayTime)
    {
        if (isEndingSequence) return;
        isEndingSequence = true;
        StartCoroutine(GameEndSequence(endPosition, endZoom, moveSpeed, zoomSpeed, visionRevealSpeed, fadeSpeed, transitionDelay, victoryMessage, victoryTextDisplayTime));
    }

    private IEnumerator GameEndSequence(Vector2 endPosition, float endZoom, float moveSpeed, float zoomSpeed, float visionRevealSpeed, float fadeSpeed, float transitionDelay, string victoryMessage, float victoryTextDisplayTime)
    {
        Debug.Log("Starting final game end sequence...");

        // Step 1: Start revealing full vision by increasing global lighting
        StartCoroutine(RevealFullVision(visionRevealSpeed));

        // Step 2: Move camera to end position and zoom out
        yield return StartCoroutine(MoveCameraToPosition(endPosition, endZoom, moveSpeed, zoomSpeed));

        // Step 3: Show victory message
        yield return StartCoroutine(ShowVictoryMessage(victoryMessage, victoryTextDisplayTime));

        // Step 4: Wait a bit for the victory message
        yield return new WaitForSeconds(0.5f);

        // Step 5: Start white fade
        yield return StartCoroutine(WhiteFadeIn(fadeSpeed));

        // Step 6: Wait for transition delay
        yield return new WaitForSeconds(transitionDelay);

        // Step 7: Load game lobby scene
        LoadGameLobby();
    }

    private IEnumerator ShowVictoryMessage(string message, float displayTime)
    {
        Debug.Log("Showing victory message...");

        // Create victory text canvas
        CreateVictoryCanvas(message);

        // Fade in the text
        float elapsedTime = 0f;
        Color startColor = new Color(0.1f, 0.1f, 0.4f, 0f); // Dark blue with alpha 0
        Color endColor = new Color(0.1f, 0.1f, 0.4f, 1f); // Dark blue with alpha 1

        while (elapsedTime < 1f) // 1 second fade in
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / 1f;
            victoryText.color = Color.Lerp(startColor, endColor, progress);
            yield return null;
        }

        victoryText.color = endColor;

        // Keep text visible for display time
        yield return new WaitForSeconds(displayTime);

        // Fade out the text
        elapsedTime = 0f;
        while (elapsedTime < 1f) // 1 second fade out
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / 1f;
            victoryText.color = Color.Lerp(endColor, startColor, progress);
            yield return null;
        }

        victoryText.color = startColor;
        Debug.Log("Victory message completed");
    }

    private IEnumerator RevealFullVision(float speed)
    {
        Debug.Log("Revealing full vision by increasing global lighting...");
        // Find all global lights
        var allLights = FindObjectsByType<Light2D>(FindObjectsSortMode.None);
        var globalLights = new List<Light2D>();

        foreach (var light in allLights)
        {
            if (light.lightType == Light2D.LightType.Global)
            {
                globalLights.Add(light);
            }
        }

        if (globalLights.Count == 0)
        {
            Debug.LogWarning("No global lights found! Creating one...");
            // Create a global light if none exists
            GameObject globalLightGO = new GameObject("GlobalLight");
            Light2D globalLight = globalLightGO.AddComponent<Light2D>();
            globalLight.lightType = Light2D.LightType.Global;
            globalLight.intensity = 0f; // Start with no light
            globalLights.Add(globalLight);
        }

        // Gradually increase global light intensity to reveal full vision
        float elapsedTime = 0f;
        float startIntensity = 0f;
        float targetIntensity = 1.5f; // Brighter for final level

        while (elapsedTime < speed)
        {
            elapsedTime += Time.deltaTime;
            float currentIntensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / speed);

            foreach (Light2D globalLight in globalLights)
            {
                if (globalLight != null)
                {
                    globalLight.intensity = currentIntensity;
                }
            }

            yield return null;
        }

        // Ensure full brightness
        foreach (Light2D globalLight in globalLights)
        {
            if (globalLight != null)
            {
                globalLight.intensity = targetIntensity;
            }
        }

        Debug.Log("Full vision revealed - global lighting at maximum");
    }

    private IEnumerator MoveCameraToPosition(Vector2 endPosition, float endZoom, float moveSpeed, float zoomSpeed)
    {
        Debug.Log("Moving camera to end position...");
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            yield break;
        }

        Vector3 targetPosition = new Vector3(endPosition.x, endPosition.y, mainCamera.transform.position.z);
        float elapsedTime = 0f;
        float moveDuration = Vector3.Distance(mainCamera.transform.position, targetPosition) / moveSpeed;
        float zoomDuration = Mathf.Abs(endZoom - mainCamera.orthographicSize) / zoomSpeed;
        float totalDuration = Mathf.Max(moveDuration, zoomDuration);

        Vector3 startPosition = mainCamera.transform.position;
        float startSize = mainCamera.orthographicSize;

        while (elapsedTime < totalDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / totalDuration;

            // Move camera
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            // Zoom camera
            mainCamera.orthographicSize = Mathf.Lerp(startSize, endZoom, progress);

            yield return null;
        }

        // Ensure final position and zoom
        mainCamera.transform.position = targetPosition;
        mainCamera.orthographicSize = endZoom;

        Debug.Log("Camera moved to end position and zoomed out");
    }

    private IEnumerator WhiteFadeIn(float speed)
    {
        Debug.Log("Starting golden fade...");
        // Create fade canvas
        CreateFadeCanvas();

        float elapsedTime = 0f;
        Color startColor = new Color(1f, 0.9f, 0.7f, 0f); // Slightly golden with alpha 0
        Color endColor = new Color(1f, 0.95f, 0.8f, 1f); // Bright golden with alpha 1

        // Make sure the text is fully visible before starting the fade
        if (victoryText != null)
        {
            victoryText.color = new Color(0.1f, 0.1f, 0.4f, 0f); // Ensure text is hidden as we start the fade
        }

        while (elapsedTime < speed)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / speed;
            fadeImage.color = Color.Lerp(startColor, endColor, progress);
            yield return null;
        }

        // Ensure fully golden
        fadeImage.color = endColor;

        Debug.Log("Golden fade completed");
    }

    private void CreateVictoryCanvas(string message)
    {
        // Create victory canvas if it doesn't exist
        if (victoryCanvas == null)
        {
            GameObject canvasGO = new GameObject("VictoryCanvas");
            victoryCanvas = canvasGO.AddComponent<Canvas>();
            victoryCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            victoryCanvas.sortingOrder = 999; // Make sure it's on top

            // Add Canvas Scaler
            var canvasScaler = canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            // Add GraphicRaycaster
            canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        // Create victory text if it doesn't exist
        if (victoryText == null)
        {
            GameObject textGO = new GameObject("VictoryText");
            textGO.transform.SetParent(victoryCanvas.transform, false);

            victoryText = textGO.AddComponent<TextMeshProUGUI>();
            victoryText.text = message;
            victoryText.fontSize = 48;
            victoryText.color = new Color(0.1f, 0.1f, 0.4f, 0f); // Start with transparent dark blue
            victoryText.alignment = TextAlignmentOptions.Center;
            victoryText.fontStyle = FontStyles.Bold;

            // Add a drop shadow for better visibility
            victoryText.enableVertexGradient = true;
            victoryText.colorGradient = new VertexGradient(
                new Color(0.1f, 0.1f, 0.4f, 1f), // Top left - dark blue
                new Color(0.1f, 0.1f, 0.4f, 1f), // Top right - dark blue
                new Color(0f, 0f, 0.2f, 1f),     // Bottom left - darker blue
                new Color(0f, 0f, 0.2f, 1f)      // Bottom right - darker blue
            );

            // Add outline for even better visibility
            victoryText.outlineWidth = 0.2f;
            victoryText.outlineColor = new Color(0f, 0f, 0f, 0.5f); // Semi-transparent black

            // Set up RectTransform
            RectTransform rectTransform = textGO.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
        else
        {
            victoryText.text = message;
        }
    }

    private void CreateFadeCanvas()
    {
        // Create canvas
        GameObject canvasGO = new GameObject("FadeCanvas");
        fadeCanvas = canvasGO.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 1000; // Ensure it's on top
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // Create fade image
        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform, false);
        fadeImage = imageGO.AddComponent<UnityEngine.UI.Image>();
        fadeImage.color = new Color(1f, 1f, 1f, 0f);

        // Set image to fill screen
        RectTransform rectTransform = fadeImage.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private void LoadGameLobby()
    {
        Debug.Log("Loading Game Lobby...");

        // Reset camera and torch features before scene transition
        ResetFeatures();

        // Load lobby scene - this is the final end of the game
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("GameLobby");
            }
        }
        else
        {
            SceneManager.LoadScene("GameLobby");
        }
    }

    private void ResetFeatures()
    {
        Debug.Log("Resetting camera and lighting features...");

        // Reset camera
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCameraPosition;
            mainCamera.orthographicSize = originalCameraSize;
        }

        // Reset global lighting to dark state
        var allLights = FindObjectsByType<Light2D>(FindObjectsSortMode.None);
        foreach (var light in allLights)
        {
            if (light.lightType == Light2D.LightType.Global)
            {
                light.intensity = 0f; // Reset to original dark state
            }
        }

        // Reset player torches to original intensity
        PlayerTorch[] playerTorches = FindObjectsByType<PlayerTorch>(FindObjectsSortMode.None);
        foreach (var playerTorch in playerTorches)
        {
            Light2D torch = playerTorch.GetTorchLight();
            if (torch != null)
            {
                torch.intensity = playerTorch.intensity; // Reset to original intensity
            }
        }
    }
}
