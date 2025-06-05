using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float dashForce = 15f;
    public float dashDuration = 0.2f;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 1;
    public Animator animator;

    private Rigidbody2D rb;
    private bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.1f;

    private bool isDashing = false;
    private float dashTime = 0f;
    private float moveInput = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 방향키로 이동 입력 받기
        moveInput = Input.GetKey(KeyCode.LeftArrow) ? -1f : (Input.GetKey(KeyCode.RightArrow) ? 1f : 0f);

        // 점프
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isDashing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (animator != null)
            {
                animator.SetBool("IsJumping", true);
            }
        }

        // 대쉬 (Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && moveInput != 0)
        {
            isDashing = true;
            dashTime = dashDuration;
            rb.linearVelocity = new Vector2(moveInput * dashForce, rb.linearVelocity.y);
            if (animator != null)
            {
                animator.SetTrigger("Dash");
            }
        }

        // 공격 (D)
        if (Input.GetKeyDown(KeyCode.D) && !isDashing)
        {
            Attack();
        }

        // 스킬 1 (A)
        if (Input.GetKeyDown(KeyCode.A) && !isDashing)
        {
            UseSkill1();
        }

        // 스킬 2 (S)
        if (Input.GetKeyDown(KeyCode.S) && !isDashing)
        {
            UseSkill2();
        }

        // 애니메이터 이동값 업데이트
        if (animator != null)
        {
            animator.SetFloat("MoveX", moveInput);
            animator.SetBool("IsMoving", Mathf.Abs(moveInput) > 0.01f && isGrounded);
        }
    }

    void FixedUpdate()
    {
        // Ground Check
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        else
            isGrounded = false;

        // 대쉬 중엔 이동 입력 무시
        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            dashTime -= Time.fixedDeltaTime;
            if (dashTime <= 0f)
            {
                isDashing = false;
            }
        }
    }

    void Attack()
    {
        if (animator != null)
            animator.SetTrigger("Attack");
        if (attackPoint == null) return;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            var enemyScript = enemy.GetComponent<EnemyScript>();
            if (enemyScript != null)
                enemyScript.TakeDamage(attackDamage);
        }
    }

    void UseSkill1()
    {
        if (animator != null)
            animator.SetTrigger("Skill1");
        // TODO: 스킬1 효과 구현
    }

    void UseSkill2()
    {
        if (animator != null)
            animator.SetTrigger("Skill2");
        // TODO: 스킬2 효과 구현
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}