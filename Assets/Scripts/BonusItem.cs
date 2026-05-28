using UnityEngine;

public class BonusItem : MonoBehaviour
{
    [SerializeField] private int bonusScore = 100;
    [SerializeField] private float destroyX = -12f;

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying())
        {
            return;
        }

        float speed = SpeedManager.Instance.GetCurrentSpeed();
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= destroyX)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        ScoreManager.Instance.AddBonusScore(bonusScore);
        Destroy(gameObject);
    }
}