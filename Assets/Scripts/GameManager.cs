using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("스테이지 설정")]
    [SerializeField] private StageDatabase stageDatabase;
    private StageData currentStageData;

    [Header("UI 연결")]
    public GameObject stageClearPanel;

    [Header("현재 게임 상태")]
    public GameState currentGameState;
    public int currentStageIndex = 1;

    // [수정] 캐싱 변수 제거
    // private EnemySpawn enemySpawn;  

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        LoadStageData(currentStageIndex);

        // [수정] 초기 게임 상태를 명시적으로 Paused로 설정
        currentGameState = GameState.Paused;
    }

    private void Start()
    {
        if (currentStageData != null)
        {
            Debug.Log($"현재 스테이지 Type: {currentStageData.stageType} (Stage {currentStageIndex})");
        }

        Debug.Log("GameManager: Start() 완료.");

        if (stageClearPanel != null)
            stageClearPanel.SetActive(false);
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

                // [수정] TimeManager 초기화를 ChangeState 내에 통합
                InitializeTimeManager();

                EnemySpawn enemySpawn = FindFirstObjectByType<EnemySpawn>();

                if (enemySpawn != null)
                {
                    // 1. Stage Data를 EnemySpawn에 직접 전달하여 초기화합니다.
                    enemySpawn.Initialize(currentStageData);

                    // 2. 스폰 코루틴을 명시적으로 시작합니다.
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
        // 1. Enemy Spawn 정지 및 비활성화
        // [수정] 다시 FindFirstObjectByType 사용
        EnemySpawn enemySpawn = FindFirstObjectByType<EnemySpawn>();
        if (enemySpawn != null)
        {
            enemySpawn.StopAllCoroutines();
            enemySpawn.enabled = false; // 비활성화하여 다음 씬 로드 전까지 확실히 멈춥니다.
        }

        // 2. 태그 기반 씬 오브젝트 정리
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
        // 최종 스테이지 여부를 전체 스테이지 길이와 비교
        bool isFinalStage = (currentStageIndex == stageDatabase.stages.Length);

        if (isFinalStage)
        {
            ChangeState(GameState.GameClear);
        }
        else
        {
            ChangeState(GameState.StageClear);

            if (stageClearPanel != null)
            {
                stageClearPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("Stage Clear Panel이 GameManager에 연결되지 않았습니다. UI를 띄울 수 없습니다.");
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
    /// TimeManager를 씬에서 찾아 타이머를 초기화합니다.
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
            Debug.LogError("GameManager: TimeManager를 씬에서 찾을 수 없습니다.");
        }
    }
}