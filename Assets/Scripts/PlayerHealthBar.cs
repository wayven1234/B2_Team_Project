using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private float maxHealth;
    private float currentHealth;

    public void Init(float health)
    {
        maxHealth = health;
        currentHealth = health;
        UpdateBar();
    }

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        UpdateBar();
    }

    private void UpdateBar()
    {
        fillImage.fillAmount = currentHealth / maxHealth;
    }
}
