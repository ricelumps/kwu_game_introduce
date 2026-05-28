using System.Collections;
using UnityEngine;

public class BonusItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] bonusItemPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float minY = -0.2f;
    [SerializeField] private float maxY = 1.8f;
    [SerializeField] private float minInterval = 3f;
    [SerializeField] private float maxInterval = 6f;

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
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            if (GameManager.Instance != null && GameManager.Instance.IsPlaying())
            {
                SpawnBonusItem();
            }
        }
    }

    private void SpawnBonusItem()
    {
        if (bonusItemPrefabs.Length == 0 || spawnPoint == null)
        {
            Debug.LogWarning("BonusItemSpawner 설정이 비어 있습니다.");
            return;
        }

        int index = Random.Range(0, bonusItemPrefabs.Length);
        Vector3 position = spawnPoint.position;
        position.y = Random.Range(minY, maxY);

        Instantiate(bonusItemPrefabs[index], position, Quaternion.identity);
    }
}