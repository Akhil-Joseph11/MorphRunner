using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DynamicLevelPanel : MonoBehaviour
{
    [Header("Level Buttons")]
    public GameObject level1Button;
    public GameObject level2Button;
    public GameObject level3Button;
    public GameObject level4Button; 
    
    [Header("Level Button Components")]
    public Image level1Image;
    public Image level2Image;
    public Image level3Image;
    public Image level4Image; 
    
    [Header("Level Texts")]
    public TextMeshProUGUI level1Text;
    public TextMeshProUGUI level2Text;
    public TextMeshProUGUI level3Text;
    public TextMeshProUGUI level4Text; 
    
    [Header("Control Texts (Optional - can be same as Level Texts)")]
    public TextMeshProUGUI level1ControlText;
    public TextMeshProUGUI level2ControlText;
    public TextMeshProUGUI level3ControlText;
    public TextMeshProUGUI level4ControlText; 
    
    [Header("Colors")]
    public Color currentLevelColor = new Color(1f, 0.8f, 0f, 1f);    // Yellow/Orange
    public Color completedLevelColor = new Color(0f, 0.8f, 0f, 1f);  // Green
    public Color lockedLevelColor = new Color(0.4f, 0.4f, 0.4f, 1f); // Dark Gray
    
    [Header("Sizes")]
    public float expandedHeight = 200f;
    public float collapsedHeight = 50f;
    
    [Header("Controls Display")]
    public bool showControlsInMainText = true; // If true, shows controls in main text. If false, uses separate control text components
    public bool showControlsOnLockedLevels = false; // Whether to show controls for locked levels
    
    private int currentLevel = 1;
    private LayoutElement level1Layout;
    private LayoutElement level2Layout;
    private LayoutElement level3Layout;
    private LayoutElement level4Layout; // NEW: Level 4 layout
    
    void Start()
    {
        Debug.Log("DynamicLevelPanel Start() called");
        
        // Get or add layout elements with null checks and debug info
        SetupLayoutElement(level1Button, ref level1Layout, "Level1");
        SetupLayoutElement(level2Button, ref level2Layout, "Level2");
        SetupLayoutElement(level3Button, ref level3Layout, "Level3");
        SetupLayoutElement(level4Button, ref level4Layout, "Level4"); // NEW: Setup Level 4
        
        // GENERALIZED SCENE DETECTION
        currentLevel = GetCurrentLevelFromScene();
        Debug.Log($"Current Level determined from scene: {currentLevel}");
        
        // Set up button click events
        SetupButtonEvents();
        
        // Update the display
        UpdateLevelDisplay();
    }
    
    /// <summary>
    /// Get control instructions for a specific level (extracted from InstructionManager logic)
    /// </summary>
    string GetControlsForLevel(int level)
    {
        switch (level)
        {
            case 1:
                return "A / D - Move Left / Right";
                
            case 2:
                return "A / D - Move Left / Right\nSpace Bar - Change Color";
                
            case 3:
                return "A / D – Move Left / Right";
                
            case 4: // NEW: Level 4 controls
                return "A / D - Move Left / Right\nSpace Bar - Change Color";
                
            default:
                return "A / D - Move Left / Right";
        }
    }
    
    /// <summary>
    /// Format the level text with optional controls
    /// </summary>
    string FormatLevelText(int levelNum, string status, bool includeControls = true)
    {
        string baseText = $"Level {levelNum} ({status})";
        
        if (includeControls && showControlsInMainText)
        {
            string controls = GetControlsForLevel(levelNum);
            return $"{baseText}\n\nControls:\n{controls}";
        }
        
        return baseText;
    }
    
    /// <summary>
    /// Update control text component if it exists and is separate from main text
    /// </summary>
    void UpdateControlText(int levelNum, bool isLocked = false)
    {
        if (showControlsInMainText) return; // Controls are in main text, skip separate control text
        
        TextMeshProUGUI controlText = GetLevelControlText(levelNum);
        if (controlText == null) return;
        
        if (isLocked && !showControlsOnLockedLevels)
        {
            controlText.text = "";
            return;
        }
        
        string controls = GetControlsForLevel(levelNum);
        controlText.text = $"Controls:\n{controls}";
        controlText.color = isLocked ? Color.gray : Color.white;
    }
    
    /// <summary>
    /// Generalized method to determine current level based on active scene
    /// </summary>
    int GetCurrentLevelFromScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Active scene: {sceneName}");
        
        // Map scene names to level numbers - Updated to match your exact scene names
        switch (sceneName)
        {
            case "Level1":
                return 1;
                
            case "Level2":
                return 2;
                
            case "Level3":
                return 3;
                
            case "Level4": // NEW: Level 4 scene mapping
                return 4;
                
            default:
                // Fallback to saved progress if scene name doesn't match
                Debug.LogWarning($"Unknown scene: {sceneName}. Using saved progress.");
                return PlayerPrefs.GetInt("CurrentLevel", 1);
        }
    }
    
    void SetupLayoutElement(GameObject button, ref LayoutElement layout, string name)
    {
        if (button != null)
        {
            layout = button.GetComponent<LayoutElement>();
            if (layout == null) 
            {
                layout = button.AddComponent<LayoutElement>();
                Debug.Log($"Added LayoutElement to {name}");
            }
            else
            {
                Debug.Log($"LayoutElement already exists on {name}");
            }
        }
        else
        {
            Debug.LogWarning($"{name} button is null!");
        }
    }
    
    void SetupButtonEvents()
    {
        if (level1Button != null)
        {
            Button btn1 = level1Button.GetComponent<Button>();
            if (btn1) 
            {
                btn1.onClick.AddListener(() => LoadLevel(1));
                Debug.Log("Level 1 button event setup");
            }
        }
        
        if (level2Button != null)
        {
            Button btn2 = level2Button.GetComponent<Button>();
            if (btn2) 
            {
                btn2.onClick.AddListener(() => LoadLevel(2));
                Debug.Log("Level 2 button event setup");
            }
        }
        
        if (level3Button != null)
        {
            Button btn3 = level3Button.GetComponent<Button>();
            if (btn3) 
            {
                btn3.onClick.AddListener(() => LoadLevel(3));
                Debug.Log("Level 3 button event setup");
            }
        }
        
        // NEW: Level 4 button setup
        if (level4Button != null)
        {
            Button btn4 = level4Button.GetComponent<Button>();
            if (btn4) 
            {
                btn4.onClick.AddListener(() => LoadLevel(4));
                Debug.Log("Level 4 button event setup");
            }
        }
    }
    
    void UpdateLevelDisplay()
    {
        Debug.Log($"UpdateLevelDisplay called with currentLevel: {currentLevel}");
        
        // GENERALIZED LEVEL DISPLAY LOGIC - Updated to handle Level 4
        for (int level = 1; level <= 4; level++) // CHANGED: Now loops to 4
        {
            if (level < currentLevel)
            {
                // Levels before current = COMPLETED
                Debug.Log($"Setting Level {level} as completed");
                SetLevelAsCompleted(level);
            }
            else if (level == currentLevel)
            {
                // Current level = CURRENT
                Debug.Log($"Setting Level {level} as current");
                SetLevelAsCurrent(level);
            }
            else
            {
                // Levels after current = LOCKED
                // Level 4 is always locked for now
                if (level == 4)
                {
                    Debug.Log($"Setting Level {level} as locked (Level 4 always locked)");
                    SetLevelAsLocked(level);
                }
                else if (currentLevel < 3)
                {
                    Debug.Log($"Setting Level {level} as locked");
                    SetLevelAsLocked(level);
                }
                else
                {
                    // At max level (3), don't show higher levels as locked
                    Debug.Log($"Level {level} hidden/disabled (beyond max level)");
                    SetLevelAsHidden(level);
                }
            }
        }
    }
    
    void SetLevelAsCurrent(int levelNum)
    {
        Debug.Log($"SetLevelAsCurrent({levelNum}) called");
        
        GameObject button = GetLevelButton(levelNum);
        Image image = GetLevelImage(levelNum);
        TextMeshProUGUI text = GetLevelText(levelNum);
        LayoutElement layout = GetLevelLayout(levelNum);
        
        // Style as current level
        if (image != null) 
        {
            image.color = currentLevelColor;
            Debug.Log($"Level {levelNum} image color set to current (yellow)");
        }
        
        if (text != null)
        {
            text.text = FormatLevelText(levelNum, "Current");
            text.color = Color.black;
            Debug.Log($"Level {levelNum} text updated");
        }
        
        // Update control text if using separate components
        UpdateControlText(levelNum, false);
        
        // Expand the button
        if (layout != null)
        {
            layout.preferredHeight = expandedHeight;
            layout.flexibleHeight = 0;
            layout.minHeight = expandedHeight;
            Debug.Log($"Level {levelNum} expanded to height: {expandedHeight}");
            
            // Force layout rebuild
            LayoutRebuilder.ForceRebuildLayoutImmediate(button.GetComponent<RectTransform>());
        }
        
        // Enable button
        if (button != null)
        {
            Button btn = button.GetComponent<Button>();
            if (btn != null) 
            {
                btn.interactable = true;
                button.SetActive(true);
                Debug.Log($"Level {levelNum} button enabled");
            }
        }
    }
    
    void SetLevelAsCompleted(int levelNum)
    {
        Debug.Log($"SetLevelAsCompleted({levelNum}) called");
        
        GameObject button = GetLevelButton(levelNum);
        Image image = GetLevelImage(levelNum);
        TextMeshProUGUI text = GetLevelText(levelNum);
        LayoutElement layout = GetLevelLayout(levelNum);
        
        // Style as completed level
        if (image != null)
        {
            image.color = completedLevelColor;
            Debug.Log($"Level {levelNum} image color set to completed (green)");
        }
        
        if (text != null)
        {
            // Don't show controls for completed levels - just simple text
            text.text = FormatLevelText(levelNum, "Complete", false);
            text.color = Color.black;
            Debug.Log($"Level {levelNum} text updated to completed");
        }
        
        // Hide control text for completed levels if using separate components
        TextMeshProUGUI controlText = GetLevelControlText(levelNum);
        if (controlText != null)
        {
            controlText.text = "";
        }
        
        // Collapse the button
        if (layout != null)
        {
            layout.preferredHeight = collapsedHeight;
            layout.flexibleHeight = 0;
            layout.minHeight = collapsedHeight;
            Debug.Log($"Level {levelNum} collapsed to height: {collapsedHeight}");
            
            // Force layout rebuild
            LayoutRebuilder.ForceRebuildLayoutImmediate(button.GetComponent<RectTransform>());
        }
        
        // Enable button (can replay completed levels)
        if (button != null)
        {
            Button btn = button.GetComponent<Button>();
            if (btn != null) 
            {
                btn.interactable = true;
                button.SetActive(true);
                Debug.Log($"Level {levelNum} button enabled for replay");
            }
        }
    }
    
    void SetLevelAsLocked(int levelNum)
    {
        Debug.Log($"SetLevelAsLocked({levelNum}) called");
        
        GameObject button = GetLevelButton(levelNum);
        Image image = GetLevelImage(levelNum);
        TextMeshProUGUI text = GetLevelText(levelNum);
        LayoutElement layout = GetLevelLayout(levelNum);
        
        // Style as locked level
        if (image != null)
        {
            image.color = lockedLevelColor;
            Debug.Log($"Level {levelNum} image color set to locked (gray)");
        }
        
        if (text != null)
        {
            text.text = FormatLevelText(levelNum, "Locked", showControlsOnLockedLevels);
            text.color = Color.gray;
            Debug.Log($"Level {levelNum} text updated to locked");
        }
        
        // Update control text if using separate components
        UpdateControlText(levelNum, true);
        
        // Collapse the button
        if (layout != null)
        {
            layout.preferredHeight = collapsedHeight;
            layout.flexibleHeight = 0;
            layout.minHeight = collapsedHeight;
            Debug.Log($"Level {levelNum} collapsed to height: {collapsedHeight}");
            
            // Force layout rebuild
            LayoutRebuilder.ForceRebuildLayoutImmediate(button.GetComponent<RectTransform>());
        }
        
        // Disable button
        if (button != null)
        {
            Button btn = button.GetComponent<Button>();
            if (btn != null) 
            {
                btn.interactable = false;
                button.SetActive(true);
                Debug.Log($"Level {levelNum} button disabled");
            }
        }
    }
    
    void SetLevelAsHidden(int levelNum)
    {
        Debug.Log($"SetLevelAsHidden({levelNum}) called");
        
        GameObject button = GetLevelButton(levelNum);
        
        // Simply hide levels beyond the max
        if (button != null)
        {
            button.SetActive(false);
            Debug.Log($"Level {levelNum} button hidden");
        }
    }
    
    void LoadLevel(int levelNum)
    {
        Debug.Log($"LoadLevel({levelNum}) called, currentLevel: {currentLevel}");
        
        // Allow loading current level or any completed level
        int savedProgress = PlayerPrefs.GetInt("CurrentLevel", 1);
        
        // Level 4 is always locked for now
        if (levelNum == 4)
        {
            Debug.Log("Cannot load Level 4 - it's always locked!");
            return;
        }
        
        if (levelNum <= savedProgress)
        {
            switch (levelNum)
            {
                case 1:
                    SceneManager.LoadScene("Level1");
                    break;
                case 2:
                    SceneManager.LoadScene("Level2");
                    break;
                case 3:
                    SceneManager.LoadScene("Level3");
                    break;
                case 4: // NEW: Level 4 scene loading (currently disabled)
                    // SceneManager.LoadScene("Level4");
                    Debug.Log("Level 4 not yet available!");
                    break;
            }
        }
        else
        {
            Debug.Log($"Cannot load Level {levelNum} - it's locked!");
        }
    }
    
    public void CompleteCurrentLevel()
    {
        Debug.Log($"CompleteCurrentLevel called, current: {currentLevel}");
        
        // Level 4 is the max for now, but don't allow progression to it yet
        if (currentLevel < 3)
        {
            int newLevel = currentLevel + 1;
            PlayerPrefs.SetInt("CurrentLevel", newLevel);
            PlayerPrefs.Save();
            Debug.Log($"Level completed! New current level: {newLevel}");
            
            // Don't update display here since we're likely changing scenes
        }
        else if (currentLevel == 3)
        {
            Debug.Log("Level 3 completed, but Level 4 is not yet available!");
        }
    }
    
    // Helper methods to get components - UPDATED to include Level 4
    GameObject GetLevelButton(int levelNum)
    {
        switch (levelNum)
        {
            case 1: return level1Button;
            case 2: return level2Button;
            case 3: return level3Button;
            case 4: return level4Button; // NEW
            default: return null;
        }
    }
    
    Image GetLevelImage(int levelNum)
    {
        switch (levelNum)
        {
            case 1: return level1Image;
            case 2: return level2Image;
            case 3: return level3Image;
            case 4: return level4Image; // NEW
            default: return null;
        }
    }
    
    TextMeshProUGUI GetLevelText(int levelNum)
    {
        switch (levelNum)
        {
            case 1: return level1Text;
            case 2: return level2Text;
            case 3: return level3Text;
            case 4: return level4Text; // NEW
            default: return null;
        }
    }
    
    TextMeshProUGUI GetLevelControlText(int levelNum)
    {
        switch (levelNum)
        {
            case 1: return level1ControlText;
            case 2: return level2ControlText;
            case 3: return level3ControlText;
            case 4: return level4ControlText; // NEW
            default: return null;
        }
    }
    
    LayoutElement GetLevelLayout(int levelNum)
    {
        switch (levelNum)
        {
            case 1: return level1Layout;
            case 2: return level2Layout;
            case 3: return level3Layout;
            case 4: return level4Layout; // NEW
            default: return null;
        }
    }
    
    [ContextMenu("Test Complete Level")]
    public void TestCompleteLevel()
    {
        CompleteCurrentLevel();
    }
    
    [ContextMenu("Reset Progress")]
    public void ResetProgress()
    {
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.Save();
        
        // Refresh display
        currentLevel = GetCurrentLevelFromScene();
        UpdateLevelDisplay();
        Debug.Log("Progress reset to Level 1");
    }
}