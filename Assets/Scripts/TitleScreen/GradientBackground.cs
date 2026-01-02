using UnityEngine;
using UnityEngine.UI;

public class GradientBackground : MonoBehaviour
{
    [SerializeField] private Color topColor = new Color(0.4f, 0.5f, 0.9f, 1f);
    [SerializeField] private Color bottomColor = new Color(0.46f, 0.29f, 0.64f, 1f);
    
    void Start()
    {
        CreateGradientTexture();
    }
    
    void CreateGradientTexture()
    {
        Image img = GetComponent<Image>();
        if (img == null) img = gameObject.AddComponent<Image>();
        
        Texture2D gradientTexture = new Texture2D(1, 256);
        
        for (int y = 0; y < 256; y++)
        {
            Color lerpedColor = Color.Lerp(bottomColor, topColor, (float)y / 255f);
            gradientTexture.SetPixel(0, y, lerpedColor);
        }
        
        gradientTexture.Apply();
        
        Sprite gradientSprite = Sprite.Create(gradientTexture, new Rect(0, 0, 1, 256), new Vector2(0.5f, 0.5f));
        img.sprite = gradientSprite;
        // Fixed: Changed from Stretched to Sliced
        img.type = Image.Type.Sliced;
    }
}