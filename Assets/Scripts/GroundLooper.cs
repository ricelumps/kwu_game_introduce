using UnityEngine;

public class GroundLooper : MonoBehaviour
{
    [SerializeField] private Transform[] groundTiles;
    [SerializeField] private float tileWidth = 18f;
    [SerializeField] private float leftLimitX = -18f;
    [SerializeField] private float speedMultiplier = 0.3f;

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying())
        {
            return;
        }

        float speed = SpeedManager.Instance.GetCurrentSpeed() * speedMultiplier;

        for (int i = 0; i < groundTiles.Length; i++)
        {
            groundTiles[i].position += Vector3.left * speed * Time.deltaTime;

            if (groundTiles[i].position.x <= leftLimitX)
            {
                MoveTileToRight(groundTiles[i]);
            }
        }
    }

    private void MoveTileToRight(Transform tile)
    {
        float rightMostX = groundTiles[0].position.x;

        for (int i = 1; i < groundTiles.Length; i++)
        {
            if (groundTiles[i].position.x > rightMostX)
            {
                rightMostX = groundTiles[i].position.x;
            }
        }

        tile.position = new Vector3(rightMostX + tileWidth, tile.position.y, tile.position.z);
    }
}