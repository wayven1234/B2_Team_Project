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

        if (spriteRenderer != null)
            spriteRenderer.sprite = data.icon;
        else
            Debug.LogWarning($"아이템 오브젝트 ({gameObject.name})에 SpriteRenderer 컴포넌트가 없습니다");
    }

    public void OnClick()
    {
        PlayerController playerCnt = PlayerController.instance;

        if (playerCnt == null)
            return;

        playerCnt.AddOrUpgradeWeapon(data);

        data.level++;
        playerCnt.currentLevel++;

        if (data.level >= data.maxLevel)
        {
            Button button = GetComponent<Button>();
            if (button != null)
                button.interactable = false;
        }

        itemSelectPanel.SetActive(false);
    }
}
