using System.Collections;
using System.ComponentModel;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private xAxis horizontalAxis;
    [SerializeField] private yAxis verticalAxis;
    [SerializeField] private DashInput dashImput;
    [SerializeField] private JumpInput jumpInput;

    private float horizontal;
    private Vector2 direction;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;
    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;

    private bool canDash = true;
    public bool isDashing { get; set; } = false;
    private float dashingPower = 16f;
    private float dashingTime = 0.2f;
    public float dashingCooldown { get; } = 1f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.1f;
    private Vector2 wallJumpingPower = new Vector2(12f, 16f);

    public int percent { get; set; }
    private bool dead = false;

    [SerializeField] private SpriteRenderer sp;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform respawn;
    [SerializeField] private AudioSource au;

    public Animator animator;

    private bool dedAnimRunning = false;

    public void SetRespawn(Transform point)
    {
        respawn = point;
    }

    private void Start()
    {
        percent = 0;
        rb.transform.localPosition = new Vector2(respawn.transform.localPosition.x, respawn.transform.localPosition.y);
    }

    private void Update()
    {
        direction = new Vector2(Input.GetAxis(EnToStr.EnumString(horizontalAxis)), Input.GetAxis(EnToStr.EnumString(verticalAxis)));

        if (dead)
        {
            animator.SetFloat("MoveSpeed", 0f);
            rb.velocity = Vector3.zero;
            return;
        }
        if (isDashing)
        {
            return;
        }

        horizontal = Input.GetAxisRaw(EnToStr.EnumString(horizontalAxis));

        if (Input.GetButtonDown(EnToStr.EnumString(jumpInput)) && IsGrounded())
        {
            animator.SetFloat("MoveSpeed", 0f);
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
        if (IsGrounded() && !Input.GetButton(EnToStr.EnumString(jumpInput)))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }
        if (Input.GetButtonUp(EnToStr.EnumString(jumpInput)) && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if (!isWallJumping)
        {
            if (Input.GetButtonDown(EnToStr.EnumString(dashImput)) && canDash)
            {
                StartCoroutine(Dash());
            }
        }

        animator.SetFloat("VerticalSpeed", rb.velocity.y);

        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            Flip();
        }
        
        if (percent >= 300)
        {
            dead = true;
        }
    }


    private void FixedUpdate()
    {
        if (isDashing)
        {
            animator.SetFloat("MoveSpeed", 0f);
            return;
        }

        if (dead)
        {
            if (!dedAnimRunning)
            {
                au.Play();
                StartCoroutine(Respawn());
                dedAnimRunning = true;
            }
        }

        if (!isWallJumping && !dead)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            animator.SetFloat("MoveSpeed", Mathf.Pow(rb.velocity.x, 2));
        }
    }

    private IEnumerator Respawn()
    {
        animator.SetBool("IsDead", true);
        yield return new WaitForEndOfFrame();
        animator.Play("Explosion");
        animator.SetBool("IsDead", false);
        rb.gravityScale = 0f;
        yield return new WaitForSeconds(0.6f);
        percent = 0;
        sp.enabled = false;
        yield return new WaitForSeconds(1f);
        rb.transform.localPosition = new Vector2(respawn.transform.localPosition.x, respawn.transform.localPosition.y);
        rb.gravityScale = 4f;
        yield return new WaitForSeconds(1f);
        sp.enabled = true;
        rb.velocity = Vector2.zero;
        dead = false;
        dedAnimRunning = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Attack"))
        {
            percent += 15;
        }
        else if (collision.CompareTag("DeathZone"))
        {
            dead = true;
        }
        else if (collision.CompareTag("Respawn"))
        {
            respawn = collision.GetComponent<Transform>();
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            animator.SetFloat("MoveSpeed", 0f);
            isWallSliding = true;
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

        if (Input.GetButtonDown(EnToStr.EnumString(jumpInput)) && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
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

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = direction.normalized * dashingPower;
        rb.angularVelocity = 0f;
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}