using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    [SerializeField] private float destroyX = -12f;
    [SerializeField] private float speedMultiplier = 0.3f;

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying())
        {
            return;
        }

        float speed = SpeedManager.Instance.GetCurrentSpeed() * speedMultiplier;
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= destroyX)
        {
            Destroy(gameObject);
        }
    }
}