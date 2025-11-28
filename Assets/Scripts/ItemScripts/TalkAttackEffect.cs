using UnityEditor.AnimatedValues;
using UnityEngine;

public class TalkAttackEffect : MonoBehaviour
{
    private float damageAmount;
    private const string ENEMY_TAG = "Enemy";
    private const float PROJECTILE_SPEED = 5f;

    private Rigidbody2D rb;

    private const int ATTACK_EFFECT_ORDER = 92;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void TalkSetupAttack(float damage, Sprite effectSprite, Vector2 direction, float lifetime)
    {
        damageAmount = damage;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && effectSprite != null)
        {
            sr.sprite = effectSprite;
            sr.sortingOrder = ATTACK_EFFECT_ORDER;
        }

        if (rb != null)
        {
            rb.linearVelocity = direction * PROJECTILE_SPEED;
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
