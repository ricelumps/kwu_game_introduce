using System.Collections;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private Transform spawnPoint;

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
        if (obstaclePrefabs.Length == 0 || spawnPoint == null)
        {
            Debug.LogWarning("ObstacleSpawner 설정이 비어 있습니다.");
            return;
        }

        int index = Random.Range(0, obstaclePrefabs.Length);
        Instantiate(obstaclePrefabs[index], spawnPoint.position, Quaternion.identity);
    }

    private float GetSpawnInterval()
    {
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
            return Random.Range(1.0f, 1.6f);
        }

        return Random.Range(0.8f, 1.4f);
    }
}