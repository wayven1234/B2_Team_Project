using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawn : MonoBehaviour
{
    [Header("공통 설정")]
    // public GameObject enemyPrefab;

    // [MODIFIELD] 이 변수는 Vertical Stage에서만 사용
    // public float spawnInterval;

    [Header("Vertical Stage 설정")]
    public Transform[] spawnPoints;

    [Header("Normal Stage 설정")]
    [SerializeField] private BoxCollider2D mapBoundary;
    //[SerializeField] private float noamalInitialDelay = 5f;
    //[SerializeField] private float normalSpawnInterval = 2f;

    private StageData currentStageData;
    private Bounds mapBounds;

    private void Start()
    {
        // Start는 GameManager가 제어하도록 비워둡니다.
    }

    /// <summary>
    /// GameManager로부터 StageData를 직접 전달받아 초기화합니다.
    /// </summary>
    public void Initialize(StageData stageData) // 매개변수 추가
    {
        if (GameManager.instance == null) return;

        // StageData 로드를 GameManager에 의존하지 않고 전달받아 즉시 사용
        currentStageData = stageData;

        if (currentStageData == null)
        {
            Debug.LogError("EnemySpawn Initialize: StageData가 null로 전달되었습니다. (Stage Index: " + GameManager.instance.currentStageIndex + ")");
            return;
        }

        // [디버그] 로드 성공 로그
        Debug.Log("EnemySpawn Initialize: StageData 로드 성공.");

        //if (currentStageData.stageType == StageData.StageType.Normal)
        //{
        //    if (mapBoundary == null) return;
        //    mapBounds = mapBoundary.bounds;
        //}
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

        // 2. 스테이지 타입에 맞는 딜레이와 간격 설정
        if (currentStageData.stageType == StageData.StageType.Normal)
        {
            initialDelay = currentStageData.normalInitialDelay;
            spawnInterval = currentStageData.normalSpawnInterval;
        }
        else // StageData.StageType.Vertical
        {
            initialDelay = currentStageData.verticalInitialDelay;
            spawnInterval = currentStageData.verticalSpawnInterval;
        }

        // 3. 첫 스폰까지의 초기 지연 시간 대기
        if (initialDelay > 0)
        {
            Debug.Log($"SpawnController: Initial Delay ({initialDelay}s) 대기 중.");
            yield return new WaitForSeconds(initialDelay);
        }

        Debug.Log("SpawnController: 초기 딜레이 완료. 반복 스폰 루프 시작.");

        if (currentStageData.isBossStage)
        {
            StartCoroutine(BossSpawnCoroutine(currentStageData.bossSpawnDelay, currentStageData.bossPrefab));
            Debug.Log($"SpawnController: 보스 스폰 코루틴 시작. {currentStageData.bossSpawnDelay}초 후 보스 등장 예정.");
        }

        // 4. 반복 스폰 루프 시작
        StartCoroutine(SpawnLoop(spawnInterval));
    }

    // [신규 함수] 보스 스폰 딜레이를 처리하는 코루틴
    IEnumerator BossSpawnCoroutine(float delay, GameObject bossPrefab)
    {
        if (bossPrefab == null)
        {
            Debug.LogError("BossSpawnCoroutine: Boss Prefab이 할당되지 않아 보스를 스폰할 수 없습니다.");
            yield break;
        }

        // 지정된 시간(150초) 대기
        yield return new WaitForSeconds(delay);

        // 게임 상태가 Playing일 때만 스폰
        if (GameManager.instance.currentGameState == GameState.Playing)
        {
            SpawnBoss(bossPrefab);
        }
        else
        {
            Debug.LogWarning("BossSpawnCoroutine: 게임이 Playing 상태가 아니어서 보스 스폰을 건너뛰었습니다.");
        }
    }

    // [신규 함수] 보스 스폰 로직
    void SpawnBoss(GameObject bossToSpawn)
    {
        Vector2 spawnPosition = Vector2.zero;

        // 1. [수정] Stage Type에 관계없이 Normal Stage의 랜덤 스폰 로직을 사용합니다.
        //    (보스 스폰 위치를 맵 경계 내 랜덤으로 통일)
        if (mapBoundary != null)
        {
            // 맵 경계 내 랜덤 위치에서 스폰
            float randomX = Random.Range(mapBounds.min.x, mapBounds.max.x);
            float randomY = Random.Range(mapBounds.min.y, mapBounds.max.y);
            spawnPosition = new Vector2(randomX, randomY);

            // 맵 경계 내에서 스폰되므로 spawnPoints는 사용하지 않습니다.
        }

        // 만약 Vertical Stage처럼 특정 포인트에서 스폰되기를 원한다면 아래 주석 해제
        /*
        else if (currentStageData.stageType == StageData.StageType.Vertical && spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            spawnPosition = spawnPoints[randomIndex].position;
        }
        */


        if (spawnPosition != Vector2.zero)
        {
            // 2. 보스 인스턴스화
            GameObject newBoss = Instantiate(bossToSpawn, spawnPosition, Quaternion.identity);
            Debug.Log($"[BOSS SPAWN] 보스가 성공적으로 스폰되었습니다: {bossToSpawn.name} at {spawnPosition}");

            // 3. Enemy 스크립트가 있다면 StageType 설정
            Enemy enemyScript = newBoss.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.currentStageType = currentStageData.stageType;
            }
        }
        else
        {
            // mapBoundary가 null일 경우
            Debug.LogError("Boss Spawn Failed: mapBoundary가 할당되지 않아 스폰 위치를 결정할 수 없습니다.");
        }
    }

    /// <summary>
    /// 지정된 간격(interval)으로 적을 스폰하는 루프
    /// </summary>
    IEnumerator SpawnLoop(float interval)
    {
        Debug.Log($"SpawnLoop: {interval}s 간격으로 반복 스폰 시작.");
        while (true)
        {
            // Paused 상태 처리를 위해 대기 루프 유지
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
        // currentStageData가 Initialize에서 로드되므로, 여기서 다시 null 체크를 합니다.
        if (currentStageData == null || currentStageData.enemies == null || currentStageData.enemies.Length == 0)
        {
            Debug.LogError("EnemySpawn: currentStageData가 설정되지 않았거나 스폰할 Enemy 목록이 비어 있습니다. (Spawn Failed)");
            return;
        }

        // 1. 스폰할 Enemy 선택 (확률 기반)
        GameObject enemyToSpawn = SelectEnemyByChance(currentStageData.enemies);
        if (enemyToSpawn == null) return; // 스폰할 적이 없으면 종료

        // 2. Stage Type에 맞는 스폰 로직 실행
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