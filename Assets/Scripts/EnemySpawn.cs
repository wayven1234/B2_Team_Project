using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [Header("공통 설정")]
    public GameObject enemyPrefab;

    // [MODIFIELD] 이 변수는 Vertical Stage에서만 사용
    public float spawnInterval;

    [Header("Vertical Stage 설정")]
    public Transform[] spawnPoints;

    [Header("Normal Stage 설정")]
    [SerializeField] private BoxCollider2D mapBoundary;
    [SerializeField] private float noamalInitialDelay = 5f;
    [SerializeField] private float normalSpawnInterval = 2f;

    private StageData.StageType currentStageType;
    private Bounds mapBounds;

    private void Start()
    {
        if (GameManager.instance == null) return;

        currentStageType = GameManager.instance.GetStageType();

        if (currentStageType == StageData.StageType.Normal)
        {
            if (mapBoundary == null) return;
            mapBounds = mapBoundary.bounds;
        }

        StartCoroutine(SpawnController());
    }

    /// <summary>
    /// 게임 상태를 체크하며 스폰 타이밍을 관리하는 메인 코루틴
    /// </summary>
    IEnumerator SpawnController()
    {
        while (GameManager.instance.currentGameState != GameState.Playing)
        {
            yield return null;
        }

        if (currentStageType == StageData.StageType.Normal)
        {
            yield return new WaitForSeconds(noamalInitialDelay);

            StartCoroutine(SpawnLoop(normalSpawnInterval));
        }
        else
        {
            StartCoroutine(SpawnLoop(spawnInterval));
        }
    }

    /// <summary>
    /// 지정된 간격(interval)으로 적을 스폰하는 루프
    /// </summary>
    IEnumerator SpawnLoop(float interval)
    {
        while (true)
        {
            if (GameManager.instance.currentGameState == GameState.Playing)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(interval);
        }
    }

    void SpawnEnemy()
    {
        switch (currentStageType)
        {
            case StageData.StageType.Vertical:
                SpawnForVertical();
                break;
            case StageData.StageType.Normal:
                SpawnForNormal();
                break;
        }
    }

    void SpawnForVertical()
    {
        if (spawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
    }

    void SpawnForNormal()
    {
        if (mapBoundary == null) return;

        float randomX = Random.Range(mapBounds.min.x, mapBounds.max.x);
        float randomY = Random.Range(mapBounds.min.y, mapBounds.max.y);

        Vector2 spawnPosition = new Vector2(randomX, randomY);

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
