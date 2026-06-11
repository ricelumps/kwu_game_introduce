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

    [Header("Spawn Chance")]
    [Range(0f, 1f)]
    [SerializeField] private float groundObstacleChance = 0.7f;

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


    private ObstacleEntry GetRandomObstacleByLocation(
    ObstacleEntry[] obstacles,
    bool spawnInAir)
    {
        int matchingCount = 0;

        // 원하는 위치와 일치하는 장애물 개수를 확인합니다.
        for (int i = 0; i < obstacles.Length; i++)
        {
            if (obstacles[i] != null &&
                obstacles[i].prefab != null &&
                obstacles[i].spawnInAir == spawnInAir)
            {
                matchingCount++;
            }
        }

        if (matchingCount == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, matchingCount);

        for (int i = 0; i < obstacles.Length; i++)
        {
            if (obstacles[i] != null &&
                obstacles[i].prefab != null &&
                obstacles[i].spawnInAir == spawnInAir)
            {
                if (randomIndex == 0)
                {
                    return obstacles[i];
                }

                randomIndex--;
            }
        }

        return null;
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

        // 먼저 지상 또는 공중 장애물 중 무엇을 생성할지 결정합니다.
        bool spawnInAir = Random.value >= groundObstacleChance;

        ObstacleEntry selected =
            GetRandomObstacleByLocation(currentObstacles, spawnInAir);

        // 선택한 종류가 현재 Phase에 없다면 반대 종류를 선택합니다.
        if (selected == null)
        {
            selected = GetRandomObstacleByLocation(
                currentObstacles,
                !spawnInAir
            );
        }

        if (selected == null || selected.prefab == null)
        {
            Debug.LogWarning("생성 가능한 장애물 Prefab이 없습니다.");
            return;
        }

        Transform spawnPoint =
            selected.spawnInAir ? airSpawnPoint : groundSpawnPoint;

        if (spawnPoint == null)
        {
            Debug.LogWarning("장애물 SpawnPoint가 연결되지 않았습니다.");
            return;
        }

        Instantiate(
            selected.prefab,
            spawnPoint.position,
            Quaternion.identity
        );

        string location = selected.spawnInAir ? "공중" : "지상";

        Debug.Log(
            $"{elapsedTime:F1}초: {selected.prefab.name} ({location}) 생성"
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