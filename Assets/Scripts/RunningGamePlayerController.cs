using UnityEngine;

public class RunningGamePlayerController : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpHoldForce = 18f;
    [SerializeField] private float maxJumpHoldTime = 0.2f;
    [SerializeField] private float maxRiseSpeed = 13f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Input Forgiveness")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    private Rigidbody2D rb;
    private Animator animator;

    private bool canControl = true;
    private bool isGrounded;
    private bool isJumpHolding;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private float jumpHoldTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!CanPlay())
        {
            return;
        }

        CheckGround();
        ReadJumpInput();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (!CanPlay())
        {
            return;
        }

        ApplyJumpHold();
    }

    private bool CanPlay()
    {
        if (!canControl)
        {
            return false;
        }

        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying())
        {
            return false;
        }

        return true;
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
    }

    private void ReadJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumpHolding = false;
        }

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        isJumpHolding = true;
        jumpHoldTimer = maxJumpHoldTime;

        jumpBufferTimer = 0f;
        coyoteTimer = 0f;

 //       if (AudioManager.Instance != null)
 //      {
 //           AudioManager.Instance.PlayJumpSound();
 //       }
    }

    private void ApplyJumpHold()
    {
        if (!isJumpHolding)
        {
            return;
        }

        if (!Input.GetKey(KeyCode.Space))
        {
            isJumpHolding = false;
            return;
        }

        if (jumpHoldTimer <= 0f)
        {
            isJumpHolding = false;
            return;
        }

        if (rb.linearVelocity.y < maxRiseSpeed)
        {
            rb.AddForce(Vector2.up * jumpHoldForce, ForceMode2D.Force);
        }

        jumpHoldTimer -= Time.fixedDeltaTime;
    }

    private void UpdateAnimation()
    {
        if (animator == null)
        {
            return;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Die();
        }
    }

    private void Die()
    {
        canControl = false;
        isJumpHolding = false;

        if (animator != null)
        {
            animator.speed = 0f;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}