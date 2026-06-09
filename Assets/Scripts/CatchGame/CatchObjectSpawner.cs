using System.Collections;
using UnityEngine;

public class CatchObjectSpawner : MonoBehaviour
{
    [Header("Plus Objects")]
    [SerializeField] private GameObject[] plusObjectPrefabs;

    [Header("Minus Objects")]
    [SerializeField] private GameObject[] minusObjectPrefabs;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnXMin = -7.5f;
    [SerializeField] private float spawnXMax = 7.5f;
    [SerializeField] private float spawnY = 5.5f;

    [Range(0f, 1f)]
    [SerializeField] private float plusSpawnChance = 0.7f;

    private Coroutine spawnRoutine;

    private void Start()
    {
        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (CatchGameManager.Instance != null &&
                !CatchGameManager.Instance.IsGameOver)
            {
                SpawnRandomObject();

                float interval =
                    CatchGameManager.Instance.GetSpawnInterval();

                yield return new WaitForSeconds(interval);
            }
            else
            {
                yield return null;
            }
        }
    }

    private void SpawnRandomObject()
    {
        bool spawnPlus = Random.value < plusSpawnChance;

        GameObject selectedPrefab = spawnPlus
            ? GetRandomPrefab(plusObjectPrefabs)
            : GetRandomPrefab(minusObjectPrefabs);

        if (selectedPrefab == null)
        {
            Debug.LogWarning("생성할 오브젝트 Prefab이 없습니다.");
            return;
        }

        Vector3 spawnPosition = new Vector3(
            Random.Range(spawnXMin, spawnXMax),
            spawnY,
            0f
        );

        Instantiate(
            selectedPrefab,
            spawnPosition,
            Quaternion.identity
        );
    }

    private GameObject GetRandomPrefab(GameObject[] prefabs)
    {
        if (prefabs == null || prefabs.Length == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, prefabs.Length);
        return prefabs[randomIndex];
    }
}