using UnityEngine;

public class BarAttackEffect : MonoBehaviour
{
    private float damageAmount;
    private const string ENEMY_TAG = "Enemy";

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void BarSetupAttack(float damage)
    {
        damageAmount = damage;

        if (animator != null)
        {
            // 'Weapon_Bar' 애니메이션이 재생되도록 설정합니다.
            // animator.SetTrigger("StartSpin"); 
        }
    }

    //void Update()
    //{
    //    transform.Rotate(0, 0, ROTATION_SPEED * Time.deltaTime);
    //}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ENEMY_TAG))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);
                Debug.Log($"[BAR HIT - AOE] {other.name} took {damageAmount} damage.");
            }
        }
    }
}
