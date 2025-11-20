using UnityEngine;

public class BarAttackEffect : MonoBehaviour
{
    private float damageAmount;
    private const string ENEMY_TAG = "Enemy";
    private const float ROTATION_SPEED = 720f;

    public void BarSetupAttack(float damage, Sprite effectSprite)
    {
        damageAmount = damage;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && effectSprite != null )
        {
            sr.sprite = effectSprite;
            sr.sortingOrder = 91;
        }
    }

    void Update()
    {
        // 1. 시각적 효과: 공격이 존재하는 동안 회전
        transform.Rotate(0, 0, ROTATION_SPEED * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ENEMY_TAG))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 2. 데미지 적용
                enemy.TakeDamage(damageAmount);
                Debug.Log($"[BAR HIT - AOE] {other.name} took {damageAmount} damage.");

                // 3. 중복 타격 방지 (AOE 관리)
                // OnTriggerEnter2D를 사용하면 한 번만 데미지를 주지만,
                // 다수의 적에게 동시에 데미지를 주는 것은 보장됩니다.
            }
        }
    }
}
