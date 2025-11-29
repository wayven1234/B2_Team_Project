using UnityEngine;

public class EnemyAttackEffect : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }

    public void PlayAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("AttackStart");
        }
    }
}
