using System.Collections;
using UnityEngine;

public class CatchObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject plusObjectPrefab;
    [SerializeField] private GameObject minusObjectPrefab;

    [Header("Spawn Area")]
    [SerializeField] private float spawnXMin = -7.5f;
    [SerializeField] private float spawnXMax = 7.5f;
    [SerializeField] private float spawnY = 5.5f;

    [Range(0f, 1f)]
    [SerializeField] private float plusSpawnChance = 0.7f;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (CatchGameManager.Instance != null &&
                !CatchGameManager.Instance.IsGameOver)
            {
                SpawnObject();

                yield return new WaitForSeconds(
                    CatchGameManager.Instance.GetSpawnInterval()
                );
            }
            else
            {
                yield return null;
            }
        }
    }

    private void SpawnObject()
    {
        GameObject selectedPrefab =
            Random.value < plusSpawnChance
                ? plusObjectPrefab
                : minusObjectPrefab;

        if (selectedPrefab == null)
        {
            Debug.LogWarning("CatchObjectSpawner에 Prefab이 연결되지 않았습니다.");
            return;
        }

        Vector3 spawnPosition = new Vector3(
            Random.Range(spawnXMin, spawnXMax),
            spawnY,
            0f
        );

        Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
    }
}