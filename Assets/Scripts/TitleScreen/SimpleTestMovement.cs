using UnityEngine;
using UnityEngine.UI;

public class SimpleTestMovement : MonoBehaviour
{
    private RectTransform rectTransform;
    
    void Start()
    {
        // Get RectTransform
        rectTransform = GetComponent<RectTransform>();
        
        // Force correct settings
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.localPosition = Vector3.zero;
        
        Debug.Log("SIMPLE TEST: Started at center");
    }
    
    void Update()
    {
        // SIMPLE Update-based movement (no coroutines)
        float yPos = Mathf.Sin(Time.time * 2f) * 200f;
        float scale = 1f + Mathf.Sin(Time.time * 4f) * 0.5f;
        
        rectTransform.localPosition = new Vector3(0f, yPos, 0f);
        rectTransform.localScale = Vector3.one * scale;
        rectTransform.localEulerAngles = new Vector3(0f, 0f, Time.time * 90f);
        
        // Debug every 60 frames
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"SIMPLE TEST: Y={yPos:F1} Scale={scale:F2}");
        }
    }
}