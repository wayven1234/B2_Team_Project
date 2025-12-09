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
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.ButtonClick);
        }
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

        if (LevelUpPanelLogic.GetOpenCount() > 0)
        {
            itemSelectPanel.SetActive(true);
            //Debug.Log("ItemPrefab: 다음 레벨업 기회가 남아 있어 패널을 유지합니다.");
        }
        else
        {
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

    public void SetDataLevel(int savedLevel)
    {
        data.level = savedLevel;

        if (data.level >= data.maxLevel)
        {
            Button button = GetComponent<Button>();
            if (button != null)
            {
                button.interactable = false;
            }
        }
        else
        {
            Button button = GetComponent<Button>();
            if (button != null)
            {
                button.interactable = true;
            }
        }
    }
}