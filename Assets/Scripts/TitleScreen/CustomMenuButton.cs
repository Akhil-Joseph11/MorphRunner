using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; // Added for TextMeshPro
using System.Collections;

public class CustomMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool isPrimary = false;
    [SerializeField] private string buttonText = "BUTTON";
    
    private Button button;
    private Image buttonImage;
    private TextMeshProUGUI textComponent;  // Changed from Text
    private RectTransform rectTransform;
    private Color normalColor;
    private Color hoverColor;
    
    void Start()
    {
        SetupButton();
        CreateButtonBackground();
        SetupColors();
    }
    
    void SetupButton()
    {
        button = GetComponent<Button>();
        if (button == null) button = gameObject.AddComponent<Button>();
        
        buttonImage = GetComponent<Image>();
        if (buttonImage == null) buttonImage = gameObject.AddComponent<Image>();
        
        rectTransform = GetComponent<RectTransform>();
        
        // Create TextMeshPro text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(transform);
        textComponent = textObj.AddComponent<TextMeshProUGUI>(); // Changed to TextMeshProUGUI
        textComponent.text = buttonText;
        textComponent.fontSize = 15;
        textComponent.color = Color.white;
        textComponent.alignment = TextAlignmentOptions.Center; // Changed alignment
        textComponent.fontStyle = FontStyles.Bold;
        
        RectTransform textRect = textComponent.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }
    
    void CreateButtonBackground()
    {
        Texture2D buttonTexture = new Texture2D(100, 40);
        
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 40; y++)
            {
                buttonTexture.SetPixel(x, y, Color.white);
            }
        }
        
        buttonTexture.Apply();
        Sprite buttonSprite = Sprite.Create(buttonTexture, new Rect(0, 0, 100, 40), new Vector2(0.5f, 0.5f));
        buttonImage.sprite = buttonSprite;
    }
    
    void SetupColors()
    {
        if (isPrimary)
        {
            normalColor = new Color(1f, 0.42f, 0.42f, 0.8f);
            hoverColor = new Color(1f, 0.32f, 0.32f, 0.9f);
        }
        else
        {
            normalColor = new Color(1f, 1f, 1f, 0.3f);
            hoverColor = new Color(1f, 1f, 1f, 0.4f);
        }
        
        buttonImage.color = normalColor;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.color = hoverColor;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.color = normalColor;
    }
}