using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawn : MonoBehaviour
{
    [Header("Vertical Stage 설정")]
    public Transform[] spawnPoints;

    [Header("Normal Stage 설정")]
    [SerializeField] private BoxCollider2D mapBoundary;

    private StageData currentStageData;
    private Bounds mapBounds;
    private int currentStageIndex = -1;

    public void Initialize(StageData stageData, int stageIndex)
    {
        if (GameManager.instance == null) return;

        currentStageData = stageData;
        currentStageIndex = stageIndex;

        if (currentStageData == null)
        {
            Debug.LogError("EnemySpawn Initialize: StageData가 null로 전달되었습니다. (Stage Index: " + GameManager.instance.currentStageIndex + ")");
            return;
        }

        Debug.Log("EnemySpawn Initialize: StageData 로드 성공.");

        if (mapBoundary != null)
        {
            mapBounds = mapBoundary.bounds;
            Debug.Log($"맵 바운더리 초기화 완료. 크기: {mapBounds.size}");
        }
        else
        {
            Debug.LogError("맵 경계 (mapBoundary) Collider가 EnemySpawn에 할당되지 않았습니다!");
        }
    }

    /// <summary>
    /// GameManager가 Playing 상태로 전환될 때 호출하여 스폰을 시작합니다.
    /// </summary>
    public void StartSpawning()
    {
        Debug.Log("EnemySpawn: StartSpawning 호출됨. 코루틴 시작.");
        StopAllCoroutines();
        StartCoroutine(SpawnController());
    }

    /// <summary>
    /// 게임 상태를 체크하며 스폰 타이밍을 관리하는 메인 코루틴
    /// </summary>
    public IEnumerator SpawnController()
    {
        if (currentStageData == null)
        {
            Debug.LogError("SpawnController: currentStageData가 null입니다. 스폰을 시작할 수 없습니다.");
            yield break;
        }

        Debug.Log("SpawnController: 코루틴 시작, 초기 딜레이 대기 시작.");

        float initialDelay = 0f;
        float spawnInterval = 0f;

        if (currentStageData.stageType == StageData.StageType.Normal)
        {
            initialDelay = currentStageData.normalInitialDelay;
            spawnInterval = currentStageData.normalSpawnInterval;

            if (currentStageIndex == 4 && currentStageData.bossEnemyPrefab != null)
            {
                Debug.Log($"Stage 4 (번호 {currentStageIndex})입니다. 보스 스폰 타이머 ({currentStageData.bossSpawnTime}s)를 시작합니다.");
                StartCoroutine(BossSpawnTimer(currentStageData.bossSpawnTime));
            }
        }
        else
        {
            initialDelay = currentStageData.verticalInitialDelay;
            spawnInterval = currentStageData.verticalSpawnInterval;
        }

        if (initialDelay > 0)
        {
            Debug.Log($"SpawnController: Initial Delay ({initialDelay}s) 대기 중.");
            yield return new WaitForSeconds(initialDelay);
        }

        Debug.Log("SpawnController: 초기 딜레이 완료. 반복 스폰 루프 시작.");

        StartCoroutine(SpawnLoop(spawnInterval));
    }

    /// <summary>
    /// Boss 스폰 타이머. 지정된 딜레이 후 Boss를 정확히 1회 스폰합니다.
    /// </summary>
    IEnumerator BossSpawnTimer(float delay)
    {
        if (currentStageData.bossEnemyPrefab == null) yield break;

        Debug.Log($"BossSpawnTimer: 총 {delay}s 후에 보스 스폰 예정.");

        float timeElapsed = 0f;
        while (timeElapsed < delay)
        {
            if (GameManager.instance.currentGameState == GameState.Playing)
            {
                timeElapsed += Time.deltaTime;
            }
            yield return null;
        }

        SpawnBoss(currentStageData.bossEnemyPrefab);
    }

    /// <summary>
    /// Normal Stage의 적 스폰 로직을 사용하여 보스를 맵 내 랜덤 위치에 스폰합니다.
    /// </summary>
    void SpawnBoss(GameObject bossToSpawn)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayBGM(BGMType.Boss, forceRestart: true, useFade: true);
        }
        if (mapBoundary == null)
        {
            Debug.LogError("Boss Spawn Failed: mapBoundary is null.");
            return;
        }

        float randomX = Random.Range(mapBounds.min.x, mapBounds.max.x);
        float randomY = Random.Range(mapBounds.min.y, mapBounds.max.y);

        Vector2 spawnPosition = new Vector2(randomX, randomY);

        GameObject newBoss = Instantiate(bossToSpawn, spawnPosition, Quaternion.identity);
        Enemy bossScript = newBoss.GetComponent<Enemy>();
        if (bossScript != null)
        {
            bossScript.currentStageType = currentStageData.stageType;
        }
        Debug.Log("Boss Spawned: " + bossToSpawn.name + " at " + spawnPosition);
    }

    /// <summary>
    /// 지정된 간격(interval)으로 적을 스폰하는 루프
    /// </summary>
    IEnumerator SpawnLoop(float interval)
    {
        Debug.Log($"SpawnLoop: {interval}s 간격으로 반복 스폰 시작.");
        while (true)
        {
            while (GameManager.instance.currentGameState != GameState.Playing)
            {
                yield return null;
            }

            SpawnEnemy();

            yield return new WaitForSeconds(interval);
        }
    }

    void SpawnEnemy()
    {
        if (currentStageData == null || currentStageData.enemies == null || currentStageData.enemies.Length == 0)
        {
            Debug.LogError("EnemySpawn: currentStageData가 설정되지 않았거나 스폰할 Enemy 목록이 비어 있습니다. (Spawn Failed)");
            return;
        }

        GameObject enemyToSpawn = SelectEnemyByChance(currentStageData.enemies);
        if (enemyToSpawn == null) return;

        switch (currentStageData.stageType)
        {
            case StageData.StageType.Vertical:
                SpawnForVertical(enemyToSpawn);
                break;
            case StageData.StageType.Normal:
                SpawnForNormal(enemyToSpawn);
                break;
        }
        Debug.Log("Enemy Spawned: " + enemyToSpawn.name);
    }

    void SpawnForVertical(GameObject enemyToSpawn)
    {
        if (spawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        GameObject newEnemy = Instantiate(enemyToSpawn, spawnPoint.position, Quaternion.identity);
        Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.currentStageType = currentStageData.stageType;
        }
    }

    void SpawnForNormal(GameObject enemyToSpawn)
    {
        if (mapBoundary == null) return;

        float randomX = Random.Range(mapBounds.min.x, mapBounds.max.x);
        float randomY = Random.Range(mapBounds.min.y, mapBounds.max.y);

        Vector2 spawnPosition = new Vector2(randomX, randomY);

        GameObject newEnemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.currentStageType = currentStageData.stageType;
        }
    }

    /// <summary>
    /// StageData에 설정된 확률을 기반으로 스폰할 Enemy를 선택합니다.
    /// </summary>
    GameObject SelectEnemyByChance(StageData.EnemySpawnData[] enemies)
    {
        if (enemies == null || enemies.Length == 0) return null;

        float totalChance = 0f;
        foreach (var enemy in enemies)
        {
            totalChance += enemy.spawnChance;
        }

        if (totalChance <= 0) return enemies[0].enemyPrefab;

        float randomValue = Random.Range(0f, totalChance);
        float currentSum = 0f;

        foreach (var enemy in enemies)
        {
            currentSum += enemy.spawnChance;
            if (randomValue < currentSum)
            {
                return enemy.enemyPrefab;
            }
        }
        return enemies[enemies.Length - 1].enemyPrefab;
    }
}