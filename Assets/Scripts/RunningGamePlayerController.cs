using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool canControl = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

        if (!canControl)
        {
            return;
        }

        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    private void Jump()
    {
        // 기존 y 속도를 0으로 만들고 위쪽 힘을 줍니다.
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        isGrounded = false;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("충돌 발생: " + collision.gameObject.name + " / Tag: " + collision.gameObject.tag);

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("장애물 충돌 감지");

            canControl = false;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
            else
            {
                Debug.LogWarning("GameManager.Instance가 없습니다.");
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            canControl = false;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    public void ResetControl()
    {
        canControl = true;
    }
}