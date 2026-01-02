using UnityEngine;
using UnityEngine.UI;

public class NextLevelButtonListener : MonoBehaviour
{
    [Header("UI")]
    public Button nextLevelButton;
    public GameManager gameManager;

    void Start()
    {
        if (nextLevelButton && gameManager)
        {
            nextLevelButton.onClick.AddListener(OnNextLevelClick);
        }
    }

    public void EnableListener()
    {
        if (nextLevelButton)
        {
            nextLevelButton.interactable = true;
        }
    }

    void OnNextLevelClick()
    {
        if (gameManager)
        {
            gameManager.LoadNextLevel();
        }
    }

    void OnDestroy()
    {
        if (nextLevelButton)
        {
            nextLevelButton.onClick.RemoveListener(OnNextLevelClick);
        }
    }
}