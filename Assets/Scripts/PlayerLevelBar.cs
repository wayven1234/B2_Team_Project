using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private float maxLevel;
    private float currentLevel;

    public void Init(float maxLevelValue)
    {
        maxLevel = maxLevelValue;
        currentLevel = 0f;
        UpdateBar();
    }

    public void SetHealth(float level)
    {
        currentLevel = Mathf.Clamp(level, 0, maxLevel);
        UpdateBar();
    }

    public void AddLevel(float amount)
    {
        currentLevel += amount;
        currentLevel = Mathf.Clamp(currentLevel, 0, maxLevel);
        UpdateBar();
    }

    private void UpdateBar()
    {
        if (maxLevel > 0)
            fillImage.fillAmount = currentLevel / maxLevel;
        else
            fillImage.fillAmount = 0f;
    }
}
