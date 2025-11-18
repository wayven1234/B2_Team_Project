using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Constraints;

public class ItemPrefab : MonoBehaviour
{
    private PlayerController playerCnt;

    [SerializeField] private GameObject itemSelectPanel;
    [SerializeField] private ItemData data;
    Image image; 
    TMP_Text text;
 
    Weapon weapon;

    void Awake()
    {
        image = GetComponentsInChildren<Image>()[1];
        image.sprite = data.icon;
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = data.display;
    }

    public void OnClick()
    {
        playerCnt = PlayerController.instance;

        if (playerCnt == null)
            return;

        switch (data.type)
        {
            case ItemData.ItemType.Book:
                if(data.level == 0)
                {
                    GameObject newWeapon = new GameObject(data.display + "Weapon");  
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                }
                else
                {
                    float nextDamage = data.damages[data.level - 1];
                    float nextSpeed = data.speeds[data.level - 1];

                    if (weapon != null)
                        weapon.Upgrade(nextDamage, nextSpeed);
                }
                    break;
            case ItemData.ItemType.Talk:
                break;
            case ItemData.ItemType.Bar:
                break;
        }

        data.level++;
        playerCnt.currentLevel++;

        if (data.level == 4)
        {
            GetComponent<Button>().interactable = false;
        }
        itemSelectPanel.SetActive(false);
    }
}
