using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SpeedLineController : MonoBehaviour
{
    [Header("Emission")]
    [SerializeField] private float minEmission = 2f;
    [SerializeField] private float maxEmission = 10f;

    [Header("Line Speed")]
    [SerializeField] private float minLineSpeed = 10f;
    [SerializeField] private float maxLineSpeed = 25f;

    [Header("Game Speed")]
    [SerializeField] private float minGameSpeed = 5f;
    [SerializeField] private float maxGameSpeed = 15f;

    private ParticleSystem speedLines;
    private ParticleSystem.EmissionModule emission;
    private ParticleSystem.VelocityOverLifetimeModule velocity;

    private void Awake()
    {
        speedLines = GetComponent<ParticleSystem>();
        emission = speedLines.emission;
        velocity = speedLines.velocityOverLifetime;

        velocity.enabled = true;
    }

    private void Update()
    {
        bool isPlaying =
            GameManager.Instance != null &&
            GameManager.Instance.IsPlaying();

        if (!isPlaying)
        {
            if (speedLines.isPlaying)
            {
                speedLines.Stop(
                    true,
                    ParticleSystemStopBehavior.StopEmittingAndClear
                );
            }

            return;
        }

        if (!speedLines.isPlaying)
        {
            speedLines.Play();
        }

        if (SpeedManager.Instance == null)
        {
            return;
        }

        float gameSpeed = SpeedManager.Instance.GetCurrentSpeed();

        float ratio = Mathf.InverseLerp(
            minGameSpeed,
            maxGameSpeed,
            gameSpeed
        );

        emission.rateOverTime = Mathf.Lerp(
            minEmission,
            maxEmission,
            ratio
        );

        float lineSpeed = Mathf.Lerp(
            minLineSpeed,
            maxLineSpeed,
            ratio
        );

        velocity.x = new ParticleSystem.MinMaxCurve(-lineSpeed);
    }
}