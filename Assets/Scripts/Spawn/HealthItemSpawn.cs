using System.Collections;
using UnityEngine;

public class HealthItemSpawn : MonoBehaviour
{
    [Header("Health Item 설정")]
    public GameObject healthItemPrefab;

    [Tooltip("게임 시작 후 첫 스폰 시간")]
    [SerializeField] private float initialWaitBeforeFirstSpawn = 8f;
    
    [Tooltip("첫 스폰 이후 반복 스폰 시간")]
    [SerializeField] private float itemSpawnInterval = 20f;

    [Header("맵 경계 설정")]
    [SerializeField] private BoxCollider2D mapBoundary;

    private StageData.StageType currentStageType;
    private Bounds mapBounds;

    private void Start()
    {
        if (GameManager.instance == null) return;

        StageData stageData = GameManager.instance.GetCurrentStageData();
        if (stageData == null)
        {
            Debug.LogError("Stage Data를 GameManager에서 로드하지 못했습니다.");
            return;
        }

        currentStageType = stageData.stageType;

        if (mapBoundary != null)
        {
            mapBounds = mapBoundary.bounds;
            // 모든 스테이지에서 스폰을 시도합니다.
            StartCoroutine(SpawnController());
        }
        else
        {
            Debug.LogError("mapBoundary가 HealthItemSpawn에 할당되지 않았습니다. 아이템 스폰이 불가능합니다.");
        }
    }

    IEnumerator SpawnController()
    {
        while (GameManager.instance.currentGameState != GameState.Playing)
        {
            yield return null;
        }

        StartCoroutine(HealthItemSpawnLoop());
    }

    IEnumerator HealthItemSpawnLoop()
    {
        while (GameManager.instance.currentGameState != GameState.Playing)
        {
            yield return null;
        }

        yield return new WaitForSeconds(initialWaitBeforeFirstSpawn);

        SpawnHealthItem();

        while (true)
        {
            while (GameManager.instance.currentGameState != GameState.Playing)
            {
                yield return null;
            }

            yield return new WaitForSeconds(itemSpawnInterval);

            SpawnHealthItem();
        }
    }

    void SpawnHealthItem()
    {
        if (healthItemPrefab == null) return;

        float randomX = Random.Range(mapBounds.min.x, mapBounds.max.x);
        float ramfomY = Random.Range(mapBounds.min.y, mapBounds.max.y);

        Vector2 spawnPosition = new Vector2(randomX, ramfomY);

        Instantiate(healthItemPrefab, spawnPosition, Quaternion.identity);
    }
}
