using UnityEngine;
using TMPro;
using System.Collections;

public class PopupManager : MonoBehaviour
{
    [Header("Manual UI References - Drag from Hierarchy")]
    public GameObject popupBackground;    // Drag "BackgroundPanel" here
    public TextMeshProUGUI titleText;     // Drag "Title" here
    public TextMeshProUGUI messageText;   // Drag "Message" here
    
    [Header("Settings")]
    public float displayDuration = 1.5f;
    public bool pauseGameWhileShowing = true; // Changed to true by default
    
    private static PopupManager instance;
    private Coroutine hideCoroutine;
    private float previousTimeScale; // Store the previous time scale
    
    // Flags to track if popups have been shown before
    private static bool hasShownShapeMismatch = false;
    private static bool hasShownColorMismatch = false;
    private static bool hasShownShapeAndColorMismatch = false;
    
    void Awake()
    {
        Debug.Log("=== PopupManager Awake() STARTING ===");
        
        // Singleton setup
        if (instance == null)
        {
            Debug.Log("Setting PopupManager instance...");
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Make sure popup is hidden initially
            if (popupBackground != null)
            {
                popupBackground.SetActive(false);
                Debug.Log("Popup background set to inactive");
            }
            else
            {
                Debug.LogError("popupBackground is NULL in Awake!");
            }
            
            Debug.Log("PopupManager initialized successfully");
        }
        else
        {
            Debug.Log("Duplicate PopupManager found, destroying...");
            Destroy(gameObject);
        }
        
        Debug.Log("=== PopupManager Awake() COMPLETED ===");
    }
    
    void Start()
    {
        Debug.Log("=== PopupManager Start() called - Script is working! ===");
    }
    
    /// <summary>
    /// Show popup with custom title and message
    /// </summary>
    public static void ShowPopup(string title, string message)
    {
        Debug.Log($"ShowPopup called with: {title} - {message}");
        
        if (instance != null)
        {
            Debug.Log("Instance found, calling ShowPopupInternal...");
            instance.ShowPopupInternal(title, message);
        }
        else
        {
            Debug.LogError("PopupManager instance is NULL!");
            
            // Try to find it manually
            PopupManager found = FindObjectOfType<PopupManager>();
            if (found != null)
            {
                Debug.Log("Found PopupManager with FindObjectOfType, using it...");
                instance = found;
                instance.ShowPopupInternal(title, message);
            }
            else
            {
                Debug.LogError("No PopupManager found anywhere in the scene!");
            }
        }
    }
    
    /// <summary>
    /// Show shape mismatch popup (only first time)
    /// </summary>
    public static void ShowShapeMismatch()
    {
        if (!hasShownShapeMismatch)
        {
            hasShownShapeMismatch = true;
            Debug.Log("First time showing Shape Mismatch popup");
            ShowPopup("Shape Mismatch", "Transform to match the obstacle shape!");
        }
        else
        {
            Debug.Log("Shape Mismatch popup already shown before - skipping");
        }
    }
    
    /// <summary>
    /// Show color mismatch popup (only first time)
    /// </summary>
    public static void ShowColorMismatch()
    {
        if (!hasShownColorMismatch)
        {
            hasShownColorMismatch = true;
            Debug.Log("First time showing Color Mismatch popup");
            ShowPopup("Color Mismatch", "Use SPACEBAR to change color!");
        }
        else
        {
            Debug.Log("Color Mismatch popup already shown before - skipping");
        }
    }
    
    /// <summary>
    /// Show both shape and color mismatch popup (only first time)
    /// </summary>
    public static void ShowShapeAndColorMismatch()
    {
        if (!hasShownShapeAndColorMismatch)
        {
            hasShownShapeAndColorMismatch = true;
            Debug.Log("First time showing Shape & Color Mismatch popup");
            ShowPopup("Shape & Color Mismatch", "Correct shape and use SPACEBAR\nto change color!");
        }
        else
        {
            Debug.Log("Shape & Color Mismatch popup already shown before - skipping");
        }
    }
    
    void ShowPopupInternal(string title, string message)
    {
        Debug.Log($"Showing popup: {title} - {message}");
        
        // Set text content
        if (titleText != null)
        {
            titleText.text = title;
        }
        else
        {
            Debug.LogWarning("Title text is null!");
        }
        
        if (messageText != null)
        {
            messageText.text = message;
        }
        else
        {
            Debug.LogWarning("Message text is null!");
        }
        
        // Show popup
        if (popupBackground != null)
        {
            popupBackground.SetActive(true);
            Debug.Log("Popup background activated");
        }
        else
        {
            Debug.LogError("Popup background is null!");
        }
        
        // Always pause game when showing popup - FORCED PAUSE
        previousTimeScale = Time.timeScale; // Store current time scale
        Time.timeScale = 0f;
        Debug.Log($"FORCED PAUSE - Previous timeScale: {previousTimeScale}, Current: {Time.timeScale}");
        
        // Auto-hide after duration
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }
    
    IEnumerator HideAfterDelay()
    {
        Debug.Log($"Waiting {displayDuration} seconds before hiding popup...");
        
        // Always use real time when paused - FORCED
        yield return new WaitForSecondsRealtime(displayDuration);
        
        HidePopup();
    }
    
    public static void HidePopup()
    {
        if (instance != null)
        {
            instance.HidePopupInternal();
        }
    }
    
    void HidePopupInternal()
    {
        // Hide popup
        if (popupBackground != null)
        {
            popupBackground.SetActive(false);
            Debug.Log("Popup background deactivated");
        }
        
        // Resume game to previous time scale - FORCED
        Time.timeScale = previousTimeScale; // Restore previous time scale
        Debug.Log($"FORCED RESUME - timeScale restored to: {Time.timeScale}");
        
        Debug.Log("Popup hidden successfully");
    }
    
    // Manual close with input - works even when game is paused
    void Update()
    {
        if (popupBackground != null && popupBackground.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("Manual popup close triggered by input");
                HidePopup();
            }
        }
    }
    
    // Force hide popup (useful for debugging or emergency situations)
    public static void ForceHidePopup()
    {
        if (instance != null)
        {
            if (instance.hideCoroutine != null)
            {
                instance.StopCoroutine(instance.hideCoroutine);
            }
            instance.HidePopupInternal();
        }
    }
    
    // Test methods - Right-click component to test
    [ContextMenu("Test Shape Mismatch")]
    public void TestShapeMismatch()
    {
        Debug.Log("Manual test: TestShapeMismatch called");
        ShowShapeMismatch();
    }
    
    [ContextMenu("Test Color Mismatch")]
    public void TestColorMismatch()
    {
        Debug.Log("Manual test: TestColorMismatch called");
        ShowColorMismatch();
    }
    
    [ContextMenu("Test Shape & Color Mismatch")]
    public void TestShapeAndColorMismatch()
    {
        Debug.Log("Manual test: TestShapeAndColorMismatch called");
        ShowShapeAndColorMismatch();
    }
    
    [ContextMenu("Check References")]
    public void CheckReferences()
    {
        Debug.Log("=== CHECKING REFERENCES ===");
        Debug.Log($"popupBackground: {popupBackground}");
        Debug.Log($"titleText: {titleText}");
        Debug.Log($"messageText: {messageText}");
        Debug.Log($"Instance: {instance}");
        Debug.Log($"This component: {this}");
        Debug.Log($"GameObject active: {gameObject.activeInHierarchy}");
        Debug.Log($"Component enabled: {enabled}");
        Debug.Log($"Current timeScale: {Time.timeScale}");
        Debug.Log($"pauseGameWhileShowing: {pauseGameWhileShowing}");
    }
    
    [ContextMenu("Force Show Test Popup")]
    public void ForceShowTestPopup()
    {
        Debug.Log("Force showing test popup directly...");
        ShowPopupInternal("FORCE TEST", "This popup was forced to show!");
    }
    
    [ContextMenu("Force Hide Popup")]
    public void ForceHidePopupMenu()
    {
        Debug.Log("Force hiding popup...");
        ForceHidePopup();
    }
    
    [ContextMenu("Debug Time Scale")]
    public void DebugTimeScale()
    {
        Debug.Log($"=== TIME SCALE DEBUG ===");
        Debug.Log($"Current Time.timeScale: {Time.timeScale}");
        Debug.Log($"Previous Time.timeScale: {previousTimeScale}");
        Debug.Log($"Popup is active: {popupBackground != null && popupBackground.activeSelf}");
        Debug.Log($"pauseGameWhileShowing: {pauseGameWhileShowing}");
    }
    
    // Methods to control first-time popup behavior
    
    /// <summary>
    /// Reset all popup flags - useful for testing or new game
    /// </summary>
    public static void ResetAllPopupFlags()
    {
        hasShownShapeMismatch = false;
        hasShownColorMismatch = false;
        hasShownShapeAndColorMismatch = false;
        Debug.Log("All popup flags reset - popups will show again on first occurrence");
    }
    
    /// <summary>
    /// Check if a specific popup has been shown before
    /// </summary>
    public static bool HasShownPopup(string popupType)
    {
        switch (popupType.ToLower())
        {
            case "shape":
                return hasShownShapeMismatch;
            case "color":
                return hasShownColorMismatch;
            case "both":
            case "shapeandcolor":
                return hasShownShapeAndColorMismatch;
            default:
                return false;
        }
    }
    
    /// <summary>
    /// Force show a popup even if it's been shown before (for testing)
    /// </summary>
    public static void ForceShowShapeMismatch()
    {
        Debug.Log("Force showing Shape Mismatch popup (bypassing first-time check)");
        ShowPopup("Shape Mismatch", "Transform to match the obstacle shape!");
    }
    
    public static void ForceShowColorMismatch()
    {
        Debug.Log("Force showing Color Mismatch popup (bypassing first-time check)");
        ShowPopup("Color Mismatch", "Use SPACEBAR to change color!");
    }
    
    public static void ForceShowShapeAndColorMismatch()
    {
        Debug.Log("Force showing Shape & Color Mismatch popup (bypassing first-time check)");
        ShowPopup("Shape & Color Mismatch", "Correct shape and use SPACEBAR\nto change color!");
    }
    
    [ContextMenu("Reset Popup Flags")]
    public void ResetPopupFlagsMenu()
    {
        ResetAllPopupFlags();
    }
    
    [ContextMenu("Check Popup Status")]
    public void CheckPopupStatus()
    {
        Debug.Log("=== POPUP STATUS ===");
        Debug.Log($"Shape Mismatch shown: {hasShownShapeMismatch}");
        Debug.Log($"Color Mismatch shown: {hasShownColorMismatch}");
        Debug.Log($"Shape & Color Mismatch shown: {hasShownShapeAndColorMismatch}");
    }
}