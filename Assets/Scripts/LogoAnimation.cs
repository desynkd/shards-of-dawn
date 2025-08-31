using UnityEngine;

public class SimpleLogoPopup : MonoBehaviour
{
    [Header("Animation Settings")]
    public Animator logoAnimator;
    
    [Header("Popup Behavior")]
    [Tooltip("If true, popup shows only once per game session")]
    public bool showOncePerSession = true;
    
    [Tooltip("If true, popup can be triggered multiple times")]
    public bool allowMultipleTriggers = false;
    
    [Header("Debug Options")]
    public bool autoTriggerOnStart = true;
    public float delayBeforePopup = 2f;
    
    private bool hasShownPopup = false;
    
    void Start()
    {
        // Get animator if not assigned
        if (logoAnimator == null)
            logoAnimator = GetComponent<Animator>();
            
        // Debug: Check if animator exists
        if (logoAnimator == null)
        {
            Debug.LogError("No Animator found! Please assign the Animator component.");
            return;
        }
        
        Debug.Log("=== LOGO POPUP INITIALIZATION ===");
        
        // Debug: Check parameters
        CheckAnimatorParameters();
        
        // Initialize animator - ALWAYS start with HasShown = false
        logoAnimator.SetBool("HasShown", false);
        Debug.Log("Animator initialized - HasShown set to false");
        
        // Show current state
        ShowCurrentAnimatorState();
        
        // Auto-trigger for testing
        if (autoTriggerOnStart)
        {
            Debug.Log("Auto-trigger enabled - will trigger popup in " + delayBeforePopup + " seconds");
            Invoke("TriggerPopup", delayBeforePopup);
        }
    }
    
    public void TriggerPopup()
    {
        if (logoAnimator == null)
        {
            Debug.LogError("Animator is null - cannot trigger popup");
            return;
        }
        
        Debug.Log("=== TRIGGER POPUP CALLED ===");
        Debug.Log("hasShownPopup: " + hasShownPopup);
        Debug.Log("showOncePerSession: " + showOncePerSession);
        Debug.Log("allowMultipleTriggers: " + allowMultipleTriggers);
        
        // Check if we should show the popup
        bool shouldShow = false;
        
        if (allowMultipleTriggers)
        {
            // Always allow if multiple triggers are enabled
            shouldShow = true;
            Debug.Log("Multiple triggers allowed - showing popup");
        }
        else if (showOncePerSession && !hasShownPopup)
        {
            // Show once per session
            shouldShow = true;
            hasShownPopup = true;
            Debug.Log("First time showing popup this session");
        }
        else if (!showOncePerSession)
        {
            // Always show if not restricted to once per session
            shouldShow = true;
            Debug.Log("No session restriction - showing popup");
        }
        else
        {
            Debug.Log("Popup already shown this session - skipping");
            return;
        }
        
        if (shouldShow)
        {
            StartCoroutine(PlayPopupAnimation());
        }
    }
    
    private System.Collections.IEnumerator PlayPopupAnimation()
    {
        // Debug: Show current parameter values BEFORE
        Debug.Log("BEFORE - HasShown: " + logoAnimator.GetBool("HasShown"));
        ShowCurrentAnimatorState();
        
        // CRITICAL: Reset HasShown to false before triggering
        logoAnimator.SetBool("HasShown", false);
        
        // Wait a frame to ensure parameter is set
        yield return null;
        
        // Now trigger the popup
        logoAnimator.SetTrigger("TriggerPopup");
        Debug.Log("TriggerPopup sent to animator");
        
        // Wait a bit for the transition to start
        yield return new WaitForSeconds(0.1f);
        
        // Debug: Show state after trigger
        Debug.Log("=== STATE CHECK AFTER TRIGGER ===");
        ShowCurrentAnimatorState();
        
        // Optional: Set HasShown to true after animation starts
        // This prevents the animation from looping if you have transitions back
        yield return new WaitForSeconds(0.5f);
        logoAnimator.SetBool("HasShown", true);
        Debug.Log("HasShown set to true after animation started");
    }
    
    private void ShowCurrentAnimatorState()
    {
        if (logoAnimator == null) return;
        
        AnimatorStateInfo currentState = logoAnimator.GetCurrentAnimatorStateInfo(0);
        Debug.Log("Current State Info:");
        Debug.Log("- Is in 'Idle': " + currentState.IsName("Idle"));
        Debug.Log("- Is in 'Entry': " + currentState.IsName("Entry"));
        Debug.Log("- Is in 'logoPopUp': " + currentState.IsName("logoPopUp"));
        Debug.Log("- State hash: " + currentState.fullPathHash);
        Debug.Log("- Normalized time: " + currentState.normalizedTime);
        
        // Also show parameter values
        Debug.Log("- HasShown parameter: " + logoAnimator.GetBool("HasShown"));
    }
    
    private void CheckAnimatorParameters()
    {
        Debug.Log("=== CHECKING ANIMATOR PARAMETERS ===");
        
        if (logoAnimator.parameters.Length == 0)
        {
            Debug.LogError("No parameters found in Animator Controller!");
            return;
        }
        
        bool hasShownExists = false;
        bool triggerPopupExists = false;
        
        for (int i = 0; i < logoAnimator.parameters.Length; i++)
        {
            var param = logoAnimator.parameters[i];
            Debug.Log("Parameter found: '" + param.name + "' (Type: " + param.type + ")");
            
            if (param.name == "HasShown") hasShownExists = true;
            if (param.name == "TriggerPopup") triggerPopupExists = true;
        }
        
        if (!hasShownExists)
        {
            Debug.LogError("❌ Parameter 'HasShown' (Bool) is MISSING from Animator Controller!");
            Debug.LogError("   → Go to Animator window → Parameters tab → Add Bool named 'HasShown'");
        }
        else
        {
            Debug.Log("✅ Parameter 'HasShown' found");
        }
            
        if (!triggerPopupExists)
        {
            Debug.LogError("❌ Parameter 'TriggerPopup' (Trigger) is MISSING from Animator Controller!");
            Debug.LogError("   → Go to Animator window → Parameters tab → Add Trigger named 'TriggerPopup'");
        }
        else
        {
            Debug.Log("✅ Parameter 'TriggerPopup' found");
        }
    }
    
    // Public method you can call from Inspector or other scripts
    [ContextMenu("Manual Trigger")]
    public void ManualTrigger()
    {
        Debug.Log("Manual trigger button pressed!");
        TriggerPopup();
    }
    
    // Reset function for testing
    [ContextMenu("Reset Popup")]
    public void ResetPopup()
    {
        hasShownPopup = false;
        if (logoAnimator != null)
        {
            logoAnimator.SetBool("HasShown", false);
            logoAnimator.Play("Entry", 0, 0);
        }
        Debug.Log("Popup reset - can trigger again");
    }
    
    // Force show popup (ignores session restrictions)
    [ContextMenu("Force Show Popup")]
    public void ForceShowPopup()
    {
        Debug.Log("Force showing popup!");
        if (logoAnimator != null)
        {
            StartCoroutine(PlayPopupAnimation());
        }
    }
    
    // Animation event callback
    public void OnPopupAnimationComplete()
    {
        Debug.Log("Popup animation completed!");
        // Animation is done, keep HasShown as true to prevent looping
    }
}