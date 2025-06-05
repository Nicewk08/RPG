using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int maxHp = 3;
    private int currentHp;

    void Start()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 적 죽는 연출, 파괴 등
        Destroy(gameObject);
    }
}