using UnityEditor.AnimatedValues;
using UnityEngine;

public class TalkAttackEffect : MonoBehaviour
{
    private float damageAmount;
    private const string ENEMY_TAG = "Enemy";
    private const float PROJECTILE_SPEED = 5f;

    private Rigidbody2D rb;
    private Animator animator;

    private const int ATTACK_EFFECT_ORDER = 92;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void TalkSetupAttack(float damage, Vector2 direction, float lifetime)
    {
        damageAmount = damage;

        if (rb != null)
        {
            rb.linearVelocity = direction * PROJECTILE_SPEED;
        }

        if (animator != null)
        {
            animator.SetTrigger("StartAttack");
        }

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ENEMY_TAG))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);
                Debug.Log($"[TALK HIT - PIERCE] {other.name} took {damageAmount} damage.");
            }
        }
    }
}
