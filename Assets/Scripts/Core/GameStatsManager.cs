using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance;

    [Header("UI")]
    public Slider hpSlider;                    // Health bar
    public TextMeshProUGUI hpText;             //
    public TextMeshProUGUI finalTimeText;      // final time text in game-over panel
    public TextMeshProUGUI finishTimeText;     // finish time text in finish-line panel
    public TextMeshProUGUI matchStatusText;

    [Header("Config")]
    public int maxHP   = 100;
    public float hpTickInterval = 0.85f;        // for every 0.85s, -1 HP

    [Header("Match Status Colors")]
    public Color positiveColor = Color.green;  // Color for perfect matches/positive points
    public Color negativeColor = Color.red;    // Color for wrong matches/negative points
    public Color defaultColor = Color.white;   // Default color

    int currentHP;
    float hpTickTimer;
    float survivalTime;                        // final time

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }
    void OnDestroy() 
    { 
        if (Instance == this) Instance = null; 
    }

    void Start()
    {
        currentHP = maxHP;
        UpdateHPUI();
        
        // Store the default color if not set
        if (matchStatusText != null && defaultColor == Color.white)
        {
            defaultColor = matchStatusText.color;
        }
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.isGameOver || GameManager.Instance.isLevelComplete) return;

        survivalTime += Time.deltaTime;

        hpTickTimer += Time.deltaTime;
        if (hpTickTimer >= hpTickInterval)
        {
            int ticks = Mathf.FloorToInt(hpTickTimer / hpTickInterval);
            ReduceHP(ticks);
            hpTickTimer -= ticks * hpTickInterval;
        }
    }

    public void AddHP(int amount)
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.isGameOver || GameManager.Instance.isLevelComplete) return;

        currentHP = Mathf.Min(currentHP + amount, maxHP);
        UpdateHPUI();
    }

    public void ReduceHP(int amount)
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.isGameOver || GameManager.Instance.isLevelComplete) return;

        currentHP -= amount;
        UpdateHPUI();

        if (currentHP <= 0)
            TriggerGameOver();
    }

    void UpdateHPUI()
    {
        if (hpSlider) hpSlider.value = (float)currentHP / maxHP;
    }

    public int GetCurrentHealth()
    {
        return currentHP;
    }

    public float GetHealthPercentage()
    {
        return (float)currentHP / maxHP;
    }

    void TriggerGameOver()
    {
        GameManager.Instance.GameOver();
        if (finalTimeText)
            finalTimeText.text = $"Survival Time: {survivalTime:F1}s";
    }

    public void WriteFinalTime(float timeToDisplay = -1f)
    {
        float displayTime = timeToDisplay >= 0f ? timeToDisplay : survivalTime;
        if (finalTimeText)
            finalTimeText.text = $"Survival Time: {displayTime:F1}s";
    }

    public void WriteFinishTime()
    {
        if (finishTimeText)
            finishTimeText.text = $"Finish Time: {survivalTime:F1}s";
    }
    
    Coroutine statusRoutine;

    // Original method for neutral status messages
    public void ShowMatchStatus(string msg, float sec = 1f)
    {
        ShowMatchStatus(msg, defaultColor, sec);
    }

    // New overloaded method with color parameter
    public void ShowMatchStatus(string msg, Color color, float sec = 1f)
    {
        if (matchStatusText == null) return;

        if (statusRoutine != null) StopCoroutine(statusRoutine);
        statusRoutine = StartCoroutine(StatusRoutine(msg, color, sec));
    }

    // Convenience methods for common scenarios
    public void ShowPositiveMatchStatus(string msg, float sec = 1f)
    {
        ShowMatchStatus(msg, positiveColor, sec);
    }

    public void ShowNegativeMatchStatus(string msg, float sec = 1f)
    {
        ShowMatchStatus(msg, negativeColor, sec);
    }

    // Updated coroutine to handle color
    IEnumerator StatusRoutine(string msg, Color color, float sec)
    {
        matchStatusText.text = msg;
        matchStatusText.color = color;
        matchStatusText.enabled = true;
        yield return new WaitForSeconds(sec);
        matchStatusText.enabled = false;
        // Reset color to default
        matchStatusText.color = defaultColor;
    }

    public float GetSurvivalTime() => survivalTime;
}