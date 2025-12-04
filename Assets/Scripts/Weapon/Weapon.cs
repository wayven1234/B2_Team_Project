using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public ItemData currentData;

    public float currentSpeed;
    public float currentDamage;

    private GameObject bookPrefab;
    private GameObject talkPrefab;
    private GameObject barPrefab;

    private const float BOOK_RANGE = 2.5f;
    private const float BOOK_ANGLE = 90f;
    private const string ENEMY_TAG = "Enemy";

    private Vector2 lastMoveDirection = Vector2.right;

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
                    break;
                case ItemData.ItemType.Talk:
                    TalkAttack();
                    break;
                case ItemData.ItemType.Bar:
                    BarAttack();
                    break;
            }
            float cooltime = 1f / currentSpeed;

            float waitTime = cooltime > 0 ? cooltime : MIN_COOLDOWN_TIME;
            yield return new WaitForSeconds(waitTime);
        }
    }

    void BookAttack(float range, float angle)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.BookAttack);
        }
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

                if (bookEffectScript != null)
                {
                    bookEffectScript.BookSetupAttack(finalDamage, range, attackDirection);
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
            attackDirection = (nearestEnemy.position - player.transform.position).normalized;
        }
        else
        {
            attackDirection = player.GetLastMoveDirection();
        }

        float currentRange = currentData.baseRange +
                  (currentData.rangeIncreasePerLevel * currentData.level);
        currentRange = Mathf.Min(currentRange, currentData.maxRange);

        const float PROJECTILE_SPEED = 5f;
        float calculatedLifetime = currentRange / PROJECTILE_SPEED;

        if (talkPrefab != null)
        {
            float rotationZ = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;

            Vector3 spawnPosition = player.transform.position;

            GameObject projectile = Instantiate(
              talkPrefab,
              spawnPosition,
              Quaternion.Euler(0, 0, rotationZ + 180));

            TalkAttackEffect talkEffectScript = projectile.GetComponent<TalkAttackEffect>();
            if (talkEffectScript != null)
            {
                float finalDamage = currentDamage + player.GetSkillDamageBonus();

                talkEffectScript.TalkSetupAttack(finalDamage, attackDirection, calculatedLifetime);
            }
        }
        else
            Debug.LogError($"Error: {currentData.display}에 Talk 프리팹이 설정되지 않았습니다.");
    }

    void BarAttack()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.BarAttack);
        }
        PlayerController player = PlayerController.instance;
        if (player == null) return;

        const float BAR_RANGE = 25f;
        const float BAR_LIFETIME = 0.3f;
        const float BAR_TARGETING_RANGE = 25f;

        Transform nearestEnemy = GetNearestEnemy(BAR_TARGETING_RANGE);
        Vector2 attackDirection;

        if (nearestEnemy != null)
        {
            attackDirection = (nearestEnemy.position - player.transform.position).normalized;
        }
        else
        {
            Vector2 rawMoveDirection = player.GetLastMoveDirection();

            if (rawMoveDirection.sqrMagnitude > 0.01f)
            {
                lastMoveDirection = rawMoveDirection.normalized;
            }

            attackDirection = lastMoveDirection;
        }

        if (barPrefab != null)
        {
            Vector3 spawnPosition = player.transform.position + (Vector3)attackDirection;

            float rotationZ = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;

            float rotationOffset = -180f;

            float finalRotationZ = rotationZ + rotationOffset;

            GameObject effect = Instantiate(
                barPrefab,
                spawnPosition,
                Quaternion.Euler(0, 0, finalRotationZ));

            BarAttackEffect barEffectScript = effect.GetComponent<BarAttackEffect>();
            if (barEffectScript != null)
            {
                float finalDamage = currentDamage + player.GetSkillDamageBonus();

                barEffectScript.BarSetupAttack(finalDamage);

                CircleCollider2D collider = effect.GetComponent<CircleCollider2D>();
                if (collider != null)
                {
                    collider.radius = BAR_RANGE;
                }
            }

            Destroy(effect, BAR_LIFETIME);
        }
    }
}