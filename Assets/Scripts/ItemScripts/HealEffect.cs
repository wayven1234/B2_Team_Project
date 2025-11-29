using UnityEngine;

public class HealEffect : MonoBehaviour
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

    public void PlayHealAnimation()
    {
        if (animator != null)
        {
            // 예: "StartHeal"이라는 Trigger를 Animator Controller에 만들었다고 가정
            animator.SetTrigger("StartHeal");
            // 만약 기본 상태(Default State)로 자동 재생된다면 이 코드는 불필요합니다.
        }
    }
}
