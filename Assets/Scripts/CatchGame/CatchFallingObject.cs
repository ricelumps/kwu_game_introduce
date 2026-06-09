using UnityEngine;

public class CatchFallingObject : MonoBehaviour
{
    [Header("Catch Result")]
    [SerializeField] private int catchScore = 100;
    [SerializeField] private bool gameOverWhenMissed = true;

    [Header("Movement")]
    [SerializeField] private float speedMultiplier = 1f;

    private bool isHandled;

    private void Update()
    {
        if (isHandled ||
            CatchGameManager.Instance == null ||
            CatchGameManager.Instance.IsGameOver)
        {
            return;
        }

        float fallSpeed =
            CatchGameManager.Instance.GetFallSpeed() * speedMultiplier;

        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isHandled || CatchGameManager.Instance == null)
        {
            return;
        }

        if (other.CompareTag("CatchZone"))
        {
            isHandled = true;
            CatchGameManager.Instance.AddScore(catchScore);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("MissZone"))
        {
            isHandled = true;

            if (gameOverWhenMissed)
            {
                CatchGameManager.Instance.GameOver();
            }

            Destroy(gameObject);
        }
    }
}