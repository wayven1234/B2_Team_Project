using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("스테이지 설정")]
    [SerializeField] private StageDatabase stageDatabase;
    private StageData currentStageData;

    [Header("UI 연결 (Scene 로직에 위임됨)")]
    private GameObject stageClearPanel; // private으로 유지

    [Header("현재 게임 상태")]
    public GameState currentGameState;
    public int currentStageIndex = 1;

    [Header("플레이어 지속 데이터")]
    public int playerLevel = 1;
    public Dictionary<ItemData.ItemType, int> weaponLevels = new Dictionary<ItemData.ItemType, int>();
    public int playerItemCount = 0;
    public bool isFirstStageLoad = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // 씬 로드 이벤트 구독
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
            Destroy(gameObject);

        LoadStageData(currentStageIndex);

        currentGameState = GameState.Paused;
    }

    private void OnDestroy()
    {
        // 오브젝트 파괴 시 이벤트 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (currentStageData != null)
        {
            Debug.Log($"현재 스테이지 Type: {currentStageData.stageType} (Stage {currentStageIndex})");
        }

        Debug.Log("GameManager: Start() 완료.");
    }

    // 씬 로드 완료 이벤트 핸들러: 다음 스테이지 데이터 및 시간 로드
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadStageData(currentStageIndex);
        InitializeTimeManager();
        Debug.Log($"GameManager: 씬 로드 완료. Stage {currentStageIndex} 데이터 및 시간 초기화 완료.");
    }

    /// <summary>
    /// 씬의 모든 루트 오브젝트와 그 자식까지 재귀적으로 탐색하여 비활성화된 오브젝트도 찾습니다.
    /// </summary>
    private GameObject FindObjectInSceneRecursively(Scene scene, string objectName)
    {
        // 씬의 모든 루트 오브젝트를 가져옵니다.
        GameObject[] rootObjects = scene.GetRootGameObjects();

        foreach (GameObject root in rootObjects)
        {
            // 루트 오브젝트 자체 검사
            if (root.name == objectName)
            {
                return root;
            }

            // 비활성화된 자식 오브젝트까지 포함하여 탐색합니다.
            Transform[] children = root.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in children)
            {
                if (child.name == objectName)
                {
                    return child.gameObject;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// StageIndex를 기반으로 StageData를 로드하고 currentStageData에 저장합니다.
    /// </summary>
    public bool LoadStageData(int stageIndex)
    {
        if (stageDatabase == null)
        {
            Debug.LogError("Stage Database가 연결되지 않았습니다!");
            return false;
        }

        // 배열 인덱스 (Stage 1 -> Index 0)
        int arrayIndex = stageIndex - 1;

        if (arrayIndex >= 0 && arrayIndex < stageDatabase.stages.Length)
        {
            currentStageData = stageDatabase.stages[arrayIndex];
            Debug.Log($"Stage {stageIndex} 데이터 로드 완료.");
            return true;
        }

        Debug.LogError($"Stage {stageIndex}의 데이터가 StageDatabase에 없습니다. (총 {stageDatabase.stages.Length}개)");
        return false;
    }

    public void ChangeState(GameState newState)
    {
        if (currentGameState == newState) return;

        currentGameState = newState;
        Debug.Log("게임 상태 변경: " + newState);

        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;

                // TimeManager 초기화는 OnSceneLoaded에서 처리됩니다.

                EnemySpawn enemySpawn = FindFirstObjectByType<EnemySpawn>();

                if (enemySpawn != null)
                {
                    enemySpawn.Initialize(currentStageData);
                    enemySpawn.StartSpawning();
                }
                else
                {
                    Debug.LogError("GameManager: EnemySpawn 컴포넌트를 찾을 수 없습니다! 적 스폰 실패.");
                }
                break;
            case GameState.StageClear:
            case GameState.GameClear:
            case GameState.GameOver:
                Time.timeScale = 0f;
                CleanupSceneObjects();
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
        }
    }

    /// <summary>
    /// 스테이지 클리어 또는 게임 오버 시 씬에 남아있는 모든 적, EXP, 아이템, 스킬 투사체를 제거하고 스폰을 중지합니다.
    /// </summary>
    void CleanupSceneObjects()
    {
        EnemySpawn enemySpawn = FindFirstObjectByType<EnemySpawn>();
        if (enemySpawn != null)
        {
            enemySpawn.StopAllCoroutines();
            enemySpawn.enabled = false;
        }

        string[] tagsToClean = { "Enemy", "EXP", "HealthItem", "SkillItem" };

        foreach (string tag in tagsToClean)
        {
            GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objectsToDestroy)
            {
                Destroy(obj);
            }
        }

        Debug.Log("씬 오브젝트 정리 완료: Enemy, EXP, HealthItem, SkillItem 제거 및 EnemySpawn 정지.");
    }

    public StageData GetCurrentStageData()
    {
        return currentStageData;
    }

    public void HandleStageClear()
    {
        bool isFinalStage = (currentStageIndex == stageDatabase.stages.Length);

        // 씬 이동 전에 플레이어 데이터 저장
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
            player.SavePlayerData();

        if (isFinalStage)
        {
            ChangeState(GameState.GameClear);
        }
        else
        {
            ChangeState(GameState.StageClear);

            // [수정] Stage Clear Panel을 띄우는 로직을 InGameButtonManager에게 위임
            InGameButtonManager buttonManager = FindFirstObjectByType<InGameButtonManager>();

            if (buttonManager != null)
            {
                // InGameButtonManager의 ShowStageClearPanel 함수를 호출합니다.
                buttonManager.ShowStageClearPanel();
            }
            else
            {
                // InGameButtonManager는 매 씬마다 새로 생성되므로, 이 로그는 심각한 오류입니다.
                Debug.LogError("InGameButtonManager를 찾을 수 없습니다. Stage Clear Panel을 띄울 수 없습니다.");
            }
        }
    }

    /// <summary>
    /// Stage Clear Panel에서 다음 스테이지 버튼을 눌렀을 때 호출됩니다.
    /// </summary>
    public void AdvanceToNextStage(string nextSceneName)
    {
        if (currentStageIndex < stageDatabase.stages.Length)
        {
            currentStageIndex++; // 스테이지 인덱스 증가
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Final stage reached. Cannot advance further.");
        }
    }

    /// <summary>
    /// TimeManager를 씬에서 찾아 타이머를 초기화합니다. (Stage 전환 시 호출)
    /// </summary>
    private void InitializeTimeManager()
    {
        // 씬 로드가 완료된 후 TimeManager를 찾아 초기화합니다.
        TimeManager tm = FindFirstObjectByType<TimeManager>();

        if (tm != null)
        {
            tm.ResetTimer();
        }
        else
        {
            // TimeManager가 아직 씬에 생성되지 않았을 수 있습니다. 경고로 처리합니다.
            Debug.LogWarning("GameManager: TimeManager를 씬에서 찾을 수 없습니다. (다음 프레임에 생성될 수 있습니다)");
        }
    }
}