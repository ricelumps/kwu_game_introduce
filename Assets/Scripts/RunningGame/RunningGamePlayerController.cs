using UnityEngine;
using UnityEngine.InputSystem;

public class RunningGamePlayerController : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpHoldForce = 18f;
    [SerializeField] private float maxJumpHoldTime = 0.2f;
    [SerializeField] private float maxRiseSpeed = 13f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField, Range(0f, 1f)] private float jumpSoundVolume = 1f;

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
    private bool isDying;

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

        UpdateAnimatorPlayback();

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

    private bool WasJumpPressedThisFrame()
    {
        bool spacePressed =
            Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame;

        bool mousePressed =
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame;

        return spacePressed || mousePressed;
    }

    private bool WasJumpReleasedThisFrame()
    {
        bool spaceReleased =
            Keyboard.current != null &&
            Keyboard.current.spaceKey.wasReleasedThisFrame;

        bool mouseReleased =
            Mouse.current != null &&
            Mouse.current.leftButton.wasReleasedThisFrame;

        return spaceReleased || mouseReleased;
    }

    private bool IsJumpHeld()
    {
        bool spaceHeld =
            Keyboard.current != null &&
            Keyboard.current.spaceKey.isPressed;

        bool mouseHeld =
            Mouse.current != null &&
            Mouse.current.leftButton.isPressed;

        return spaceHeld || mouseHeld;
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
        if (WasJumpPressedThisFrame())
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        // Space와 마우스를 모두 놓았을 때 홀드 점프를 종료합니다.
        if (WasJumpReleasedThisFrame() && !IsJumpHeld())
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
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpForce
        );

        isJumpHolding = true;
        jumpHoldTimer = maxJumpHoldTime;

        jumpBufferTimer = 0f;
        coyoteTimer = 0f;

        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(
                jumpSound,
                jumpSoundVolume
            );
        }
    }

    private void ApplyJumpHold()
    {
        if (!isJumpHolding)
        {
            return;
        }

        if (!IsJumpHeld() || jumpHoldTimer <= 0f)
        {
            isJumpHolding = false;
            return;
        }

        if (rb.linearVelocity.y < maxRiseSpeed)
        {
            rb.AddForce(
                Vector2.up * jumpHoldForce,
                ForceMode2D.Force
            );
        }

        jumpHoldTimer -= Time.fixedDeltaTime;
    }


    private void UpdateAnimatorPlayback()
    {
        if (animator == null)
        {
            return;
        }

        bool shouldAnimate =
            isDying ||
            (GameManager.Instance != null &&
             GameManager.Instance.IsPlaying());

        animator.speed = shouldAnimate ? 1f : 0f;
    }

    private void UpdateAnimation()
    {
        if (animator == null)
        {
            return;
        }

        bool isPlaying =
            GameManager.Instance != null &&
            GameManager.Instance.IsPlaying();

        animator.speed = isPlaying ? 1f : 0f;

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
            animator.speed = 1f;
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
