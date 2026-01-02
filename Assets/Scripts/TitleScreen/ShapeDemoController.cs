using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShapeDemoController : MonoBehaviour
{
    public enum ShapeType { Circle, Triangle, Square }
    
    [SerializeField] private ShapeType shapeType;
    private Button button;
    private RectTransform rectTransform;
    private ProceduralShapeRenderer shapeRenderer;
    private Vector3 originalPosition;
    private Vector3 originalRotation;
    private bool isAnimating = false;
    
    void Start()
    {
        // Get components
        rectTransform = GetComponent<RectTransform>();
        shapeRenderer = GetComponent<ProceduralShapeRenderer>();
        
        // Store original transform values
        originalPosition = rectTransform.localPosition;
        originalRotation = rectTransform.localEulerAngles;
        
        // Setup button for clicking (optional)
        button = GetComponent<Button>();
        if (button == null) 
        {
            button = gameObject.AddComponent<Button>();
        }
        button.onClick.AddListener(OnShapeClick);
        
        // Set IMPROVED colors based on shape type
        if (shapeRenderer != null)
        {
            Color shapeColor = GetImprovedShapeColor();
            shapeRenderer.SetColor(shapeColor);
        }
        
        // Start animation immediately - no delay!
        StartFloatingAnimation();
        
        Debug.Log($"Shape {gameObject.name} started with type: {shapeType}");
    }
    
    Color GetImprovedShapeColor()
    {
        switch (shapeType)
        {
            case ShapeType.Circle:
                // Warmer, more vibrant coral
                return new Color(1f, 0.5f, 0.3f, 1f);
                
            case ShapeType.Triangle:
                // Cooler, more mint-like cyan
                return new Color(0.2f, 0.9f, 0.8f, 1f);
                
            case ShapeType.Square:
                // MUCH BETTER PURPLE - deeper, richer, more modern
                return new Color(0.7f, 0.3f, 1f, 1f); // Bright electric purple
                
            default:
                return Color.white;
        }
    }
    
    void OnShapeClick()
    {
        Debug.Log($"Clicked on {shapeType} shape!");
        StartCoroutine(ImprovedClickAnimation());
        
        // Notify TitleScreenManager if it exists
        if (TitleScreenManager.Instance != null)
        {
            TitleScreenManager.Instance.OnShapeSelected(shapeType);
        }
    }
    
    IEnumerator ImprovedClickAnimation()
    {
        Vector3 originalScale = rectTransform.localScale;
        Image shapeImage = GetComponent<Image>();
        Color originalColor = shapeImage ? shapeImage.color : Color.white;
        
        // More exciting click animation with color flash
        float timer = 0f;
        while (timer < 0.15f)
        {
            timer += Time.deltaTime;
            float progress = timer / 0.15f;
            
            // Bouncy scale animation
            float bounceScale = Mathf.Lerp(1f, 1.6f, Mathf.Sin(progress * Mathf.PI));
            rectTransform.localScale = originalScale * bounceScale;
            
            // Flash to white for exciting feedback
            if (shapeImage != null)
            {
                Color flashColor = Color.Lerp(originalColor, Color.white, Mathf.Sin(progress * Mathf.PI) * 0.8f);
                shapeImage.color = flashColor;
            }
            
            yield return null;
        }
        
        // Return to normal
        rectTransform.localScale = originalScale;
        if (shapeImage != null)
        {
            shapeImage.color = originalColor;
        }
    }
    
    void StartFloatingAnimation()
    {
        if (!isAnimating)
        {
            isAnimating = true;
            StartCoroutine(ImprovedFloatingLoop());
            Debug.Log($"Started floating animation for {gameObject.name}");
        }
    }
    
    IEnumerator ImprovedFloatingLoop()
    {
        float timeOffset = (int)shapeType * 1.5f; // Different timing for each shape
        float elapsed = timeOffset; // Start with offset
        
        while (isAnimating)
        {
            elapsed += Time.deltaTime;
            
            // Different movement patterns for each shape type
            float yOffset = GetShapeMovement(elapsed);
            
            // Varied rotation speeds for visual interest
            float rotationSpeed = GetRotationSpeed();
            float rotationZ = elapsed * rotationSpeed;
            
            // Subtle scale pulsing with different rhythms
            float scaleSpeed = GetScaleSpeed();
            float scaleMultiplier = 1f + Mathf.Sin(elapsed * scaleSpeed) * 0.12f;
            
            // Apply transformations
            rectTransform.localPosition = originalPosition + Vector3.up * yOffset;
            rectTransform.localEulerAngles = originalRotation + Vector3.forward * rotationZ;
            rectTransform.localScale = Vector3.one * scaleMultiplier;
            
            yield return null; // Wait for next frame
        }
    }
    
    float GetShapeMovement(float time)
    {
        switch (shapeType)
        {
            case ShapeType.Circle:
                // Smooth, meditative movement
                return Mathf.Sin(time * 0.6f) * 30f;
                
            case ShapeType.Triangle:
                // More energetic, quicker movement
                return Mathf.Sin(time * 0.9f) * 25f;
                
            case ShapeType.Square:
                // Complex movement with harmonics
                return (Mathf.Sin(time * 0.5f) + Mathf.Sin(time * 1.5f) * 0.3f) * 28f;
                
            default:
                return Mathf.Sin(time * 0.5f) * 25f;
        }
    }
    
    float GetRotationSpeed()
    {
        switch (shapeType)
        {
            case ShapeType.Circle:
                return 25f; // Slower, more graceful
            case ShapeType.Triangle:
                return 40f; // Faster, more dynamic
            case ShapeType.Square:
                return 20f; // Steady, reliable
            default:
                return 30f;
        }
    }
    
    float GetScaleSpeed()
    {
        switch (shapeType)
        {
            case ShapeType.Circle:
                return 0.7f; // Gentle breathing
            case ShapeType.Triangle:
                return 1.1f; // Quick pulsing
            case ShapeType.Square:
                return 0.9f; // Steady rhythm
            default:
                return 0.8f;
        }
    }
    
    void OnDestroy()
    {
        isAnimating = false;
    }
}