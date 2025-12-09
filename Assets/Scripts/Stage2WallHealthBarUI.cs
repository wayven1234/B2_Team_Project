using UnityEngine;
using UnityEngine.UI;
using System;

public class Stage2WallHealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private Stage2Wall targetWall;

    public void LinkToWall(Stage2Wall wall)
    {
        if (targetWall != null)
        {
            targetWall.OnHealthChanged -= UpdateHealthBar;
        }

        targetWall = wall;
        if (targetWall != null)
        {
            targetWall.OnHealthChanged += UpdateHealthBar;

            UpdateHealthBar(targetWall.GetCurrentHealth(), targetWall.GetMaxHealth());
        }
    }

    private void OnDisable()
    {
        if (targetWall != null)
        {
            targetWall.OnHealthChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (fillImage != null && maxHealth > 0)
        {
            fillImage.fillAmount = currentHealth / maxHealth;
        }
    }
}
