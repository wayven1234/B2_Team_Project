using UnityEngine;
using UnityEngine.UI;

public class LevelUpPanelLogic : MonoBehaviour
{
    [SerializeField] private Button LevelUpPanelButton;

    [SerializeField] private Image talkProgressBar;
    [SerializeField] private Image bookProgressBar;
    [SerializeField] private Image barProgressBar;

    private static int panelOpenCount = 0;

    void OnEnable()
    {
        panelOpenCount++;

        if (panelOpenCount == 1)
        {
            LevelUpPanelButton.gameObject.SetActive(false);
        }
        else if (panelOpenCount > 1)
        {
            LevelUpPanelButton.gameObject.SetActive(true);
        }
    }

    public void UpdateItemBar(ItemData data)
    {
        // 1. Fill Amount 계산: (현재 레벨 / 최대 레벨)
        // data.maxLevel이 4일 때, 레벨 1 증가 시 fillAmount는 0.25 증가합니다.
        float fillAmount = (float)data.level / data.maxLevel;

        // 2. ItemType에 따라 올바른 Bar를 찾아 업데이트합니다.
        switch (data.type)
        {
            case ItemData.ItemType.Talk:
                if (talkProgressBar != null) talkProgressBar.fillAmount = fillAmount;
                break;
            case ItemData.ItemType.Book:
                if (bookProgressBar != null) bookProgressBar.fillAmount = fillAmount;
                break;
            case ItemData.ItemType.Bar:
                if (barProgressBar != null) barProgressBar.fillAmount = fillAmount;
                break;
            default:
                Debug.LogWarning("알 수 없는 아이템 타입입니다: " + data.type);
                break;
        }
    }

    public static void ResetOpenCount()
    {
        panelOpenCount = 0;
    }

    // [추가] ItemPrefab이 카운트에 접근할 수 있는 함수
    public static int GetOpenCount()
    {
        return panelOpenCount;
    }

    // [추가] ItemPrefab이 카운트를 감소시킬 수 있는 함수
    public static void DecrementOpenCount()
    {
        if (panelOpenCount > 0)
        {
            panelOpenCount--;
        }
    }
}
