using UnityEngine;
using UnityEngine.InputSystem;

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
    private RigidbodyConstraints2D initialConstraints;

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

        initialConstraints = rb.constraints;
    }

    public void ResetPlayer()
    {
        canControl = true;
        isDying = false;
        isJumpHolding = false;
        isGrounded = false;

        coyoteTimer = 0f;
        jumpBufferTimer = 0f;
        jumpHoldTimer = 0f;

        rb.constraints = initialConstraints;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = true;

        Collider2D playerCollider = GetComponent<Collider2D>();

        if (playerCollider != null)
        {
            playerCollider.enabled = true;
        }

        if (animator != null)
        {
            animator.speed = 1f;
            animator.ResetTrigger("Die");
        }
    }


    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log(
                $"Space 입력 / canControl: {canControl} / " +
                $"Playing: {GameManager.Instance?.IsPlaying()}"
            );
        }

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
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            jumpBufferTimer -= Time.deltaTime;
            isJumpHolding = false;
            return;
        }

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (keyboard.spaceKey.wasReleasedThisFrame)
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

        if (Keyboard.current == null || !Keyboard.current.spaceKey.isPressed)
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

    private bool isDying;

    private void Die()
    {
        if (isDying)
        {
            return;
        }

        isDying = true;
        canControl = false;
        isJumpHolding = false;

        // 충돌 후 캐릭터의 물리 이동을 멈춥니다.
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        Collider2D playerCollider = GetComponent<Collider2D>();

        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.BeginDeathSequence();
        }

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        else
        {
            // Animator가 없다면 즉시 게임 오버 처리
            OnDeathAnimationFinished();
        }
    }


    public void OnDeathAnimationFinished()
    {
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
