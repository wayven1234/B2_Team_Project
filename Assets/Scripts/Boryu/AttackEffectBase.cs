using UnityEngine;

public abstract class AttackEffectBase : MonoBehaviour
{
    protected float damageAmount;
    protected const string ENEMY_TAG = "Enemy";

    public void SetupAttack(float damage)
    {
        damageAmount = damage;
    }

    protected abstract void HandleHit(Collider2D other);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ENEMY_TAG))
        {
            HandleHit(other);
        }
    }
}