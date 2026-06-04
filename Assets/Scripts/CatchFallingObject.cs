using UnityEngine;

public class CatchFallingObject : MonoBehaviour
{
    [Header("Catch Result")]
    [SerializeField] private int catchScore = 100;
    [SerializeField] private bool gameOverWhenMissed = true;

    private bool isHandled;

    private void Update()
    {
        if (isHandled ||
            CatchGameManager.Instance == null ||
            CatchGameManager.Instance.IsGameOver)
        {
            return;
        }

        float fallSpeed = CatchGameManager.Instance.GetFallSpeed();
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isHandled || CatchGameManager.Instance == null)
        {
            return;
        }

        Debug.Log($"{name} 충돌 대상: {other.name}, Tag: {other.tag}");

        if (other.CompareTag("CatchZone"))
        {
            CatchGameManager.Instance.AddScore(catchScore);
            ConsumeObject();
        }
        else if (other.CompareTag("MissZone"))
        {
            if (gameOverWhenMissed)
            {
                CatchGameManager.Instance.GameOver();
            }

            ConsumeObject();
        }
    }

    private void ConsumeObject()
    {
        isHandled = true;

        // 충돌 직후 화면과 물리 판정에서 즉시 제거합니다.
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}