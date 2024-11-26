using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    internal int lifes;
    private bool permaDead = false;
    public int knockback = 8;
    internal Vector2 moveInput;
    private Vector2 direction;
    private float speed = 8f;
    private float jumpingPower = 16f;
    public bool isFacingRight = true;
    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;

    private bool dashInput;
    internal bool canDash;
    public bool isDashing { get; set; } = false;
    private float dashingPower = 16f;
    private Vector2 dashMovement;
    private float dashingTime = 0.2f;
    public float dashingCooldown { get; } = 1f;

    private bool jumpInput, stoppedJumping;
    private bool isWallJumping, canFlip;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.1f;
    private Vector2 wallJumpingPower = new Vector2(12f, 16f);

    public int percent;
    public bool dead = false;

    [HideInInspector] public bool attacked { get; set; } = false;

    [SerializeField] internal UserInput UserInput;
    [SerializeField] internal SpriteRenderer sp;
    [SerializeField] public Rigidbody2D rb { get; set; }
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform respawn;
    [SerializeField] private AudioSource au;

    public Animator animator;

    private bool dedAnimRunning = false;
    private bool checkStarted = false;
    public bool dummy = false;

    public void SetRespawn(Transform point)
    {
        respawn = point;
    }

    private void Start()
    {
        canDash = true;
        canFlip = true;
        lifes = 3;
        percent = 0;
        rb = GetComponent<Rigidbody2D>();
        rb.transform.localPosition = new Vector2(respawn.transform.localPosition.x, respawn.transform.localPosition.y);
    }

    private void Update()
    {
        if (dummy) return;
        if (permaDead) this.gameObject.SetActive(false);
        if (dead) { rb.velocity = Vector2.zero; return; }
        if (isDashing || attacked) return;

        moveInput = UserInput.moveInput.normalized;
        jumpInput = UserInput.jumpInput;
        stoppedJumping = UserInput.jumpReleaseInput;
        dashInput = UserInput.dashInput;

        // Jump logic
        if (IsGrounded() && jumpInput)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            canFlip = false;
        }
        if (!IsGrounded() && stoppedJumping && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }
        if (!IsGrounded())
        {
            animator.SetFloat("VerticalSpeed", rb.velocity.y);
        }
        if (!canFlip && IsGrounded() && !checkStarted)
        {
            StartCoroutine(CanFlip());
        }

        // Dash logic
        if (dashInput && canDash)
        {
            StartCoroutine(Dash());
        }

        WallSlide();
        WallJump();

        if (!isWallJumping && !attacked && canFlip)
        {
            Flip();
        }

        // Check for death condition
        if (percent >= 300)
        {
            dead = true;
        }
    }
    private void FixedUpdate()
    {
        if (dead)
        {
            rb.velocity = Vector2.zero;
            if (!dedAnimRunning)
            {
                GetComponent<CapsuleCollider2D>().enabled = false;
                au.Play();
                StartCoroutine(Respawn());
                dedAnimRunning = true;
            }
        }

        if (attacked)
        {
            StartCoroutine(Attacked());
            return;
        }

        if (isDashing)
        {
            animator.SetFloat("MoveSpeed", 0f);
            return;
        }


        if (!isWallJumping && !dead)
        {
            rb.velocity = new Vector2(moveInput.x * speed, rb.velocity.y);
            animator.SetFloat("MoveSpeed", Mathf.Pow(rb.velocity.x, 2));
        }
    }

    private IEnumerator Attacked()
    {
        yield return new WaitForSeconds(0.01f * knockback);
        attacked = false;
    }

    private IEnumerator Respawn()
    {
        lifes--;
        animator.SetBool("IsDead", true);
        yield return new WaitForEndOfFrame();
        animator.Play("Explosion");
        animator.SetBool("IsDead", false);
        yield return new WaitForSeconds(0.6f);
        percent = 0;
        knockback = 1;
        sp.enabled = false;
        float normalGrav = rb.gravityScale;
        rb.gravityScale = 0;
        yield return new WaitForSeconds(1f);
        rb.transform.localPosition = respawn.transform.localPosition;
        yield return new WaitForSeconds(1f);
        GetComponent<CapsuleCollider2D>().enabled = true;
        if (lifes <= 0)
        {
            permaDead = true;
            yield break;
        }
        sp.enabled = true;
        dead = false;
        dedAnimRunning = false;
        yield return new WaitForEndOfFrame();
        rb.gravityScale = normalGrav;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeathZone"))
        {
            dead = true;
        }
        else if (collision.CompareTag("Respawn"))
        {
            respawn = collision.GetComponent<Transform>();
        }
        else if (collision.CompareTag("Finish"))
        {
            Invoke(nameof(WinChange), 3);
        }
    }

    private void WinChange()
    {
        SceneManager.LoadScene("WinScreen");
    }

    private IEnumerator CanFlip()
    {
        checkStarted = true;
        yield return new WaitForSeconds(0.2f);
        if (IsGrounded() && !canFlip) { canFlip = true; }
        checkStarted = false;
    }

    internal bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && moveInput.x != 0f)
        {
            animator.SetFloat("MoveSpeed", 0f);
            isWallSliding = true;
            canFlip = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (jumpInput && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            canFlip = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            animator.SetFloat("VerticalSpeed", rb.velocity.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    public void Flip()
    {
        if (isFacingRight && moveInput.x < 0f || !isFacingRight && moveInput.x > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator Dash()
    {
        canFlip = true;
        canDash = false;
        float originalGravity = rb.gravityScale;
        dashMovement = moveInput * dashingPower;
        isDashing = true;
        rb.gravityScale = 0f;
        rb.angularVelocity = 0f;
        rb.velocity = dashMovement;
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}