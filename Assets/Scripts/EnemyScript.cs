using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public GameObject deathEffect; // Optional: Assign a particle effect or similar prefab in the Inspector

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died!");

        // Instantiate death effect if assigned
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Destroy the enemy GameObject
        Destroy(gameObject);
    }
}
