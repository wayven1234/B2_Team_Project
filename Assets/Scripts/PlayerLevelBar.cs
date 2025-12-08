using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLevelBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI levelText;

    private float maxLevelValue;
    private float currentExp;

    private int maxPlayerLevel;

    public void Init(float expOrbsPerLevel, int maxPlayerLevel)
    {
        maxLevelValue = expOrbsPerLevel;
        this.maxPlayerLevel = maxPlayerLevel;
        currentExp = 0f;
        UpdateBar();
        UpdateLevelText(PlayerController.instance.currentLevel);
    }

    public void SetHealth(float expCount)
    {
        currentExp = Mathf.Clamp(expCount, 0, maxLevelValue);
        UpdateBar();
    }

    public void UpdateLevelText(int level)
    {
        if (levelText == null)
        {
            Debug.LogWarning("Level Text 컴포넌트가 PlayerLevelBar에 연결되지 않았습니다.");
            return;
        }

        if (level >= maxPlayerLevel)
        {
            levelText.text = "Lv.Max";
        }
        else
        {
            levelText.text = $"Lv.{level}";
        }
    }

    private void UpdateBar()
    {
        if (maxLevelValue > 0)
            fillImage.fillAmount = currentExp / maxLevelValue;
        else
            fillImage.fillAmount = 0f;
    }
}
