using UnityEngine;

public class BackgroundBlurController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] backgroundTiles;

    [Header("Blur Settings")]
    [SerializeField] private float minBlur = 0.001f;
    [SerializeField] private float maxBlur = 0.006f;
    [SerializeField] private float maxGameSpeed = 15f;

    private static readonly int BlurSizeId =
        Shader.PropertyToID("_BlurSize");

    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (SpeedManager.Instance == null)
        {
            return;
        }

        float currentSpeed =
            SpeedManager.Instance.GetCurrentSpeed();

        float speedRatio = Mathf.Clamp01(
            currentSpeed / maxGameSpeed
        );

        float blurAmount = Mathf.Lerp(
            minBlur,
            maxBlur,
            speedRatio
        );

        foreach (SpriteRenderer tile in backgroundTiles)
        {
            if (tile == null)
            {
                continue;
            }

            tile.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(BlurSizeId, blurAmount);
            tile.SetPropertyBlock(propertyBlock);
        }
    }
}