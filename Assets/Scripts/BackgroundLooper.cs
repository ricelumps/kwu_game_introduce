using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    [SerializeField] private Transform[] backgroundTiles;
    [SerializeField] private float tileWidth = 25f;
    [SerializeField] private float leftLimitX = -25f;
    [SerializeField] private float speedMultiplier = 0.5f;

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying())
        {
            return;
        }

        float speed = SpeedManager.Instance.GetCurrentSpeed() * speedMultiplier;

        for (int i = 0; i < backgroundTiles.Length; i++)
        {
            backgroundTiles[i].position += Vector3.left * speed * Time.deltaTime;

            if (backgroundTiles[i].position.x <= leftLimitX)
            {
                MoveTileToRight(backgroundTiles[i]);
            }
        }
    }

    private void MoveTileToRight(Transform tile)
    {
        float rightMostX = backgroundTiles[0].position.x;

        for (int i = 1; i < backgroundTiles.Length; i++)
        {
            if (backgroundTiles[i].position.x > rightMostX)
            {
                rightMostX = backgroundTiles[i].position.x;
            }
        }

        tile.position = new Vector3(rightMostX + tileWidth, tile.position.y, tile.position.z);
    }
}