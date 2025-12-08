using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossEnemySpawn : MonoBehaviour
{
    public GameObject bossEnemyPrefab;

    [Header("맵 바운더리 설정 (보스 위치)")]
    [SerializeField] private BoxCollider2D mapBoundary;

    [Header("스폰 타이머 설정")]
    public float bossSpawnDelay = 150f;

    private Bounds mapBounds;
    private bool isBossSpawned = false;

    private void OnEnable()
    {
        Debug.Log("BossEnemySpawn: 오브젝트 활성화됨. 초기 설정 및 타이머 시작");
        InitializeBounds();
        StartBossSpawnTimer();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void InitializeBounds()
    {
        if (mapBoundary != null)
        {
            mapBounds = mapBoundary.bounds;
            Debug.Log($"BossEnemySpawn: 맵 바운더리 초기화 완료. 크기: {mapBounds.size}");
        }
        else
        {
            Debug.LogError("BossEnemySpawn: 맵 경계 (mapBoundary) Collider가 할당되지 않았습니다!");
        }
    }

    /// <summary>
    /// 보스 스폰 타이머를 시작합니다.
    /// </summary>
    public void StartBossSpawnTimer()
    {
        if (bossEnemyPrefab == null)
        {
            Debug.LogError("BossEnemySpawn: 보스 프리팹이 할당되지 않았습니다.");
            return;
        }
        if (isBossSpawned)
        {
            Debug.LogWarning("BossEnemySpawn: 보스는 이미 스폰되었습니다. 타이머 시작 건너뜀.");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(BossSpawnTimerCoroutine(bossSpawnDelay));
    }

    /// <summary>
    /// 보스 스폰 타이머 코루틴.
    /// </summary>
    IEnumerator BossSpawnTimerCoroutine(float delay)
    {
        isBossSpawned = true; // 타이머 시작 시점부터 true로 설정

        Debug.Log($"BossSpawnTimerCoroutine: 총 {delay}s 후에 보스 스폰 예정.");

        float timeElapsed = 0f;
        while (timeElapsed < delay)
        {
            // GameManager가 Playing 상태일 때만 시간을 세도록 합니다.
            if (GameManager.instance != null && GameManager.instance.currentGameState == GameState.Playing)
            {
                timeElapsed += Time.deltaTime;
            }
            yield return null;
        }

        SpawnBoss(bossEnemyPrefab);

        Debug.Log("Boss Spawned. BossSpawnTimerCoroutine 종료.");

        // 보스가 스폰된 후에는 더 이상 필요 없으므로 이 스크립트를 비활성화하거나 오브젝트를 파괴할 수 있습니다.
        // gameObject.SetActive(false);
    }

    /// <summary>
    /// 보스를 맵 내 랜덤 위치에 스폰합니다.
    /// </summary>
    void SpawnBoss(GameObject bossToSpawn)
    {
        if (mapBoundary == null)
        {
            Debug.LogError("Boss Spawn Failed: mapBoundary is null. Spawn canceled.");
            return;
        }

        float randomX = Random.Range(mapBounds.min.x, mapBounds.max.x);
        float randomY = Random.Range(mapBounds.min.y, mapBounds.max.y);

        Vector2 spawnPosition = new Vector2(randomX, randomY);

        GameObject newBoss = Instantiate(bossToSpawn, spawnPosition, Quaternion.identity);

        Debug.Log("Boss Spawned: " + bossToSpawn.name + " at " + spawnPosition);

        if (AudioManager.instance != null)
        {
            // BGMType.Boss 로 BGM 변경 요청
            AudioManager.instance.PlayBGM(BGMType.Boss, forceRestart: true, useFade: true);
        }
    }
}