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

        // 1. 스프라이트 설정
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && effectSprite != null)
        {
            sr.sprite = effectSprite;
            sr.sortingOrder = ATTACK_EFFECT_ORDER;
        }

        // 2. 투사체 이동 시작
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
                // 관통 공격이므로 투사체를 파괴하지 않고 다음 적에게 이동
                Debug.Log($"[TALK HIT - PIERCE] {other.name} took {damageAmount} damage.");
            }
        }

        // 맵 끝 경계 등에 닿았을 때 투사체를 파괴하는 로직이 필요할 수 있습니다.
        // (예: if (other.CompareTag("Wall")) { Destroy(gameObject); })
    }
}
