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
        transform.Rotate(0, 0, ROTATION_SPEED * Time.deltaTime);
    }

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
