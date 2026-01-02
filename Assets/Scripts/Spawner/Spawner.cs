using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    
    public float[] lanes = new float[] { -2f, 0f, 2f };

    void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        string levelDataPath = $"{currentSceneName} data/{currentSceneName.ToLower()}";
        LoadAndSpawnLevel(levelDataPath);
    }

    void LoadAndSpawnLevel(string levelName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(levelName);
        if (jsonFile != null)
        {
            LevelData[] levelDataArray = JsonHelper.FromJson<LevelData>(jsonFile.text);

            foreach (LevelData data in levelDataArray)
            {
                SpawnObstacle(data);
            }
        }
        else
        {
            Debug.LogError("Could not find level JSON: " + levelName);
        }
    }

    void SpawnObstacle(LevelData data)
    {
        if (IsValid(data))
        {
            Vector3 pos = new Vector3(lanes[data.laneIndex], data.spawnY, 0f);
            Quaternion rotation = Quaternion.identity;
            //Todo: optimize rotation
            if (data.obstacleIndex == 0)
            {
                rotation = Quaternion.Euler(0, 0, 90);
            }
            Instantiate(obstaclePrefabs[data.obstacleIndex], pos, rotation);
        }
        else
        {
            Debug.LogWarning("Invalid spawn data: " + JsonUtility.ToJson(data));
        }
    }

    bool IsValid(LevelData data)
    {
        return data.obstacleIndex >= 0 && data.obstacleIndex < obstaclePrefabs.Length &&
               data.laneIndex >= 0 && data.laneIndex < lanes.Length;
    }
}
