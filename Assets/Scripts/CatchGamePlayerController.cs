using UnityEngine;
using UnityEngine.InputSystem;

public class CatchGamePlayerController : MonoBehaviour
{
    [SerializeField] private float fixedY = -3.5f;
    [SerializeField] private float minX = -7.5f;
    [SerializeField] private float maxX = 7.5f;
    [SerializeField] private float followSpeed = 15f;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (CatchGameManager.Instance != null &&
            CatchGameManager.Instance.IsGameOver)
        {
            return;
        }

        Vector3 mouseScreenPosition;

        if (Mouse.current == null)
        {
            return;
        }

        mouseScreenPosition = Mouse.current.position.ReadValue();

        mouseScreenPosition.z = Mathf.Abs(mainCamera.transform.position.z);

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        float targetX = Mathf.Clamp(mouseWorldPosition.x, minX, maxX);

        Vector3 targetPosition = new Vector3(
            targetX,
            fixedY,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );
    }
}
