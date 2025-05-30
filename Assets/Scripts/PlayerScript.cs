using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float moveSpeed;
    private Rigidbody2D rb;
    private Vector2 movement;

    // Jump related variables
    public float jumpForce = 10f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;
    public float groundCheckRadius = 0.1f;

    // Attack related variables
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 1;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;
    private float attackPointInitialX; // For flipping attack point

    public Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (attackPoint != null)
        {
            attackPointInitialX = attackPoint.localPosition.x; // Store initial X for flipping
        }
    }

    void Update()
    {
        // Horizontal Input
        movement.x = Input.GetAxisRaw("Horizontal");

        // Jump Input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            if (animator != null)
            {
                animator.SetBool("IsJumping", true);
                animator.SetBool("IsFalling", false);
            }
        }

        // Attack Input
        if (Input.GetButtonDown("Fire1"))
        {
            if (Time.time >= nextAttackTime)
            {
                PerformAttack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }

        // Animator parameters for jump/fall
        if (animator != null)
        {
            animator.SetFloat("MoveX", movement.x);
            animator.SetBool("IsMoving", Mathf.Abs(movement.x) > 0.01f);

            if (isGrounded) {
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", false);
            } else {
                if (rb.velocity.y > 0.01f) {
                    animator.SetBool("IsJumping", true);
                    animator.SetBool("IsFalling", false);
                } else if (rb.velocity.y < -0.01f) {
                    animator.SetBool("IsJumping", false);
                    animator.SetBool("IsFalling", true);
                } else {
                    if (!animator.GetBool("IsJumping"))
                    {
                         animator.SetBool("IsFalling", true);
                    }
                }
            }
        }

        // Sprite flipping and AttackPoint adjustment
        if (spriteRenderer != null)
        {
            bool flip = movement.x < -0.01f;
            bool unflip = movement.x > 0.01f;

            if (unflip)
            {
                spriteRenderer.flipX = false;
                if (attackPoint != null) {
                     attackPoint.localPosition = new Vector3(Mathf.Abs(attackPointInitialX), attackPoint.localPosition.y, attackPoint.localPosition.z);
                }
            }
            else if (flip)
            {
                spriteRenderer.flipX = true;
                 if (attackPoint != null) {
                     attackPoint.localPosition = new Vector3(-Mathf.Abs(attackPointInitialX), attackPoint.localPosition.y, attackPoint.localPosition.z);
                }
            }
        }
    }

    void FixedUpdate()
    {
        // Ground Check
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        } else {
            isGrounded = false;
        }

        // Apply horizontal movement
        rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
    }

    void PerformAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("AttackTrigger");
        }

        if (attackPoint == null) {
            Debug.LogError("AttackPoint not assigned on PlayerScript!");
            return;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemyCollider in hitEnemies)
        {
            EnemyScript enemy = enemyCollider.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            } else {
                // It's common to hit other colliders on the enemy layer that aren't the main enemy script
                // e.g. if the enemy has multiple colliders for different body parts.
                // So, a warning might be too noisy here unless specifically debugging.
                // Consider if a Debug.Log for successful hit on EnemyScript is more useful.
            }
        }
    }
}
