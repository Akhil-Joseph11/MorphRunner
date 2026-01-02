using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public GameObject gameOverPanel;               // Panel for Game Over
    public GameObject finishLinePanel;            // Panel for Level Complete (your FinishLinePanel)
    
    [Header("Button Listeners")]
    public RestartButtonListener restartListener;  // For Game Over restart

    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public bool isLevelComplete = false;

    public float normalSpeed = 3f;
    [HideInInspector] public float currentSpeed;

    private Coroutine slowdownRoutine;

    [HideInInspector] public bool isSpeedingUp = false;
    public float speedUpMultiplier = 2.25f;

    void Start()
    {
        currentSpeed = normalSpeed;
    }

    public void SlowDownObstacles(float slowFactor = 0.5f, float duration = 1.5f)
    {
        if (slowdownRoutine != null)
        {
            StopCoroutine(slowdownRoutine);
        }
        slowdownRoutine = StartCoroutine(SlowDownRoutine(slowFactor, duration));
    }

    private IEnumerator SlowDownRoutine(float slowFactor, float duration)
    {
        currentSpeed = normalSpeed * slowFactor;
        yield return new WaitForSeconds(duration);
        currentSpeed = normalSpeed;
        slowdownRoutine = null;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        // Handle keyboard input when game is paused
        if (isGameOver || isLevelComplete)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
            else if (Input.GetKeyDown(KeyCode.N) && isLevelComplete)
            {
                LoadNextLevel();
            }
            return;
        }

        // if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        // {
        //     if (!isSpeedingUp)
        //     {
        //         isSpeedingUp = true;
        //         currentSpeed = normalSpeed * speedUpMultiplier;
        //     }
        // }
        // else if (isSpeedingUp)
        // {
        //     isSpeedingUp = false;
        //     currentSpeed = normalSpeed;
        // }
    }

    public void GameOver()
{
    if (isGameOver) return;
    
    // Capture the survival time 
    float currentSurvivalTime = 0f;
    if (GameStatsManager.Instance != null)
        currentSurvivalTime = GameStatsManager.Instance.GetSurvivalTime();
    
    isGameOver = true;

    // Write the captured time
    if (GameStatsManager.Instance != null)
        GameStatsManager.Instance.WriteFinalTime(currentSurvivalTime);

    // Log death level for analytics
    FindObjectOfType<ObstacleMismatchLogging>()?.LogDeathLevel();

    if (gameOverPanel) gameOverPanel.SetActive(true);
    if (restartListener) restartListener.EnableListener();
    
    Time.timeScale = 0f;
}

    public void FinishLine()
{
    if (isGameOver || isLevelComplete) return;
    isLevelComplete = true;

    // Write finish time
    if (GameStatsManager.Instance != null)
        GameStatsManager.Instance.WriteFinishTime();

    if (GameStatsManager.Instance != null)
    {
        int healthRemaining = GameStatsManager.Instance.GetCurrentHealth();
        FindObjectOfType<ObstacleMismatchLogging>()?.LogLevelCompletion(healthRemaining);
    }

    // Show Finish Line Panel (your FinishLinePanel)
    if (finishLinePanel) finishLinePanel.SetActive(true);

    // Pause the game
    Time.timeScale = 0f;
}

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f; // Resume game time
        
        string currentSceneName = SceneManager.GetActiveScene().name; 
        Debug.Log("Current scene: " + currentSceneName);
        
        int currentLevelNumber = ExtractLevelNumber(currentSceneName);
        string nextSceneName = "Level" + (currentLevelNumber + 1);
        
        Debug.Log("Trying to load: " + nextSceneName);
        
        // Check if next level exists, otherwise restart current level or go to main menu
        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("No more levels! Going back to Level1");
            // Go back to Level1 when no more levels
            SceneManager.LoadScene("TitleScreen");
        }
    }

    int ExtractLevelNumber(string sceneName)
    {
        for (int i = 0; i < sceneName.Length; i++)
        {
            if (char.IsDigit(sceneName[i]))
            {
                string number = sceneName.Substring(i);
                if (int.TryParse(number, out int level))
                {
                    return level;
                }
            }
        }
        Debug.LogWarning("No number found in scene name. Defaulting to 1.");
        return 1;
    }
}