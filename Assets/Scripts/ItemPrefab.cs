using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Constraints;

public class ItemPrefab : MonoBehaviour
{
    [SerializeField] private ItemData data;
    Image image; 
    TMP_Text text;
    int level;

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
        switch (data.type)
        {
            case ItemData.ItemType.Book:
                if(level == 0)
                {
                    GameObject newWeapon = new GameObject();  
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                }
                else
                {
                    float nextDamage = data.damages[level];
                    float nextSpeed = data.speeds[level];


                }
                    break;
            case ItemData.ItemType.Tray:
                break;
            case ItemData.ItemType.Bar:
                break;
        }
        level++;
        if (level == data.damages.Length)
        {
            this.gameObject.SetActive(false);
        }
    }
}
