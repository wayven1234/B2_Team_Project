using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private ItemData currentData;   // 무기 데이터 참조
    public float currentSpeed;
    public float currentDamage;

    private const float BOOK_RANGE = 2.5f;
    private const float BOOK_ANGLE = 90f;
    private const string ENEMY_TAG = "Enemy";

    public void Init(ItemData data)
    {
        currentData = data;
        currentDamage = data.baseDamage;
        currentSpeed = data.baseSpeed;

        StartCoroutine(WeaponSpawn());
    }

    public void Upgrade(float damage, float speed)
    {
        currentDamage = damage;
        currentSpeed = speed;
    }

    private IEnumerator WeaponSpawn()
    {
        while (true)
        {
            if (currentData == null)
                yield break;

            switch (currentData.type)
            {
                case ItemData.ItemType.Book:
                    BookAttack(BOOK_RANGE, BOOK_ANGLE);
                    // 책으로 때리는거 구현
                    //Debug.Log("ItemData.ItemType.Book : " + currentSpeed);
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

    void BookAttack(float range, float angle)
    {
        PlayerController player = PlayerController.instance;
        if (player == null) return;

        Vector2 attackDirection = player.GetLastMoveDirection();
        Vector2 playerPosition = transform.position;

        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(playerPosition, range);

        foreach (Collider2D collider in enemiesInRange)
        {
            if (collider.CompareTag(ENEMY_TAG))
            {
                Vector2 targetPosition = collider.transform.position;
                Vector2 directionToTarget = (targetPosition - playerPosition).normalized;

                float angleToEnemy = Vector2.Angle(attackDirection, directionToTarget);

                if (angleToEnemy <= angle / 2f)
                {
                    Enemy enemy = collider.GetComponent<Enemy>();

                    if (enemy != null)
                    {
                        float finalDamage = currentDamage + player.GetSkillDamageBonus();

                        // ? 이 부분이 실제로 호출되고 있는지 디버그로 확인합니다.
                        enemy.TakeDamage(finalDamage);

                        Debug.Log($"[WEAPON HIT] Book hit {collider.name} for {finalDamage} damage."); // <-- 이 로그가 출력되는지 확인!
                    }
                    // else: Enemy 컴포넌트를 찾지 못했다면 공격 실패!
                    else
                    {
                        Debug.LogWarning($"[WEAPON ERROR] {collider.name}에 Enemy 컴포넌트가 없습니다.");
                    }
                }
            }
        }
    }
}