using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Constraints;

public class ItemPrefab : MonoBehaviour
{
    [SerializeField] private GameObject itemSelectPanel;
    [SerializeField] private ItemData data;
    Image image;
    TMP_Text text;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        Image[] images = GetComponentsInChildren<Image>();

        if (images.Length > 1)
        {
            image = images[1];
            image.sprite = data.icon;
        }

        text = GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = data.display;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = data.icon;
        }
    }

    public void OnClick()
    {
        PlayerController playerCnt = PlayerController.instance;

        if (playerCnt == null)
            return;

        playerCnt.AddOrUpgradeWeapon(data);

        data.level++;

        LevelUpPanelLogic panelLogic = itemSelectPanel.GetComponent<LevelUpPanelLogic>();

        if (panelLogic != null)
        {
            panelLogic.UpdateItemBar(data);
        }
        else
        {
            Debug.LogError("ItemPrefab: itemSelectPanel에서 LevelUpPanelLogic 컴포넌트를 찾을 수 없습니다. 연결을 확인해주세요.");
        }

        LevelUpPanelLogic.DecrementOpenCount();

        if (data.level >= data.maxLevel)
        {
            Button button = GetComponent<Button>();
            if (button != null)
                button.interactable = false;
        }

        // [핵심 수정] 남은 레벨업 기회 확인 및 패널 재활성화
        if (LevelUpPanelLogic.GetOpenCount() > 0)
        {
            // 남은 기회가 있으면 패널을 닫지 않고 즉시 다시 활성화하여 다음 선택 기회 제공
            itemSelectPanel.SetActive(true);
            Debug.Log("ItemPrefab: 다음 레벨업 기회가 남아 있어 패널을 유지합니다.");
        }
        else
        {
            // 남은 기회가 없으면 패널을 닫고 게임을 재개합니다.
            if (panelLogic != null)
                panelLogic.OnItemSelectFinish();
            else
                itemSelectPanel.SetActive(false);
        }
    }

    public ItemData GetData()
    {
        return data;
    }
}