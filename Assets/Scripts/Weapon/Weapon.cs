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
        StartCoroutine(WeaponSpawn(data));

    }

    public void Upgrade(float damage, float speed)
    {

    }

    private IEnumerator WeaponSpawn(ItemData weaponType)
    {
        while (true)
        {
            switch (weaponType.type)
            {
                case ItemData.ItemType.Book:
                    // 책으로 때리는거 구현
                    Debug.Log("ItemData.ItemType.Book : " + currentSpeed);

                    //// ObjectFindByTag ("Player"); [플레이어 1개만 가져옴]
                    //// ObjectsFindByTags("Enemy"); [몬스터 태그를 가진 전체를 리스트로 가져옴]
                    //for(int i = 0; i < enemy.count; i++)
                    //{
                    //    float d = Vector2.Distance(player.transfrom.position, enemy[i].transfrom.position);
                    //    if( d < "아이템의 사정거리라면")
                    //    {
                    //        "공격구현;"
                    //    }
                    //}
                    break;
                case ItemData.ItemType.Talk:

                    Debug.Log("ItemData.ItemType.Talk : " + currentSpeed);
                    // 말로 때리는거 구현
                    break;
                case ItemData.ItemType.Bar:

                    Debug.Log("ItemData.ItemType.Bar : " + currentSpeed);
                    break;
            }
            yield return new WaitForSeconds(currentSpeed);
        }
    }
}