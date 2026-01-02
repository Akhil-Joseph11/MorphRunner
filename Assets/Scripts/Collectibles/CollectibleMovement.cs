using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectibleMovement : MonoBehaviour
{
    public ShapeType obstacleShape;

    public float rotationSpeed = 90f;

    public GameObject onCollectEffect;

    void Update()
    {
        // if (GameManager.Instance == null) return;
        
        // transform.Translate(Vector3.down * GameManager.Instance.currentSpeed * Time.deltaTime, Space.World);

        if (transform.position.y < Camera.main.transform.position.y - 8f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
{
    if (!other.CompareTag("Player")) return;
    if (CompareTag("FinishLine"))
    {
        Debug.Log("COLLECTIBLE: Finish line hit! Calling GameManager.FinishLine()");
        GameManager.Instance.FinishLine();
        return;
    }
}


    int ExtractLevelNumber(string sceneName)
    {
        for (int i = 0; i < sceneName.Length; i++)
        {
            if (char.IsDigit(sceneName[i]))
            {
                string number = sceneName.Substring(i);
                if (int.TryParse(number, out int level))
                {
                    return level;
                }
            }
        }
        Debug.LogWarning("No number found in scene name. Defaulting to 1.");
        return 1;
    }
}
