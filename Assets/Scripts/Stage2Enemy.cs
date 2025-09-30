using Unity.VisualScripting;
using UnityEngine;

public class Stage2Enemy : MonoBehaviour
{
    public float moveSpeed;

    public float maxHealth;
    public float currentHealth;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        rb.linearVelocity = Vector2.left * moveSpeed;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
