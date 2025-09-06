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

    public void StartFinalGameEndSequence(Vector2 endPosition, float endZoom, float moveSpeed, float zoomSpeed, float visionRevealSpeed, float fadeSpeed, float transitionDelay, string victoryMessage, float victoryTextDisplayTime)
    {
        if (isEndingSequence) return;
        isEndingSequence = true;
        StartCoroutine(FinalGameEndSequence(endPosition, endZoom, moveSpeed, zoomSpeed, visionRevealSpeed, fadeSpeed, transitionDelay, victoryMessage, victoryTextDisplayTime));
    }

    private IEnumerator FinalGameEndSequence(Vector2 endPosition, float endZoom, float moveSpeed, float zoomSpeed, float visionRevealSpeed, float fadeSpeed, float transitionDelay, string victoryMessage, float victoryTextDisplayTime)
    {
        Debug.Log("🎊 Starting FINAL game end sequence! 🎊");

        // Step 1: Start revealing full vision by increasing global lighting
        StartCoroutine(RevealFullVision(visionRevealSpeed));

        // Step 2: Move camera to end position and zoom out (slower for dramatic effect)
        yield return StartCoroutine(MoveCameraToPosition(endPosition, endZoom, moveSpeed, zoomSpeed));

        // Step 3: Show victory message
        yield return StartCoroutine(ShowVictoryMessage(victoryMessage, victoryTextDisplayTime));

        // Step 4: Wait a bit longer for the final celebration
        yield return new WaitForSeconds(0.5f);

        // Step 5: Start golden/white fade (more beautiful for final level)
        yield return StartCoroutine(GoldenFadeIn(fadeSpeed));

        // Step 6: Wait for transition delay (longer for final level)
        yield return new WaitForSeconds(transitionDelay);

        // Step 7: Load Loading scene
        LoadLoadingScene();
    }

    private IEnumerator ShowVictoryMessage(string message, float displayTime)
    {
        Debug.Log("Showing victory message...");

        // Create victory text canvas
        CreateVictoryCanvas(message);

        // Fade in the text
        float elapsedTime = 0f;
        Color startColor = new Color(1f, 1f, 1f, 0f);
        Color endColor = new Color(1f, 1f, 1f, 1f);

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
        Debug.Log("Revealing full vision with dramatic lighting...");
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
            GameObject globalLightGO = new GameObject("FinalGlobalLight");
            Light2D globalLight = globalLightGO.AddComponent<Light2D>();
            globalLight.lightType = Light2D.LightType.Global;
            globalLight.intensity = 0f; // Start with no light
            globalLights.Add(globalLight);
        }

        // Gradually increase intensity to maximum (more dramatic for final level)
        float elapsedTime = 0f;
        float maxIntensity = 1.5f; // Brighter than normal for final celebration

        while (elapsedTime < speed)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / speed;
            float currentIntensity = Mathf.Lerp(0f, maxIntensity, progress);

            foreach (var light in globalLights)
            {
                if (light != null)
                {
                    light.intensity = currentIntensity;
                }
            }

            yield return null;
        }

        // Ensure all lights are at maximum intensity
        foreach (var light in globalLights)
        {
            if (light != null)
            {
                light.intensity = maxIntensity;
            }
        }

        Debug.Log("Full vision revealed with enhanced brightness!");
    }

    private IEnumerator MoveCameraToPosition(Vector2 endPosition, float endZoom, float moveSpeed, float zoomSpeed)
    {
        if (mainCamera == null) yield break;

        Debug.Log($"Moving camera to final position: {endPosition} with zoom: {endZoom}");

        Vector3 startPosition = mainCamera.transform.position;
        Vector3 targetPosition = new Vector3(endPosition.x, endPosition.y, startPosition.z);
        float startSize = mainCamera.orthographicSize;

        float elapsedTime = 0f;
        float totalTime = Mathf.Max(
            Vector3.Distance(startPosition, targetPosition) / moveSpeed,
            Mathf.Abs(startSize - endZoom) / zoomSpeed
        );

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / totalTime;

            // Smooth camera movement and zoom
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            mainCamera.orthographicSize = Mathf.Lerp(startSize, endZoom, progress);

            yield return null;
        }

        // Ensure final position and zoom
        mainCamera.transform.position = targetPosition;
        mainCamera.orthographicSize = endZoom;

        Debug.Log("Camera movement completed");
    }

    private IEnumerator GoldenFadeIn(float speed)
    {
        Debug.Log("Starting golden fade for final victory...");
        // Create fade canvas
        CreateFadeCanvas();

        float elapsedTime = 0f;
        Color startColor = new Color(1f, 0.9f, 0.7f, 0f); // Golden color instead of pure white
        Color endColor = new Color(1f, 0.95f, 0.8f, 1f); // Bright golden

        while (elapsedTime < speed)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / speed;
            fadeImage.color = Color.Lerp(startColor, endColor, progress);
            yield return null;
        }

        // Ensure fully golden
        fadeImage.color = endColor;

        Debug.Log("Golden fade completed - game victory achieved!");
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
            victoryText.color = new Color(1f, 1f, 1f, 0f); // Start transparent
            victoryText.alignment = TextAlignmentOptions.Center;
            victoryText.fontStyle = FontStyles.Bold;

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
        // Create fade canvas if it doesn't exist
        if (fadeCanvas == null)
        {
            GameObject canvasGO = new GameObject("FadeCanvas");
            fadeCanvas = canvasGO.AddComponent<Canvas>();
            fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            fadeCanvas.sortingOrder = 1000; // Make sure it's on top of everything

            // Add Canvas Scaler
            var canvasScaler = canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            // Add GraphicRaycaster
            canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        // Create fade image if it doesn't exist
        if (fadeImage == null)
        {
            GameObject imageGO = new GameObject("FadeImage");
            imageGO.transform.SetParent(fadeCanvas.transform, false);

            fadeImage = imageGO.AddComponent<UnityEngine.UI.Image>();
            fadeImage.color = new Color(1f, 1f, 1f, 0f); // Start transparent

            // Set up RectTransform to cover the entire screen
            RectTransform rectTransform = imageGO.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }

    private void LoadLoadingScene()
    {
        Debug.Log("🎯 Loading the Loading scene - Game Completed! 🎯");

        // Reset camera and features before scene transition
        ResetFeatures();

        // Load Loading scene (the game is complete!)
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("Loading");
            }
        }
        else
        {
            SceneManager.LoadScene("Loading");
        }
    }

    private void ResetFeatures()
    {
        Debug.Log("Resetting all features before final scene transition...");

        // Reset camera
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCameraPosition;
            mainCamera.orthographicSize = originalCameraSize;
        }

        // Reset lighting to normal
        var allLights = FindObjectsByType<Light2D>(FindObjectsSortMode.None);
        foreach (var light in allLights)
        {
            if (light.lightType == Light2D.LightType.Global)
            {
                light.intensity = 0.3f; // Reset to normal game lighting
            }
        }

        Debug.Log("All features reset for final transition!");
    }
}
