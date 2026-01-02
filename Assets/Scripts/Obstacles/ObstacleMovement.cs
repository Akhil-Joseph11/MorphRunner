using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    public ShapeType obstacleShape;
    public PlayerColor obstacleColor; //added

    public float rotationSpeed = 90f;

    public GameObject onCollectEffect;

    public PlayerColor userColor; //added
    // SpriteRenderer sr = GetComponent<SpriteRenderer>(); //added
    // Color obstacleColor = sr.color; //added

    void Update()
    {
        // if (GameManager.Instance == null) return;
        // // Scroller moves downward
        // transform.Translate(Vector3.down * GameManager.Instance.currentObstacleSpeed * Time.deltaTime);

        if (transform.position.y < Camera.main.transform.position.y - 8f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (CompareTag("Wall"))
        {
            GameManager.Instance.GameOver();
            return;
        }
        
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
         
        if (playerMovement == null) return;

        // bool colorMatches = playerMovement.playerColors[playerMovement.playerColorIndex] == obstacleColor; //added

        // Shape Matched
        if ((ShapeType)playerMovement.CurrentShape == obstacleShape && (PlayerColor)playerMovement.currentPlayerColor == obstacleColor)
        {            
            FindObjectOfType<ObstacleMismatchLogging>()?.LogCorrectMatch(); // Log correct match

            GameStatsManager.Instance.AddHP(10);   // Matching +10
            GameStatsManager.Instance.ShowPositiveMatchStatus("Match +10", 1f);

            if (onCollectEffect) Instantiate(onCollectEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        else if (obstacleShape != ShapeType.ShapeShifter)
        {
            Debug.Log("=== WRONG MATCH DETECTED ===");

             // Log mismatch and y-position
            FindObjectOfType<ObstacleMismatchLogging>()?.LogMismatch(transform.position.y);

            GameStatsManager.Instance.ReduceHP(20);
            GameStatsManager.Instance.ShowNegativeMatchStatus("Wrong Match! -20", 1f);
            GameManager.Instance.SlowDownObstacles(0.4f, 1.5f);
            CameraShake.Instance.Shake(0.2f, 0.30f);
            
            Debug.Log("About to call ShowShapeMismatchPopup...");
            //  ADD POPUP FOR SHAPE MISMATCH 
            ShowShapeMismatchPopup(playerMovement);
            Debug.Log("ShowShapeMismatchPopup call completed");
        }
    }
    
    /// <summary>
    /// Show popup when shapes/colors don't match
    /// </summary>
    private void ShowShapeMismatchPopup(PlayerMovement playerMovement)
    {
        Debug.Log("=== ShowShapeMismatchPopup called ===");
        
        // Check if PopupManager exists in the scene
        if (FindObjectOfType<PopupManager>() == null)
        {
            Debug.LogWarning("PopupManager not found! Please create the popup system first.");
            return;
        }
        else
        {
            Debug.Log("PopupManager found in scene!");
        }
        
        // Get current player info
        string playerShapeStr = GetShapeString((ShapeType)playerMovement.CurrentShape);
        string playerColorStr = playerMovement.currentPlayerColor.ToString();
        
        // Get required obstacle info
        string obstacleShapeStr = GetShapeString(obstacleShape);
        string obstacleColorStr = obstacleColor.ToString();
        
        Debug.Log($"Player: {playerColorStr} {playerShapeStr}");
        Debug.Log($"Obstacle: {obstacleColorStr} {obstacleShapeStr}");
        
        // Determine what didn't match
        bool shapeMatches = (ShapeType)playerMovement.CurrentShape == obstacleShape;
        bool colorMatches = (PlayerColor)playerMovement.currentPlayerColor == obstacleColor;
        
        Debug.Log($"Shape matches: {shapeMatches}, Color matches: {colorMatches}");
        
        // Show appropriate generalized popup based on what doesn't match
        if (!shapeMatches && !colorMatches)
        {
            // Both wrong
            Debug.Log("Both shape and color mismatch - showing combined popup");
            PopupManager.ShowShapeAndColorMismatch();
        }
        else if (!shapeMatches)
        {
            // Only shape wrong
            Debug.Log("Shape mismatch only - showing shape popup");
            PopupManager.ShowShapeMismatch();
        }
        else if (!colorMatches)
        {
            // Only color wrong
            Debug.Log("Color mismatch only - showing color popup");
            PopupManager.ShowColorMismatch();
        }
        
        Debug.Log("PopupManager popup call completed!");
    }
    
    /// <summary>
    /// Convert ShapeType enum to readable string
    /// </summary>
    private string GetShapeString(ShapeType shape)
    {
        switch (shape)
        {
            case ShapeType.Circle: return "Circle";
            case ShapeType.Triangle: return "Triangle";
            case ShapeType.Square: return "Square";
            case ShapeType.ShapeShifter: return "ShapeShifter";
            default: return shape.ToString();
        }
    }
}