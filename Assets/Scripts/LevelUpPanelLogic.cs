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
        //Debug.Log($"LevelUpPanelLogic OnEnable: 현재 패널 카운트 = {panelOpenCount}");

        if (LevelUpPanelButton == null)
        {
            Debug.LogError("LevelUpPanelButton이 LevelUpPanelLogic에 연결되지 않았습니다! 인스펙터 연결을 확인하세요.");
            return;
        }

        if (panelOpenCount == 0)
        {
            LevelUpPanelButton.gameObject.SetActive(false);
        }
        else if (panelOpenCount > 0)
        {
            LevelUpPanelButton.gameObject.SetActive(true);
        }
    }

    public void UpdateItemBar(ItemData data)
    {
        float fillAmount = (float)data.level / data.maxLevel;

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
        //Debug.Log("LevelUpPanelLogic: ResetOpenCount 호출. Count가 0으로 초기화되었습니다.");
    }

    public static int GetOpenCount()
    {
        return panelOpenCount;
    }

    public static void DecrementOpenCount()
    {
        if (panelOpenCount > 0)
        {
            panelOpenCount--;
        }
    }

    /// <summary>
    /// 아이템 선택 완료 후 호출하여 패널을 닫고 시간을 재개합니다.
    /// </summary>
    public void OnItemSelectFinish()
    {
        gameObject.SetActive(false);

        if (GameManager.instance == null)
        {
            Time.timeScale = 1f;
        }
    }

    public static void IncreaseOpenCount()
    {
        panelOpenCount++;
        //Debug.Log($"LevelUpPanelLogic: IncreaseOpenCount 호출. 현재 Count: {panelOpenCount}");
    }
}