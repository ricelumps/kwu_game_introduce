using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    [SerializeField] private Transform[] backgroundTiles;
    [SerializeField] private float speedMultiplier = 0.3f;

    private float tileWidth;
    private float cameraLeftX;

    private void Start()
    {
        SpriteRenderer renderer =
            backgroundTiles[0].GetComponent<SpriteRenderer>();

        tileWidth = renderer.bounds.size.x;

        float cameraHalfWidth =
            Camera.main.orthographicSize * Camera.main.aspect;

        cameraLeftX =
            Camera.main.transform.position.x - cameraHalfWidth;

        // 두 번째 배경을 첫 번째 배경 바로 오른쪽에 배치합니다.
        backgroundTiles[1].position = new Vector3(
            backgroundTiles[0].position.x + tileWidth,
            backgroundTiles[0].position.y,
            backgroundTiles[0].position.z
        );
    }

    private void Update()
    {
        if (GameManager.Instance != null &&
            !GameManager.Instance.IsPlaying())
        {
            return;
        }

        if (SpeedManager.Instance == null)
        {
            return;
        }

        float speed =
            SpeedManager.Instance.GetCurrentSpeed() * speedMultiplier;

        foreach (Transform tile in backgroundTiles)
        {
            tile.position += Vector3.left * speed * Time.deltaTime;

            float tileRightX = tile.position.x + tileWidth * 0.5f;

            // 배경의 오른쪽 끝까지 카메라 밖으로 나가면 재배치합니다.
            if (tileRightX < cameraLeftX)
            {
                MoveToRight(tile);
            }
        }
    }

    private void MoveToRight(Transform tile)
    {
        float rightMostX = backgroundTiles[0].position.x;

        foreach (Transform otherTile in backgroundTiles)
        {
            if (otherTile.position.x > rightMostX)
            {
                rightMostX = otherTile.position.x;
            }
        }

        tile.position = new Vector3(
            rightMostX + tileWidth,
            tile.position.y,
            tile.position.z
        );
    }
}