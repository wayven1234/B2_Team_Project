using UnityEngine;

public class BookAttackEffect : MonoBehaviour
{
    private float damageAmount;
    private const string ENEMY_TAG = "Enemy";
    private const float BOOK_ANGLE = 90f;

    private Animator animator;
    private Vector2 storedAttackDirection;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void BookSetupAttack(float damage, float range, Vector2 direction)
    {
        damageAmount = damage;

        storedAttackDirection = direction;

        if (animator != null)
        {
            // 예: BookAttackEffect의 Animator Controller에 "BookStart"라는 Trigger가 있다면
            // animator.SetTrigger("BookStart");
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
