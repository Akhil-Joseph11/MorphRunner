using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float lastShapeChangeTime;  // TimeStamp

    // Define the X positions for the 3 lanes
    private float[] lanePositions = new float[] { -2f, 0f, 2f };

    // Start in middle lane (index 1)
    private int currentLane = 1;

    // Initialize array of Sprites for shape change
    public Sprite[] spriteShapes;
    private int currentShapeIndex = 0;

    //store player colors in an array
    public Color[] playerColors;
    private int playerColorIndex = 0;

    // Reference to SpriteRenderer attached to player GameObject
    private SpriteRenderer spriteRenderer;

    // Hold last 2 shape index on level 3
    private List<int> shapeStack = new List<int>();

    // Expose current shape as a property
    public ShapeType CurrentShape
    {
        get
        {
            switch (currentShapeIndex)
            {
                case 0: return ShapeType.Circle;
                case 1: return ShapeType.Triangle;
                case 2: return ShapeType.Square;
                case 3: return ShapeType.CC;
                case 4: return ShapeType.CT;
                case 5: return ShapeType.CS;
                case 6: return ShapeType.TC;
                case 7: return ShapeType.TT;
                case 8: return ShapeType.TS;
                case 9: return ShapeType.SC;
                case 10: return ShapeType.ST;
                case 11: return ShapeType.SS;
                default: return ShapeType.Circle;
            }
        }
    }

    public PlayerColor currentPlayerColor
    {
        get
        {
            switch (playerColorIndex)
            {
                case 0:
                    return PlayerColor.Black;
                case 1:
                    return PlayerColor.Red;
                default:
                    return PlayerColor.Black;
            }
        }
    }

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            string scene = SceneManager.GetActiveScene().name;
            if (scene == "Level3" || scene == "Level4")

            {
                shapeStack.Clear();
                shapeStack.Add(0); // C
                shapeStack.Add(0); // C

                string shapeKey = GetShapeKeyFromStack();
                int combinedIndex = GetShapeSpriteIndexFromKey(shapeKey);
                if (combinedIndex >= 0 && combinedIndex < spriteShapes.Length)
                {
                    currentShapeIndex = combinedIndex;
                    spriteRenderer.sprite = spriteShapes[currentShapeIndex];
                }
            } else {
                if (spriteShapes.Length > 0)
                {
                    spriteRenderer.sprite = spriteShapes[currentShapeIndex];
                }
            }
        }

    void Update()
    {
        if (GameManager.Instance == null) return;
        transform.Translate(Vector3.up * GameManager.Instance.currentSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            MoveRight();
        }

        if (Input.GetKeyDown(KeyCode.Space)) //change player color
        {
            UpdatePlayerColor();
        }
    }

    void MoveLeft()
    {
        if (currentLane > 0)
        {
            currentLane--;
            UpdatePlayerPosition();
        }
    }

    void MoveRight()
    {
        if (currentLane < lanePositions.Length - 1)
        {
            currentLane++;
            UpdatePlayerPosition();
        }
    }

    void UpdatePlayerPosition()
    {
        Vector3 newPosition = transform.position;
        newPosition.x = lanePositions[currentLane];
        transform.position = newPosition;
    }

void UpdatePlayerShape(int shapeIndex)
{
    if (spriteRenderer != null && shapeIndex < spriteShapes.Length)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "Level3" || currentSceneName == "Level4") { // Level 3 and Level 4
            Debug.Log($"[Stacking] Running in {currentSceneName} with shapeIndex = {shapeIndex}");

            if (shapeStack.Count >= 2)
            {
                shapeStack.RemoveAt(0);
            }
            shapeStack.Add(shapeIndex);
            Debug.Log($"[Stacking] shapeStack = [{string.Join(", ", shapeStack)}]");

            string shapeKey = GetShapeKeyFromStack();
            int combinedIndex = GetShapeSpriteIndexFromKey(shapeKey);
            if (combinedIndex >= 0 && combinedIndex < spriteShapes.Length)
            {
                currentShapeIndex = combinedIndex;
                spriteRenderer.sprite = spriteShapes[currentShapeIndex];
            }
        } else { // other levels
            currentShapeIndex = shapeIndex;
            string shapeKey = GetShapeKeyFromStack();
            int combinedIndex = GetShapeSpriteIndexFromKey(shapeKey);
            Debug.Log($"[Sprite Update] shapeKey = {shapeKey} → combinedIndex = {combinedIndex}");
            spriteRenderer.sprite = spriteShapes[currentShapeIndex];
        }
    }
    lastShapeChangeTime = Time.time;
}


    // void UpdatePlayerShapeLoop()
    // {
    //     currentShapeIndex = (currentShapeIndex + 1) % spriteShapes.Length;
    //     UpdatePlayerShape(currentShapeIndex);
    // }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ShapeShift"))
        {
            ShapeShiftPickup pickup = other.GetComponent<ShapeShiftPickup>();
            if (pickup != null)
            {
                Debug.Log($"[Pickup] Triggered in scene {SceneManager.GetActiveScene().name} with shapeIndex = {pickup.shapeIndex}");
                UpdatePlayerShape(pickup.shapeIndex);
                Destroy(other.gameObject);
            }
        }
    }

    
    void UpdatePlayerColor()
    {
        playerColorIndex = (playerColorIndex + 1) % playerColors.Length;
        Debug.Log($"Changing player color to index {playerColorIndex} color {playerColors[playerColorIndex]}");


        if(spriteRenderer != null && playerColorIndex < playerColors.Length)
        {
            spriteRenderer.color = playerColors[playerColorIndex];
        }
        
    }

    string GetShapeKeyFromStack()
    {
        List<string> key = new List<string>();
        foreach (int i in shapeStack)
        {
            if (i == 0) key.Add("C");
            else if (i == 1) key.Add("T");
            else if (i == 2) key.Add("S");
        }
        return string.Join("", key);
    }

    int GetShapeSpriteIndexFromKey(string key)
    {
        switch (key)
        {
            case "C": return 0;
            case "T": return 1;
            case "S": return 2;
            case "CC": return 3;
            case "CT": return 4;
            case "CS": return 5;
            case "TC": return 6;
            case "TT": return 7;
            case "TS": return 8;
            case "SC": return 9;
            case "ST": return 10;
            case "SS": return 11;
            default: return 0;
        }
    }
}