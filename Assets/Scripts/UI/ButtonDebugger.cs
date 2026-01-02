using UnityEngine;
using UnityEngine.UI;

public class ButtonDebugger : MonoBehaviour
{
    private Button button;
    private Image buttonImage;
    private bool wasInteractable;
    private Color lastColor;

    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        wasInteractable = button.interactable;
        lastColor = buttonImage.color;
    }

    void Update()
    {
        // Check if interactable changed
        if (button.interactable != wasInteractable)
        {
            Debug.Log($"Button Interactable changed: {wasInteractable} -> {button.interactable}");
            wasInteractable = button.interactable;
        }

        // Check if color changed
        if (buttonImage.color != lastColor)
        {
            Debug.Log($"Button Color changed: {lastColor} -> {buttonImage.color}");
            lastColor = buttonImage.color;
        }

        // Check if button component is enabled
        if (!button.enabled)
        {
            Debug.Log("Button component is DISABLED!");
        }
    }
}