using UnityEngine;

public class SpeedManager : MonoBehaviour
{
    public static SpeedManager Instance { get; private set; }

    [Header("Speed Settings")]
    [SerializeField] private float startSpeed = 5f;
    [SerializeField] private float speedIncreasePerSecond = 0.05f;
    [SerializeField] private float maxSpeed = 15f;

    private float currentSpeed;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ResetSpeed();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPlaying())
        {
            UpdateSpeed();
        }
    }

    public void UpdateSpeed()
    {
        currentSpeed += speedIncreasePerSecond * Time.deltaTime;
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public void ResetSpeed()
    {
        currentSpeed = startSpeed;
    }
}