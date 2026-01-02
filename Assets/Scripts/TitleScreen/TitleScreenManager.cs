using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // Added for TextMeshPro
using System.Collections;

public class TitleScreenManager : MonoBehaviour
{
    public static TitleScreenManager Instance;
    
    [Header("UI References")]
    public TextMeshProUGUI titleText;        // Changed from Text
    public TextMeshProUGUI subtitleText;     // Changed from Text
    public TextMeshProUGUI versionText;      // Changed from Text
    public TextMeshProUGUI creditsText;      // Changed from Text
    
    [Header("Buttons")]
    public Button startButton;
    //public Button settingsButton;
    public Button leaderboardButton;
    public Button creditsButton;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        SetupUI();
        StartTitleAnimation();
    }
    
    void SetupUI()
    {
        // Setup text
        if (titleText != null)
        {
            titleText.text = "MORPH RUNNER";
            titleText.fontSize = 72;
            titleText.color = Color.white;
            titleText.fontStyle = FontStyles.Bold;
            titleText.alignment = TextAlignmentOptions.Center;
        }
        
        if (subtitleText != null)
        {
            subtitleText.text = "Shape • Shift • Survive";
            subtitleText.fontSize = 44;
            subtitleText.color = new Color(0.9f, 0.9f, 0.9f, 1f);
            subtitleText.alignment = TextAlignmentOptions.Center;
        }
        
        if (versionText != null)
        {
            versionText.text = "v3.0.0";
            //versionText.fontSize = 16;
            versionText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            //versionText.alignment = TextAlignmentOptions.BottomRight;
        }
        
        if (creditsText != null)
        {
            creditsText.text = "Team Morph Runner • CSCI 526 • Gold";
            //creditsText.fontSize = 16;
            creditsText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            //creditsText.alignment = TextAlignmentOptions.BottomLeft;
        }
        
        // Setup button listeners
        if (startButton != null) startButton.onClick.AddListener(StartGame);
        //if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        if (leaderboardButton != null) leaderboardButton.onClick.AddListener(OpenLeaderboard);
        if (creditsButton != null) creditsButton.onClick.AddListener(OpenCredits);
    }
    
    void StartTitleAnimation()
    {
        if (titleText != null)
        {
            StartCoroutine(TitleGlowAnimation());
        }
    }
    
    IEnumerator TitleGlowAnimation()
    {
        while (true)
        {
            // Fade out
            float timer = 0f;
            while (timer < 1f)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0.7f, timer);
                titleText.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }
            
            // Fade in
            timer = 0f;
            while (timer < 1f)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(0.7f, 1f, timer);
                titleText.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }
        }
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            OnShapeSelected(ShapeDemoController.ShapeType.Circle);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            OnShapeSelected(ShapeDemoController.ShapeType.Triangle);
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            OnShapeSelected(ShapeDemoController.ShapeType.Square);
        }
    }
    
    public void OnShapeSelected(ShapeDemoController.ShapeType shapeType)
    {
        Debug.Log($"Shape selected: {shapeType}");
    }
    
    public void StartGame()
    {
        StartCoroutine(LoadGameScene());
    }
    
    IEnumerator LoadGameScene()
    {
        if (startButton != null)
        {
            TextMeshProUGUI buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null) buttonText.text = "LOADING...";
        }
        
        yield return new WaitForSeconds(1f);
        
        SceneManager.LoadScene("GameScene"); // Replace with your actual game scene name
    }
    
    // public void OpenSettings()
    // {
    //     Debug.Log("Opening Settings...");
    // }
    
    public void OpenLeaderboard()
    {
        Debug.Log("Opening Leaderboard...");
    }
    
    public void OpenCredits()
    {
        Debug.Log("Opening Credits...");
    }
}