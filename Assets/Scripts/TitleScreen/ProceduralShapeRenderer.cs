using UnityEngine;
using UnityEngine.UI;

public class ProceduralShapeRenderer : MonoBehaviour
{
    public enum ShapeType { Circle, Triangle, Square }
    
    [SerializeField] private ShapeType shapeType;
    [SerializeField] private Color shapeColor = Color.white;
    // Removed unused 'size' field to fix warning
    
    private Image imageComponent;
    
    void Start()
    {
        imageComponent = GetComponent<Image>();
        if (imageComponent == null)
        {
            imageComponent = gameObject.AddComponent<Image>();
        }
        
        CreateShape();
    }
    
    void CreateShape()
    {
        Texture2D texture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
        
        switch (shapeType)
        {
            case ShapeType.Circle:
                CreateCircle(texture);
                break;
            case ShapeType.Triangle:
                CreateTriangle(texture);
                break;
            case ShapeType.Square:
                CreateSquare(texture);
                break;
        }
        
        texture.Apply();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
        imageComponent.sprite = sprite;
        imageComponent.color = shapeColor;
    }
    
    void CreateCircle(Texture2D texture)
    {
        Vector2 center = new Vector2(64, 64);
        float radius = 50f;
        
        for (int x = 0; x < 128; x++)
        {
            for (int y = 0; y < 128; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= radius)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
    }
    
    void CreateTriangle(Texture2D texture)
    {
        // Clear texture
        for (int x = 0; x < 128; x++)
        {
            for (int y = 0; y < 128; y++)
            {
                texture.SetPixel(x, y, Color.clear);
            }
        }
        
        // Draw triangle
        Vector2 p1 = new Vector2(64, 100);  // Top
        Vector2 p2 = new Vector2(20, 28);   // Bottom left
        Vector2 p3 = new Vector2(108, 28);  // Bottom right
        
        for (int x = 0; x < 128; x++)
        {
            for (int y = 0; y < 128; y++)
            {
                if (IsPointInTriangle(new Vector2(x, y), p1, p2, p3))
                {
                    texture.SetPixel(x, y, Color.white);
                }
            }
        }
    }
    
    void CreateSquare(Texture2D texture)
    {
        for (int x = 0; x < 128; x++)
        {
            for (int y = 0; y < 128; y++)
            {
                if (x >= 20 && x <= 108 && y >= 20 && y <= 108)
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
    }
    
    bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float denominator = ((b.y - c.y) * (a.x - c.x) + (c.x - b.x) * (a.y - c.y));
        if (Mathf.Abs(denominator) < 0.001f) return false;
        
        float alpha = ((b.y - c.y) * (p.x - c.x) + (c.x - b.x) * (p.y - c.y)) / denominator;
        float beta = ((c.y - a.y) * (p.x - c.x) + (a.x - c.x) * (p.y - c.y)) / denominator;
        float gamma = 1.0f - alpha - beta;
        
        return alpha >= 0 && beta >= 0 && gamma >= 0;
    }
    
    public void SetColor(Color color)
    {
        shapeColor = color;
        if (imageComponent != null)
            imageComponent.color = color;
    }
}