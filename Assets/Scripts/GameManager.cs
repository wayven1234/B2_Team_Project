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
    private GameObject stageClearPanel;
    private GameObject gameOverPanel;
    private GameObject gameClearPanel;

    [Header("현재 게임 상태")]
    public GameState currentGameState;
    public int currentStageIndex = 1;

    [Header("플레이어 지속 데이터")]
    public int playerLevel = 1;
    public Dictionary<ItemData.ItemType, int> weaponLevels = new Dictionary<ItemData.ItemType, int>();
    public int playerItemCount = 0;
    public int playerExpCount = 0;
    public bool isFirstStageLoad = true;

    [Header("Retry 복구 지점 데이터")]
    private int savedPlayerLevel = 1;
    private Dictionary<ItemData.ItemType, int> savedWeaponLevels = new Dictionary<ItemData.ItemType, int>();
    private int savedPlayerCount = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
            Destroy(gameObject);

        LoadStageData(currentStageIndex);

        currentGameState = GameState.Paused;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (currentStageData != null)
        {
            //Debug.Log($"현재 스테이지 Type: {currentStageData.stageType} (Stage {currentStageIndex})");
        }

        //Debug.Log("GameManager: Start() 완료.");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TitleScene")
        {
            //Debug.Log("GameManager: TitleScene 로드 감지. 모든 DontDestroyOnLoad 오브젝트 파괴를 시작합니다.");
            CleanupPersistentObjects();
            return;
        }
        LoadStageData(currentStageIndex);
        InitializeTimeManager();

        Transform spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint")?.transform;
        if (InGameButtonManager.instance != null && spawnPoint != null)
        {
            InGameButtonManager.instance.SetPlayerSpawnPoint(spawnPoint);
        }

        if (currentStageIndex > 1 && InGameButtonManager.instance != null)
        {
            InGameButtonManager.instance.StartNextStageFlow();
        }

        if (InGameButtonManager.instance != null)
    {
        InGameButtonManager.instance.ResetAllPanelsAndState();
    }

        //Debug.Log($"GameManager: 씬 로드 완료. Stage {currentStageIndex} 데이터 및 시간 초기화 완료.");
    }

    private void CleanupPersistentObjects()
    {
        if (EnemyManager.instance != null)
            Destroy(EnemyManager.instance.gameObject);
        if (InGameButtonManager.instance != null)
            Destroy(InGameButtonManager.instance.gameObject);

        PersistentPanel persistentPanel = FindFirstObjectByType<PersistentPanel>();
        if (persistentPanel != null)
            Destroy(persistentPanel.gameObject);

        Destroy(gameObject);
    }

    /// <summary>
    /// 씬의 모든 루트 오브젝트와 그 자식까지 재귀적으로 탐색하여 비활성화된 오브젝트도 찾습니다.
    /// </summary>
    private GameObject FindObjectInSceneRecursively(Scene scene, string objectName)
    {
        GameObject[] rootObjects = scene.GetRootGameObjects();

        foreach (GameObject root in rootObjects)
        {
            if (root.name == objectName)
            {
                return root;
            }

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

        int arrayIndex = stageIndex - 1;

        if (arrayIndex >= 0 && arrayIndex < stageDatabase.stages.Length)
        {
            currentStageData = stageDatabase.stages[arrayIndex];
            //Debug.Log($"Stage {stageIndex} 데이터 로드 완료.");
            return true;
        }

        Debug.LogError($"Stage {stageIndex}의 데이터가 StageDatabase에 없습니다. (총 {stageDatabase.stages.Length}개)");
        return false;
    }

    public void ChangeState(GameState newState)
    {
        if (currentGameState == newState) return;

        currentGameState = newState;
        //Debug.Log("게임 상태 변경: " + newState);

        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;

                EnemySpawn enemySpawn = FindFirstObjectByType<EnemySpawn>();

                if (enemySpawn != null)
                {
                    //Debug.Log("asdfasdfasdfasdfasdfasdf");
                    enemySpawn.Initialize(currentStageData, currentStageIndex);
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

                if (newState == GameState.GameClear && InGameButtonManager.instance != null)
                    InGameButtonManager.instance.ShowGameClearPanel();
                else if (newState == GameState.GameOver && InGameButtonManager.instance != null)
                    InGameButtonManager.instance.ShowGameOverPanel();

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
            //enemySpawn.StopAllCoroutines();
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

        //Debug.Log("씬 오브젝트 정리 완료: Enemy, EXP, HealthItem, SkillItem 제거 및 EnemySpawn 정지.");
    }

    public StageData GetCurrentStageData()
    {
        return currentStageData;
    }

    /// <summary>
    /// 다음 스테이지 진입 전, 현재 플레이어 데이터를 복구 지점에 저장합니다.
    /// </summary>
    public void SaveCheckpointData()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
            player.SavePlayerData();

        savedPlayerLevel = playerLevel;
        savedPlayerCount = playerItemCount;

        savedWeaponLevels.Clear();
        foreach (var pair in weaponLevels)
            savedWeaponLevels[pair.Key] = pair.Value;

        //Debug.Log($"[Checkpoint Save] Stage {currentStageIndex} 클리어 데이터 백업 완료.");
    }

    /// <summary>
    /// 게임 오버 시 호출되어 복구 지점 데이터로 현재 데이터를 덮어씁니다.
    /// </summary>
    public void RestoreCheckpointData()
    {
        playerLevel = savedPlayerLevel;
        playerItemCount = savedPlayerCount;

        weaponLevels.Clear();
        foreach (var pair in savedWeaponLevels)
            weaponLevels.Add(pair.Key, pair.Value);

        //Debug.Log($"[Checkpoint Restore] Stage {currentStageIndex} 시작 상태로 데이터 복원 완료.");
    }

    public void HandleStageClear()
    {
        bool isFinalStage = (currentStageIndex == stageDatabase.stages.Length);

        SaveCheckpointData();

        if (isFinalStage)
        {
            ChangeState(GameState.GameClear);
        }
        else
        {
            ChangeState(GameState.StageClear);

            if (InGameButtonManager.instance != null)
            {
                InGameButtonManager.instance.ShowStageClearPanel();
            }
            else
            {
                Debug.LogError("InGameButtonManager를 찾을 수 없습니다. Stage Clear Panel을 띄울 수 없습니다.");
            }
        }
    }

    /// <summary>
    /// Retry 버튼 클릭 시 현재 씬을 복원된 데이터로 다시 로드합니다.
    /// </summary>
    public void StartRetryFlow()
    {
        RestoreCheckpointData();

        if (currentStageIndex == 1)
        {
            InGameButtonManager.ClearSelectedPlayerPrefab();
            LevelUpPanelLogic.ResetOpenCount(); // 추가로 LevelUpPanelLogic 카운트도 초기화합니다.

            // currentStageIndex는 이미 1이므로 변경할 필요는 없으나,
            // 혹시 모르니 Stage 1 데이터 재로드를 명시할 수도 있습니다.
            // LoadStageData(1); 
        }

        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);

        //Debug.Log($"[Retry] 씬 재시작 요청: {currentScene.name} (Stage {currentStageIndex} 복원)");
    }

    /// <summary>
    /// 현재 스테이지 인덱스를 기반으로 다음 씬을 계산하여 로드합니다.
    /// </summary>
    public void AdvanceToNextStageByCurrentIndex()
    {
        int nextStageNumber = currentStageIndex + 1;

        if (nextStageNumber <= stageDatabase.stages.Length)
        {
            string nextSceneName = "Stage" + nextStageNumber.ToString();

            currentStageIndex = nextStageNumber;
            SceneManager.LoadScene(nextSceneName);
            //Debug.Log($"씬 전환 요청: {nextSceneName}");
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
        TimeManager tm = FindFirstObjectByType<TimeManager>();

        if (tm != null)
        {
            tm.ResetTimer();
        }
        else
        {
            Debug.LogWarning("GameManager: TimeManager를 씬에서 찾을 수 없습니다. (다음 프레임에 생성될 수 있습니다)");
        }
    }
}