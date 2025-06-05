using UnityEngine;

public class PlayerSkull : MonoBehaviour
{
    public float attackRange = 0.5f;
    public int attackDamage = 1;
    public float attackRate = 1f;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    private float nextAttackTime = 0f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            // Z 키로 기본 공격
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Attack()
    {
        // 애니메이션 재생 (Attack 트리거)
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // 공격 판정
        if (attackPoint == null)
        {
            Debug.LogWarning("AttackPoint가 설정되지 않았습니다.");
            return;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            // EnemyScript(적 스크립트)에서 데미지 받는 함수가 TakeDamage라고 가정
            EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(attackDamage);
            }
        }
    }

    // 공격 범위 시각화 (에디터에서만)
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}