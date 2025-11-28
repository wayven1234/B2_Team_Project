using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BookAttackEffect : MonoBehaviour
{
    private float damageAmount;
    private const string ENEMY_TAG = "Enemy";
    private const float BOOK_ANGLE = 90f;

    private SpriteRenderer spriteRenderer;
    private Vector2 storedAttackDirection;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void BookSetupAttack(float damage, float range, Sprite effectSprite, Vector2 direction)
    {
        damageAmount = damage;

        storedAttackDirection = direction;

        if (spriteRenderer != null && effectSprite != null)
        {
            spriteRenderer.sprite = effectSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ENEMY_TAG))
        {
            Vector2 attackDirection = storedAttackDirection;

            Vector2 targetPosition = other.transform.position;
            Vector2 directionToTarget = (targetPosition - (Vector2)transform.position).normalized;

            float angleToEnemy = Vector2.Angle(attackDirection, directionToTarget);

            if (angleToEnemy <= BOOK_ANGLE / 2f)
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.TakeDamage(damageAmount);
            }
        }
    }
}
