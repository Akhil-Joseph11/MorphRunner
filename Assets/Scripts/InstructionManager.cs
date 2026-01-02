using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InstructionManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject instructionPanel;
    public TMP_Text titleText;
    public TMP_Text instructionsText;
    public TMP_Text footerText;
    
    [Header("Game References")]
    public MonoBehaviour playerController; // Reference to your player movement script
    public MonoBehaviour gameManager; // Reference to pause game logic if needed
    
    [Header("Level Settings")]
    [SerializeField] private int currentLevel = 1; // Set this in inspector for each scene
    
    private bool instructionsActive = true;

    void Start()
    {
        SetupInstructionScreen();
        ShowInstructions();
    }

    void Update()
    {
        if (instructionsActive)
        {
            // Check for input to close instructions
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                HideInstructions();
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitGame();
            }
        }
    }

    void SetupInstructionScreen()
    {
        // Set up the title with current level
        if (titleText != null)
        {
            titleText.text = $"MORPH RUNNER\nShape-Shift to Survive the Endless Run\n \nLevel {currentLevel}";
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.cyan;
            titleText.fontStyle = FontStyles.Bold;
        }

        // Set up the main instructions based on current level
        if (instructionsText != null)
        {
            instructionsText.text = GetInstructionTextForLevel(currentLevel);
            instructionsText.alignment = TextAlignmentOptions.Left;
            instructionsText.color = Color.white;
        }

        // Set up the footer (same for all levels)
        if (footerText != null)
        {
            footerText.text = "\nPress [SPACE] or [ENTER] to Start Game";
            footerText.alignment = TextAlignmentOptions.Center;
            footerText.color = Color.yellow;
        }
    }

    string GetInstructionTextForLevel(int level)
    {
        switch (level)
        {
            case 1:
                return @"Objective
Transform into the correct shape to pass through each obstacle.
Collect shapes to change your form and match the obstacle's gap.

Controls
A / D - Move Left / Right";

            case 2:
                return @"Objective
Transform into the correct shape & color to pass through each obstacle.

Controls
A / D - Move Left / Right
Space Bar - Change Color";

            case 3:
                return @"Objective
Transform into the correct shape to pass through obstacles.
Collect and combine shapes for upcoming gaps.

Controls
A / D – Move Left / Right";

            case 4: // NEW: Level 4 instructions
                return @"Objective
Transform into correct shapes combination and color to pass through obstacles.

Controls
A / D - Move Left / Right
Space Bar - Change Color";

            default:
                return @"Objective
Transform into the correct shape to pass through each obstacle.

Controls
A / D - Move Left / Right";
        }
    }

    // Public method to set level externally if needed
    public void SetLevel(int level)
    {
        currentLevel = level;
        SetupInstructionScreen();
    }

    void ShowInstructions()
    {
        instructionPanel.SetActive(true);
        instructionsActive = true;
        
        // Disable player movement
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        // Pause the game
        Time.timeScale = 0f;
    }

    void HideInstructions()
    {
        instructionPanel.SetActive(false);
        instructionsActive = false;
        
        // Enable player movement
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        
        // Resume the game
        Time.timeScale = 1f;
    }

    void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}