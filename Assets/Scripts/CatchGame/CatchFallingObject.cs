using UnityEngine;

public class CatchFallingObject : MonoBehaviour
{
    [Header("Catch Result")]
    [SerializeField] private int catchScore = 100;
    [SerializeField] private bool gameOverWhenMissed = true;

    [Header("Movement")]
    [SerializeField] private float speedMultiplier = 1f;

    [Tooltip("체크하면 획득 시 목숨을 1개 잃습니다.")]
    [SerializeField] private bool damagesPlayerWhenCaught;

    private bool isHandled;

    private void Update()
    {
        if (CatchGameManager.Instance == null ||
            !CatchGameManager.Instance.IsPlaying())
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

            if (damagesPlayerWhenCaught)
            {
                CatchGameManager.Instance.LoseLife();
                CatchGameManager.Instance.PlayMinusCatchSound();
            }
            else
            {
                CatchGameManager.Instance.AddScore(catchScore);
                CatchGameManager.Instance.PlayPlusCatchSound();

                CatchGameManager.Instance.ShowScorePopup(
                    catchScore,
                    transform.position
                );
            }

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