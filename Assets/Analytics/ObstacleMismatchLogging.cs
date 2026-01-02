using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Text;

public class ObstacleMismatchLogging : MonoBehaviour
{
    private const string firebaseURL = "https://morphrunneranalytics3107-default-rtdb.firebaseio.com/";
    private string userId;

    private int sessionMatchCount = 0;
    private int sessionMismatchCount = 0;
    private float sessionHealth = 0f;


    void Awake()
    {
        if (PlayerPrefs.HasKey("user_id"))
        {
            userId = PlayerPrefs.GetString("user_id");
        }
        else
        {
            userId = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("user_id", userId);
        }

        DontDestroyOnLoad(this.gameObject); 
        Debug.Log("[Analytics] Initialized with userId: " + userId);
    }

 
    public void LogDeathLevel()
    {
        string levelName = SceneManager.GetActiveScene().name;
        string url = $"{firebaseURL}users/{userId}/{levelName}_death_times.json";
        StartCoroutine(GetAndIncrement(url));
    }

    public void LogCorrectMatch()
    {
        string levelName = SceneManager.GetActiveScene().name;
        string url = $"{firebaseURL}users/{userId}/match_stats/{levelName}/obstacle_match_count.json";
        StartCoroutine(GetAndIncrement(url));
        sessionMatchCount++;

    }

    public void LogMismatch(float yPosition)
    {
        string levelName = SceneManager.GetActiveScene().name;
        string countUrl = $"{firebaseURL}users/{userId}/match_stats/{levelName}/obstacle_mismatch_count.json";
        string yPosListUrl = $"{firebaseURL}users/{userId}/match_stats/{levelName}/obstacle_mismatch_positions.json";

        StartCoroutine(GetAndIncrement(countUrl));              
        StartCoroutine(AppendFloatToArray(yPosListUrl, yPosition));
        sessionMismatchCount++;

        Debug.Log($"[Analytics] Mismatch triggered. y={yPosition}, sessionMismatchCount={sessionMismatchCount}");

    }


    public void LogLevelCompletion(float healthRemaining)
    {
        string levelName = SceneManager.GetActiveScene().name;
        string completionCountUrl = $"{firebaseURL}users/{userId}/completion_stats/{levelName}/completion_count.json";
        
        sessionHealth = healthRemaining;
        StartCoroutine(GetAndIncrement(completionCountUrl));
        
        StartCoroutine(CopyMatchStatsToCompletionStats(levelName));

        Debug.Log($"[Analytics] Final counts before write → match: {sessionMatchCount}, mismatch: {sessionMismatchCount}");

    }

    public void LogLevelCompletionByName(string levelName, float healthRemaining)
    {
        string completionCountUrl = $"{firebaseURL}users/{userId}/completion_stats/{levelName}/completion_count.json";
        
        sessionHealth = healthRemaining;
        StartCoroutine(GetAndIncrement(completionCountUrl));

        StartCoroutine(CopyMatchStatsToCompletionStats(levelName));
        
        Debug.Log($"[Analytics] Level {levelName} completed with {healthRemaining} health remaining");
    }

    public void LogHealthProgression(float level1Health, float level2Health, float level3Health, float level4Health)
    {
        string sessionId = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string progressionUrl = $"{firebaseURL}users/{userId}/health_progression/{sessionId}.json";
        
        
        string healthProgressionData = $"{{\"level1\":{level1Health:F2},\"level2\":{level2Health:F2},\"level3\":{level3Health:F2},\"level4\":{level4Health:F2}}}";
        
        StartCoroutine(SendJsonToFirebase(progressionUrl, healthProgressionData));
        
        Debug.Log($"[Analytics] Health progression logged: L1:{level1Health}, L2:{level2Health}, L3:{level3Health}, L4:{level4Health}");
    }

    // Logs the latest health for each level 
    public void LogCurrentLevelHealth(float healthRemaining)
    {
        string levelName = SceneManager.GetActiveScene().name;
        string latestHealthUrl = $"{firebaseURL}users/{userId}/current_level_health/{levelName}.json";
        
        StartCoroutine(SendFloatToFirebase(latestHealthUrl, healthRemaining));
        
        Debug.Log($"[Analytics] Current health for {levelName}: {healthRemaining}");
    }

    public void LogAllLevelsHealth(float[] healthValues, string[] levelNames = null)
    {
        if (healthValues.Length != 4)
        {
            Debug.LogError("[Analytics] Health values array must contain exactly 4 values for 4 levels");
            return;
        }

        string[] defaultLevelNames = {"Level1", "Level2", "Level3", "Level4"};
        string[] levelsToUse = levelNames ?? defaultLevelNames;

        for (int i = 0; i < 4; i++)
        {
            string levelName = levelsToUse[i];
            string healthUrl = $"{firebaseURL}users/{userId}/completion_stats/{levelName}/health_remaining_values.json";
            string countUrl = $"{firebaseURL}users/{userId}/completion_stats/{levelName}/completion_count.json";
            
            StartCoroutine(GetAndIncrement(countUrl));
            StartCoroutine(AppendFloatToArray(healthUrl, healthValues[i]));
        }
        
        Debug.Log($"[Analytics] Logged health for all 4 levels: {string.Join(", ", healthValues)}");
    }

    private IEnumerator SendStringToFirebase(string url, string data)
    {
        string jsonData = $"\"{data}\""; 
        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log($"[Analytics] Logged string: {data} to {url}");
        else
            Debug.LogError($"[Analytics] Failed to log string: {request.error}");
    }

    private IEnumerator SendFloatToFirebase(string url, float value)
    {
        string jsonData = value.ToString("F2");
        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log($"[Analytics] Logged float: {value} to {url}");
        else
            Debug.LogError($"[Analytics] Failed to log float: {request.error}");
    }
    private IEnumerator SendJsonToFirebase(string url, string jsonData)
    {
        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log($"[Analytics] Logged JSON data to {url}");
        else
            Debug.LogError($"[Analytics] Failed to log JSON: {request.error}");
    }

    private IEnumerator GetAndIncrement(string url)
    {
        UnityWebRequest getRequest = UnityWebRequest.Get(url);
        yield return getRequest.SendWebRequest();

        int current = 0;
        if (getRequest.result == UnityWebRequest.Result.Success && !string.IsNullOrEmpty(getRequest.downloadHandler.text))
        {
            int.TryParse(getRequest.downloadHandler.text, out current);
        }

        int updated = current + 1;
        string jsonData = updated.ToString();

        UnityWebRequest putRequest = UnityWebRequest.Put(url, jsonData);
        putRequest.SetRequestHeader("Content-Type", "application/json");
        yield return putRequest.SendWebRequest();

        if (putRequest.result == UnityWebRequest.Result.Success)
            Debug.Log($"[Analytics] Updated count at {url} to {updated}");
        else
            Debug.LogError($"[Analytics] Failed to update count: {putRequest.error}");
    }

    // Appends a float (e.g., y-position) to a Firebase array using POST.
    private IEnumerator AppendFloatToArray(string url, float value)
    {
        string jsonData = value.ToString("F2");

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            Debug.Log($"[Analytics] Appended value {value} to {url}");
        else
            Debug.LogError($"[Analytics] Failed to append value: {request.error}");
    }

    private IEnumerator CopyMatchStatsToCompletionStats(string levelName)
    {
    string targetMatchUrl = $"{firebaseURL}users/{userId}/completion_stats/{levelName}/obstacle_match_count.json";
    string targetMismatchUrl = $"{firebaseURL}users/{userId}/completion_stats/{levelName}/obstacle_mismatch_count.json";
    string scoreUrl = $"{firebaseURL}users/{userId}/completion_stats/{levelName}/score.json";
    string healthUrl = $"{firebaseURL}users/{userId}/completion_stats/{levelName}/health_remaining_values.json";

    // Calculate score
    int score = (sessionMatchCount * 10) - (sessionMismatchCount * 20);

    // Upload match count
    UnityWebRequest putMatch = UnityWebRequest.Put(targetMatchUrl, sessionMatchCount.ToString());
    putMatch.SetRequestHeader("Content-Type", "application/json");
    yield return putMatch.SendWebRequest();

    // Upload mismatch count
    UnityWebRequest putMismatch = UnityWebRequest.Put(targetMismatchUrl, sessionMismatchCount.ToString());
    putMismatch.SetRequestHeader("Content-Type", "application/json");
    yield return putMismatch.SendWebRequest();

    // Upload score
    UnityWebRequest putScore = UnityWebRequest.Put(scoreUrl, score.ToString());
    putScore.SetRequestHeader("Content-Type", "application/json");
    yield return putScore.SendWebRequest();

    // Upload health
    UnityWebRequest putHealth = UnityWebRequest.Put(healthUrl, sessionHealth.ToString("F2"));
    putHealth.SetRequestHeader("Content-Type", "application/json");
    yield return putHealth.SendWebRequest();

    if (putHealth.result == UnityWebRequest.Result.Success)
        Debug.Log($"[Analytics] Health {sessionHealth} logged for {levelName}");
    else
        Debug.LogError($"[Analytics] Failed to log health for {levelName}: {putHealth.error}");

    if (putScore.result == UnityWebRequest.Result.Success)
        Debug.Log($"[Analytics] Score {score} logged for {levelName}");
    else
        Debug.LogError($"[Analytics] Failed to log score for {levelName}: {putScore.error}");

    Debug.Log($"[Analytics] WRITING to completion_stats - match: {sessionMatchCount}, mismatch: {sessionMismatchCount}");

    // RESET session counts after upload
    sessionMatchCount = 0;
    sessionMismatchCount = 0;
    sessionHealth = 0f;

    }

}