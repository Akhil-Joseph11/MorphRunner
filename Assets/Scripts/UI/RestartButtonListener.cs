using UnityEngine;
using UnityEngine.UI;
public class RestartButtonListener : MonoBehaviour
{
    [Header("UI")]
    public Button restartButton;
    public GameManager gameManager;
    bool listenerAdded = false;

    void Start()
    {
        if (restartButton != null)
            restartButton.interactable = false;
    }

    public void EnableListener()
    {
        if (listenerAdded) return;
        if (restartButton == null || gameManager == null)
        {
            Debug.LogError("RestartButtonListener！");
            return;
        }

        restartButton.interactable = true;
        restartButton.onClick.AddListener(HandleRestart);
        listenerAdded = true;
    }

    void HandleRestart()
    {
        Debug.Log("<color=lime>Restart</color>");
        restartButton.interactable = false;
        restartButton.onClick.RemoveListener(HandleRestart);
        gameManager.RestartGame();
    }

    void OnDestroy()
    {
        if (listenerAdded && restartButton != null)
            restartButton.onClick.RemoveListener(HandleRestart);
    }
}
