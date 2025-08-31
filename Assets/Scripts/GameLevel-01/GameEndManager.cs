using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class GameEndManager : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 originalCameraPosition;
    private float originalCameraSize;
    private bool isEndingSequence = false;
    
    // UI elements for white fade
    private Canvas fadeCanvas;
    private UnityEngine.UI.Image fadeImage;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
            originalCameraSize = mainCamera.orthographicSize;
        }
    }

    public void StartGameEndSequence(Vector2 endPosition, float endZoom, float moveSpeed, float zoomSpeed, float visionRevealSpeed, float fadeSpeed, float transitionDelay)
    {
        if (isEndingSequence) return;
        
        isEndingSequence = true;
        StartCoroutine(GameEndSequence(endPosition, endZoom, moveSpeed, zoomSpeed, visionRevealSpeed, fadeSpeed, transitionDelay));
    }

    private IEnumerator GameEndSequence(Vector2 endPosition, float endZoom, float moveSpeed, float zoomSpeed, float visionRevealSpeed, float fadeSpeed, float transitionDelay)
    {
        Debug.Log("Starting game end sequence...");

        // Step 1: Start revealing full vision by increasing global lighting
        StartCoroutine(RevealFullVision(visionRevealSpeed));

        // Step 2: Move camera to end position and zoom out
        yield return StartCoroutine(MoveCameraToPosition(endPosition, endZoom, moveSpeed, zoomSpeed));

        // Step 3: Wait a bit for the reveal
        yield return new WaitForSeconds(1f);

        // Step 4: Start white fade
        yield return StartCoroutine(WhiteFadeIn(fadeSpeed));

        // Step 5: Wait for transition delay
        yield return new WaitForSeconds(transitionDelay);

        // Step 6: Load next scene
        LoadNextScene();
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
        float targetIntensity = 1f; // Full brightness

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
        Debug.Log("Starting white fade...");
        
        // Create fade canvas
        CreateFadeCanvas();

        float elapsedTime = 0f;
        Color startColor = new Color(1f, 1f, 1f, 0f);
        Color endColor = new Color(1f, 1f, 1f, 1f);

        while (elapsedTime < speed)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / speed;
            fadeImage.color = Color.Lerp(startColor, endColor, progress);
            yield return null;
        }

        // Ensure fully white
        fadeImage.color = endColor;

        Debug.Log("White fade completed");
    }

    private void CreateFadeCanvas()
    {
        // Create canvas
        GameObject canvasGO = new GameObject("FadeCanvas");
        fadeCanvas = canvasGO.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 999; // Ensure it's on top
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

    private void LoadNextScene()
    {
        Debug.Log("Loading GameLevel02...");
        
        // Reset camera and torch features before scene transition
        ResetFeatures();

        // Load next scene
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("GameLevel02");
            }
        }
        else
        {
            SceneManager.LoadScene("GameLevel02");
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
