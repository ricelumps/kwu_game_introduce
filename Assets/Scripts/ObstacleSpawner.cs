using System.Collections;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Ground Obstacles")]
    [SerializeField] private GameObject[] groundObstaclePrefabs;
    [SerializeField] private Transform groundSpawnPoint;

    [Header("Air Obstacles")]
    [SerializeField] private GameObject[] airObstaclePrefabs;
    [SerializeField] private Transform airSpawnPoint;
    [SerializeField] private float airObstacleChance = 0.3f;

    private Coroutine spawnRoutine;

    public void StartSpawning()
    {
        StopSpawning();
        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float interval = GetSpawnInterval();
            yield return new WaitForSeconds(interval);

            if (GameManager.Instance != null && GameManager.Instance.IsPlaying())
            {
                SpawnObstacle();
            }
        }
    }

    private void SpawnObstacle()
    {
        bool canSpawnAir = airObstaclePrefabs != null && airObstaclePrefabs.Length > 0 && airSpawnPoint != null;
        bool shouldSpawnAir = canSpawnAir && Random.value < airObstacleChance;

        if (shouldSpawnAir)
        {
            SpawnFromArray(airObstaclePrefabs, airSpawnPoint);
        }
        else
        {
            SpawnFromArray(groundObstaclePrefabs, groundSpawnPoint);
        }
    }

    private void SpawnFromArray(GameObject[] prefabs, Transform spawnPoint)
    {
        if (prefabs == null || prefabs.Length == 0 || spawnPoint == null)
        {
            Debug.LogWarning("장애물 Prefab 또는 SpawnPoint가 비어 있습니다.");
            return;
        }

        int index = Random.Range(0, prefabs.Length);
        Instantiate(prefabs[index], spawnPoint.position, Quaternion.identity);
    }

    private float GetSpawnInterval()
    {
        if (SpeedManager.Instance == null)
        {
            return Random.Range(2.0f, 3.0f);
        }

        float speed = SpeedManager.Instance.GetCurrentSpeed();

        if (speed < 7f)
        {
            return Random.Range(2.0f, 3.0f);
        }

        if (speed < 9f)
        {
            return Random.Range(1.7f, 2.5f);
        }

        if (speed < 11f)
        {
            return Random.Range(1.3f, 2.0f);
        }

        if (speed < 13f)
        {
            return Random.Range(1.0f, 1.7f);
        }

        if (speed < 16f)
        {
            return Random.Range(0.8f, 1.3f);
        }
        if (speed < 20f)
        {
            return Random.Range(0.7f, 1.0f);
        }


        return Random.Range(0.6f, 0.8f);
    }
}