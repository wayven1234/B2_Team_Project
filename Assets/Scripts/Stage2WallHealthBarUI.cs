using UnityEngine;
using UnityEngine.UI;

public class Stage2WallHealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    private Stage2Wall targetWall;

    public void LinkToWall(Stage2Wall wall)
    {
        targetWall = wall;
        if (targetWall != null)
        {
            targetWall.OnHealthChanged += UpdateHealthBar;
            UpdateHealthBar(targetWall.GetCurrentHealth(), targetWall.GetMaxHealth());
        }
    }

    private void OnDisable()
    {
        // 오브젝트 비활성화 또는 파괴 시 이벤트 구독 해제 (누수 방지)
        if (targetWall != null)
        {
            targetWall.OnHealthChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }
}
