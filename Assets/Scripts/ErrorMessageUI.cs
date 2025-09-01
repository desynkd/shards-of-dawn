using UnityEngine;
using TMPro;

public class ErrorMessageUI : MonoBehaviour
{
    private static ErrorMessageUI instance;
    private TextMeshProUGUI errorText;
    
    public static ErrorMessageUI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ErrorMessageUI>();
                if (instance == null)
                {
                    GameObject go = new GameObject("ErrorMessageUI");
                    instance = go.AddComponent<ErrorMessageUI>();
                }
            }
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            CreateErrorMessageUI();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void CreateErrorMessageUI()
    {
        // Find the Canvas in the scene
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in the scene!");
            return;
        }
        
        // Create the error message GameObject
        GameObject errorMessageGO = new GameObject("ErrorMessage");
        errorMessageGO.transform.SetParent(canvas.transform, false);
        
        // Add RectTransform
        RectTransform rectTransform = errorMessageGO.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0f);
        rectTransform.anchorMax = new Vector2(0.5f, 0f);
        rectTransform.anchoredPosition = new Vector2(0, 100);
        rectTransform.sizeDelta = new Vector2(600, 50);
        rectTransform.pivot = new Vector2(0.5f, 0f);
        
        // Add TextMeshProUGUI component
        errorText = errorMessageGO.AddComponent<TextMeshProUGUI>();
        errorText.text = "";
        errorText.color = Color.red;
        errorText.fontSize = 18;
        errorText.fontStyle = FontStyles.Bold;
        errorText.alignment = TextAlignmentOptions.Center;
        errorText.enableWordWrapping = true;
        
        // Set the font asset (using the same one from the scene)
        errorText.font = Resources.Load<TMPro.TMP_FontAsset>("Fonts & Materials/Jersey_10 SDF");
        if (errorText.font == null)
        {
            // Fallback to default font
            errorText.font = Resources.Load<TMPro.TMP_FontAsset>("LiberationSans SDF");
        }
        
        // Initially hide the error message
        errorMessageGO.SetActive(false);
        
        // Find the CreateAndJoinRooms script and assign this error text
        CreateAndJoinRooms createAndJoinRooms = FindObjectOfType<CreateAndJoinRooms>();
        if (createAndJoinRooms != null)
        {
            // Use reflection to set the private field
            var field = typeof(CreateAndJoinRooms).GetField("errorMessageText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(createAndJoinRooms, errorText);
            }
            else
            {
                // Try public field
                var publicField = typeof(CreateAndJoinRooms).GetField("errorMessageText");
                if (publicField != null)
                {
                    publicField.SetValue(createAndJoinRooms, errorText);
                }
            }
        }
        
        Debug.Log("Error message UI created successfully!");
    }
    
    public void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.gameObject.SetActive(true);
        }
    }
    
    public void HideError()
    {
        if (errorText != null)
        {
            errorText.text = "";
            errorText.gameObject.SetActive(false);
        }
    }
}
