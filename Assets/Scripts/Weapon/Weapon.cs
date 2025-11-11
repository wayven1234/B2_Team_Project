using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    float currentSpeed;
    float currentDamage;

    public void Init(ItemData data)
    {
        currentDamage = data.baseDamage;
        currentSpeed = data.baseSpeed;
    }
    public void Upgrade(float damage, float speed)
    {
        
    }
    private IEnumerator WeaponSpawn(ItemData weaponType, float delay)
    {
        while (true)
        {
            switch (weaponType.type)
            {
                case ItemData.ItemType.Book:
                    break;
                case ItemData.ItemType.Tray:
                    break;
                case ItemData.ItemType.Bar:
                    break;
            }
            yield return new WaitForSeconds(delay);
        }
    }
}
