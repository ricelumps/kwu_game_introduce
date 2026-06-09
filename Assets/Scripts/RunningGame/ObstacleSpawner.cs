using System.Collections;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ObstacleEntry
    {
        public GameObject prefab;

        [Tooltip("체크하면 공중 SpawnPoint에서 생성합니다.")]
        public bool spawnInAir;
    }

    [Header("Spawn Points")]
    [SerializeField] private Transform groundSpawnPoint;
    [SerializeField] private Transform airSpawnPoint;

    [Header("0 ~ 10 Seconds")]
    [SerializeField] private ObstacleEntry[] phase1Obstacles;

    [Header("10 ~ 30 Seconds")]
    [SerializeField] private ObstacleEntry[] phase2Obstacles;

    [Header("30 ~ 50 Seconds")]
    [SerializeField] private ObstacleEntry[] phase3Obstacles;

    [Header("50 ~ 60 Seconds")]
    [SerializeField] private ObstacleEntry[] phase4Obstacles;

    private Coroutine spawnCoroutine;
    private float elapsedTime;

    private void Update()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.IsPlaying())
        {
            elapsedTime += Time.deltaTime;
        }
    }

    public void StartSpawning()
    {
        StopSpawning();

        elapsedTime = 0f;
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        if (spawnCoroutine == null)
        {
            return;
        }

        StopCoroutine(spawnCoroutine);
        spawnCoroutine = null;
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(GetSpawnInterval());

            if (GameManager.Instance != null &&
                GameManager.Instance.IsPlaying())
            {
                SpawnObstacle();
            }
        }
    }

    private void SpawnObstacle()
    {
        ObstacleEntry[] currentObstacles = GetCurrentObstacles();

        if (currentObstacles == null || currentObstacles.Length == 0)
        {
            Debug.LogWarning(
                $"{elapsedTime:F1}초 구간의 장애물이 설정되지 않았습니다."
            );
            return;
        }

        ObstacleEntry selected =
            currentObstacles[Random.Range(0, currentObstacles.Length)];

        if (selected.prefab == null)
        {
            Debug.LogWarning("Obstacle Prefab이 연결되지 않았습니다.");
            return;
        }

        Transform spawnPoint =
            selected.spawnInAir ? airSpawnPoint : groundSpawnPoint;

        if (spawnPoint == null)
        {
            Debug.LogWarning("Obstacle SpawnPoint가 연결되지 않았습니다.");
            return;
        }

        Instantiate(
            selected.prefab,
            spawnPoint.position,
            Quaternion.identity
        );

        Debug.Log(
            $"{elapsedTime:F1}초: {selected.prefab.name} 생성"
        );
    }

    private ObstacleEntry[] GetCurrentObstacles()
    {
        if (elapsedTime < 10f)
        {
            return phase1Obstacles;
        }

        if (elapsedTime < 30f)
        {
            return phase2Obstacles;
        }

        if (elapsedTime < 50f)
        {
            return phase3Obstacles;
        }

        if (elapsedTime < 60f)
        {
            return phase4Obstacles;
        }

        return GetAllObstacles();
    }

    private ObstacleEntry[] GetAllObstacles()
    {
        int totalLength =
            phase1Obstacles.Length +
            phase2Obstacles.Length +
            phase3Obstacles.Length +
            phase4Obstacles.Length;

        ObstacleEntry[] allObstacles =
            new ObstacleEntry[totalLength];

        int destinationIndex = 0;

        CopyEntries(phase1Obstacles, allObstacles, ref destinationIndex);
        CopyEntries(phase2Obstacles, allObstacles, ref destinationIndex);
        CopyEntries(phase3Obstacles, allObstacles, ref destinationIndex);
        CopyEntries(phase4Obstacles, allObstacles, ref destinationIndex);

        return allObstacles;
    }

    private void CopyEntries(
        ObstacleEntry[] source,
        ObstacleEntry[] destination,
        ref int destinationIndex)
    {
        for (int i = 0; i < source.Length; i++)
        {
            destination[destinationIndex] = source[i];
            destinationIndex++;
        }
    }

    private float GetSpawnInterval()
    {
        if (SpeedManager.Instance == null)
        {
            return Random.Range(2f, 3f);
        }

        float speed = SpeedManager.Instance.GetCurrentSpeed();

        if (speed < 7f)
        {
            return Random.Range(2f, 3f);
        }

        if (speed < 9f)
        {
            return Random.Range(1.7f, 2.5f);
        }

        if (speed < 11f)
        {
            return Random.Range(1.3f, 2f);
        }

        if (speed < 13f)
        {
            return Random.Range(1f, 1.6f);
        }

        return Random.Range(0.8f, 1.4f);
    }
}