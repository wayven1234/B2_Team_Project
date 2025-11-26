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
        // 디버그로 현재 카운트 확인
        Debug.Log($"LevelUpPanelLogic OnEnable: 현재 패널 카운트 = {panelOpenCount}");

        if (LevelUpPanelButton == null)
        {
            Debug.LogError("LevelUpPanelButton이 LevelUpPanelLogic에 연결되지 않았습니다! 인스펙터 연결을 확인하세요.");
            return;
        }

        // 1개 이하일 때는 버튼 숨김 (강제 선택 유도)
        if (panelOpenCount <= 1)
        {
            LevelUpPanelButton.gameObject.SetActive(false);
            Debug.Log($"LevelUpPanelButton: 비활성화 (Count: {panelOpenCount})");
        }
        // 2개 이상일 때는 버튼 활성화 (나중에 선택 가능)
        else
        {
            LevelUpPanelButton.gameObject.SetActive(true);
            Debug.Log($"LevelUpPanelButton: 활성화 (Count: {panelOpenCount})");
        }
    }

    public void UpdateItemBar(ItemData data)
    {
        // 1. Fill Amount 계산: (현재 레벨 / 최대 레벨)
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

    // ItemPrefab이 카운트에 접근할 수 있는 함수
    public static int GetOpenCount()
    {
        return panelOpenCount;
    }

    // ItemPrefab이 카운트를 감소시킬 수 있는 함수
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

        // GameManager의 Update()가 모든 패널 상태를 감지하고 Playing으로 전환하도록 위임
        if (GameManager.instance == null)
        {
            Time.timeScale = 1f; // 안전 장치
        }
    }

    public static void IncreaseOpenCount()
    {
        panelOpenCount++;
        Debug.Log($"LevelUpPanelLogic: IncreaseOpenCount 호출. 현재 Count: {panelOpenCount}");
    }
}