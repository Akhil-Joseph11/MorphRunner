using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Text;

public class UsernameManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameInputField;
    public Button startGameButton;
    public TextMeshProUGUI placeholderText;
    public TextMeshProUGUI errorMessage;
    
    [Header("Settings")]
    public int minUsernameLength = 3;
    public int maxUsernameLength = 15;
    
    private const string firebaseURL = "https://morphrunneranalytics3107-default-rtdb.firebaseio.com/";
    private string playerUsername = "";
    private string userId;
    
    void Start()
    {
        // Get or create user ID (same system as ObstacleMismatchLogging)
        if (PlayerPrefs.HasKey("user_id"))
        {
            userId = PlayerPrefs.GetString("user_id");
        }
        else
        {
            userId = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("user_id", userId);
        }
        
        Debug.Log($"[UsernameManager] Initialized with userId: {userId}");
        
        // Initially disable the start game button
        if (startGameButton != null)
            startGameButton.interactable = false;
        
        // Set up input field listener
        if (usernameInputField != null)
            usernameInputField.onValueChanged.AddListener(OnUsernameChanged);
        
        // Hide error message initially
        if (errorMessage != null)
            errorMessage.gameObject.SetActive(false);
            
        // Set placeholder text
        if (placeholderText != null)
            placeholderText.text = "Enter your username...";
    }
    
    public void OnUsernameChanged(string username)
    {
        playerUsername = username.Trim();
        
        // Validate username
        bool isValid = ValidateUsername(playerUsername);
        
        // Enable/disable start button based on validation
        if (startGameButton != null)
            startGameButton.interactable = isValid;
        
        // Show/hide error message
        if (errorMessage != null)
        {
            if (playerUsername.Length > 0 && !isValid)
            {
                errorMessage.gameObject.SetActive(true);
                errorMessage.text = GetErrorMessage(playerUsername);
            }
            else
            {
                errorMessage.gameObject.SetActive(false);
            }
        }
    }
    
    private bool ValidateUsername(string username)
    {
        // Check if username is not empty and within length limits
        if (string.IsNullOrEmpty(username))
            return false;
            
        if (username.Length < minUsernameLength || username.Length > maxUsernameLength)
            return false;
            
        // Check for invalid characters (optional)
        foreach (char c in username)
        {
            if (!char.IsLetterOrDigit(c) && c != '_' && c != '-')
                return false;
        }
        
        return true;
    }
    
    private string GetErrorMessage(string username)
    {
        if (username.Length < minUsernameLength)
            return $"Username must be at least {minUsernameLength} characters";
            
        if (username.Length > maxUsernameLength)
            return $"Username must be no more than {maxUsernameLength} characters";
            
        return "Username contains invalid characters";
    }
    
    public void OnStartGameClicked()
    {
        Debug.Log("OnStartGameClicked method called!");
        
        if (ValidateUsername(playerUsername))
        {
            // Save username locally
            PlayerPrefs.SetString("PlayerUsername", playerUsername);
            PlayerPrefs.Save();
            
            Debug.Log($"Starting game with username: {playerUsername}");
            
            // Save username to Firebase
            StartCoroutine(SaveUsernameToFirebase(playerUsername));
        }
        else
        {
            Debug.Log("Username validation failed!");
        }
    }
    
    private IEnumerator SaveUsernameToFirebase(string username)
    {
        // Save username to Firebase under the user's ID
        string url = $"{firebaseURL}users/{userId}/username.json";
        string jsonData = $"\"{username}\""; // JSON string format
        
        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.SetRequestHeader("Content-Type", "application/json");
        
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"[UsernameManager] Successfully saved username '{username}' to Firebase");
            
            // Also save a timestamp for when the user registered
            StartCoroutine(SaveRegistrationTimestamp());
            
            // Load the game scene after successful Firebase save
            SceneManager.LoadScene("Level1");
        }
        else
        {
            Debug.LogError($"[UsernameManager] Failed to save username to Firebase: {request.error}");
            
            // Still load the game even if Firebase fails (offline play)
            SceneManager.LoadScene("Level1");
        }
    }
    
    private IEnumerator SaveRegistrationTimestamp()
    {
        string url = $"{firebaseURL}users/{userId}/registration_time.json";
        string timestamp = System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        string jsonData = $"\"{timestamp}\"";
        
        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.SetRequestHeader("Content-Type", "application/json");
        
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"[UsernameManager] Registration timestamp saved: {timestamp}");
        }
        else
        {
            Debug.LogError($"[UsernameManager] Failed to save registration timestamp: {request.error}");
        }
    }
    
    // Method to get the current username (can be called from other scripts)
    public static string GetCurrentUsername()
    {
        return PlayerPrefs.GetString("PlayerUsername", "Player");
    }
    
    // Method to get the current user ID (useful for leaderboards)
    public static string GetCurrentUserId()
    {
        return PlayerPrefs.GetString("user_id", "");
    }
}