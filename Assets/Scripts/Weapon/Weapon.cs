using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.Hierarchy;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // private ItemData currentData;   // 무기 데이터 참조
    public ItemData currentData;

    public float currentSpeed;
    public float currentDamage;

    private GameObject bookPrefab;
    private GameObject talkPrefab;
    private GameObject barPrefab;

    private const float BOOK_RANGE = 2.5f;
    private const float BOOK_ANGLE = 90f;
    private const string ENEMY_TAG = "Enemy";

    public void Init(ItemData data)
    {
        currentData = data;
        currentDamage = data.baseDamage;
        currentSpeed = data.baseSpeed;

        bookPrefab = data.bookPrefab;
        talkPrefab = data.talkPrefab;
        barPrefab = data.barPrefab;

        StartCoroutine(WeaponSpawn());
    }

    public void Upgrade(float damage, float speed)
    {
        currentDamage = damage;
        currentSpeed = speed;
    }

    private IEnumerator WeaponSpawn()
    {
        const float MIN_COOLDOWN_TIME = 0.05f;

        while (true)
        {
            if (currentData == null)
                yield break;

            while (GameManager.instance != null && GameManager.instance.currentGameState != GameState.Playing)
                yield return null;

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
                    TalkAttack();
                    //Debug.Log("ItemData.ItemType.Talk : " + currentSpeed);
                    // 말로 때리는거 구현
                    break;
                case ItemData.ItemType.Bar:
                    BarAttack();
                    //Debug.Log("ItemData.ItemType.Bar : " + currentSpeed);
                    break;
            }
            float cooltime = 1f / currentSpeed;

            float waitTime = cooltime > 0 ? cooltime : MIN_COOLDOWN_TIME;
            yield return new WaitForSeconds(waitTime);
        }
    }

    void BookAttack(float range, float angle)
    {
        PlayerController player = PlayerController.instance;
        if (player == null) return;

        Transform nearestEnemy = GetNearestEnemy(range);

        Vector2 attackDirection;

        if (nearestEnemy != null)
        {
            attackDirection = (nearestEnemy.position - player.transform.position).normalized;
        }
        else
        {
             attackDirection = player.GetLastMoveDirection();
        }

        if (bookPrefab != null)
        {
            float rotationZ = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;

            Vector3 spawnPositon = player.transform.position;

            GameObject effect = Instantiate(
                bookPrefab,
                spawnPositon,
                Quaternion.Euler(0, 0, rotationZ - 90));

            BookAttackEffect bookEffectScript = effect.GetComponent<BookAttackEffect>();
            if (bookEffectScript != null)
            {
                float finalDamage = currentDamage + player.GetSkillDamageBonus();

                Sprite bookSprite = currentData.icon;

                if (bookEffectScript != null)
                {
                    bookEffectScript.BookSetupAttack(finalDamage, range, bookSprite, attackDirection);
                }
            }

            Destroy(effect, 0.2f);
        }
        else
            Debug.LogError($"Error: {currentData.display}에 아이템 프리팹이 설정되지 않았습니다.");
    }

    private Transform GetNearestEnemy(float range)
    {
        PlayerController player = PlayerController.instance;
        if (player == null) return null;

        Vector3 searchCenter = player.transform.position;

        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(searchCenter, range);
        Transform nearestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (Collider2D col in enemiesInRange)
        {
            if (col.CompareTag(ENEMY_TAG))
            {
                float distance = Vector2.Distance(searchCenter, col.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = col.transform;
                }
            }
        }
        return nearestEnemy;
    }

    void TalkAttack()
    {
        PlayerController player = PlayerController.instance;
        if (player == null) return;

        const float TALK_TARGETING_RANGE = 10f;

        Transform nearestEnemy = GetNearestEnemy(TALK_TARGETING_RANGE);

        Vector2 attackDirection;

        if (nearestEnemy != null)
        {
            // 2. 적이 있다면: 플레이어 -> 적 방향으로 공격 방향 설정
            attackDirection = (nearestEnemy.position - player.transform.position).normalized;
        }
        else
        {
            // 3. 주변에 적이 없다면: 플레이어가 바라보는 방향 사용
            attackDirection = player.GetLastMoveDirection();
        }

        float currentRange = currentData.baseRange +
                             (currentData.rangeIncreasePerLevel * currentData.level);
        currentRange = Mathf.Min(currentRange, currentData.maxRange);

        const float PROJECTILE_SPEED = 5f;
        float calculatedLifetime = currentRange / PROJECTILE_SPEED;

        if (talkPrefab != null)
        {
            // MathF.Atan2 대신 Mathf.Atan2를 사용해야 합니다. (using System이 아닌 UnityEngine.Mathf 사용)
            float rotationZ = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;

            Vector3 spawnPosition = player.transform.position;

            GameObject projectile = Instantiate(
                talkPrefab,
                spawnPosition,
                Quaternion.Euler(0, 0, rotationZ + 90)); // 스프라이트 방향에 따라 +90 또는 -90 조정

            TalkAttackEffect talkEffectScript = projectile.GetComponent<TalkAttackEffect>();
            if (talkEffectScript != null)
            {
                float finalDamage = currentDamage + player.GetSkillDamageBonus();
                Sprite talkSprite = currentData.icon;

                // 투사체 설정 함수 호출
                talkEffectScript.TalkSetupAttack(finalDamage, talkSprite, attackDirection, calculatedLifetime);
            }
        }
        else
            Debug.LogError($"Error: {currentData.display}에 Talk 프리팹이 설정되지 않았습니다.");
    }

    void BarAttack()
    {
        PlayerController player = PlayerController.instance;
        if (player == null) return;

        const float BAR_RANGE = 5f;
        const float BAR_LIFETIME = 0.3f;

        if (barPrefab != null)
        {
            Vector3 spawnPosition = player.transform.position;

            GameObject effect = Instantiate(
                barPrefab,
                spawnPosition,
                Quaternion.identity);

            BarAttackEffect barEffectScript = effect.GetComponent<BarAttackEffect>();
            if (barEffectScript != null)
            {
                float finalDamage = currentDamage + player.GetSkillDamageBonus();
                Sprite barSprite = currentData.icon; // ItemData의 icon을 사용

                barEffectScript.BarSetupAttack(finalDamage, barSprite);

                CircleCollider2D collider = effect.GetComponent<CircleCollider2D>();
                if (collider != null)
                {
                    collider.radius = BAR_RANGE;
                }
            }

            // 3. 이펙트 파괴
            Destroy(effect, BAR_LIFETIME);
        }

        //Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(player.transform.position, BAR_RANGE);

        //foreach (Collider2D collider in enemiesInRange)
        //{
        //    if (collider.CompareTag(ENEMY_TAG))
        //    {
        //        Enemy enemy = collider.GetComponent<Enemy>();
        //        if (enemy != null)
        //        {
        //            float finalDamage = currentDamage + player.GetSkillDamageBonus();
        //            enemy.TakeDamage(finalDamage);
        //        }
        //    }
        //}
    }
}