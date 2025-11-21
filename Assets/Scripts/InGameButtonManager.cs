using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections; // 코루틴 사용

public class InGameButtonManager : MonoBehaviour
{
    public static InGameButtonManager instance { get; private set; }

    private static GameObject selectedPlayerPrefab = null;

    [Header("Player Prefabs & Spawn")]
    [SerializeField] private GameObject girlPlayerPrefab;
    [SerializeField] private GameObject boyPlayerPrefab;
    [SerializeField] private Transform playerSpawnPoint;

    // Esc
    [SerializeField] private GameObject escPanel;    // Esc 창
    // Setting
    [SerializeField] private GameObject setPanel;    // 설정창

    [SerializeField] private GameObject characterSelectionPanel;
    [SerializeField] private GameObject itemSelectionPanel;
    [SerializeField] private GameObject itemLevelUpPanel;

    [Header("게임 종료/클리어 패널")]
    [SerializeField] private GameObject stageClearPanel;

    public string nextSceneName;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        Debug.Log("InGameButtonManager: Awake 함수 실행 시작.");

        // [수정] 모든 패널 초기 비활성화 로직을 Awake로 이동
        escPanel.SetActive(false);
        setPanel.SetActive(false);
        characterSelectionPanel.SetActive(false);
        itemSelectionPanel.SetActive(false);
        itemLevelUpPanel.SetActive(false);

        if (GameManager.instance == null)
        {
            Debug.LogError("InGameButtonManager: GameManager 인스턴스를 찾을 수 없습니다. (Critical Error)");
            return;
        }

        // [수정] Awake에서 코루틴을 시작하여 Start()보다 먼저 실행되도록 강제
        StartCoroutine(StageStartFlow());
    }

    // [수정] Start() 함수를 제거했습니다.

    private void Update()
    {
        if (GameManager.instance == null) return;

        // Stage Clear/Game Over 상태에서는 입력 무시
        if (GameManager.instance.currentGameState == GameState.GameOver ||
        GameManager.instance.currentGameState == GameState.GameClear ||
        GameManager.instance.currentGameState == GameState.StageClear)
        {
            return;
        }

        // Esc 키 처리 (일시 정지)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escPanel.SetActive(!escPanel.activeSelf);    // Esc 키를 누르면 Esc 창 토글
            setPanel.SetActive(false);                   // Esc 창이 켜질 때 설정창 끄기
        }

        // 현재 켜져 있는 패널이 있는지 확인
        bool isAnyPausePanelActive = escPanel.activeSelf ||
                                     setPanel.activeSelf ||
                                     characterSelectionPanel.activeSelf ||
                                     itemSelectionPanel.activeSelf ||
                                     itemLevelUpPanel.activeSelf;

        if (isAnyPausePanelActive)
        {
            if (GameManager.instance.currentGameState != GameState.Paused)
            {
                GameManager.instance.ChangeState(GameState.Paused);
            }
        }
        else
        {
            // 위의 패널이 *모두* 꺼져 있을 때만 Playing 상태로 변경
            if (GameManager.instance.currentGameState != GameState.Playing)
            {
                GameManager.instance.ChangeState(GameState.Playing);
            }
        }
    }

    // [수정] 스테이지 시작 흐름을 관리하는 코루틴
    IEnumerator StageStartFlow()
    {
        // [추가] GameManager가 Awake를 끝낼 때까지 한 프레임 대기 (안전장치)
        yield return null;

        if (GameManager.instance == null)
        {
            Debug.LogError("StageStartFlow: GameManager 인스턴스를 찾을 수 없습니다.");
            yield break;
        }

        // 이전에 선택된 캐릭터가 있는지 확인 (씬 전환 시 유지)
        if (selectedPlayerPrefab == null)
        {
            // 1. Stage 1 (첫 진입): 
            characterSelectionPanel.SetActive(true);
            GameManager.instance.ChangeState(GameState.Paused);
            Debug.Log("InGameButtonManager: Stage 1 진입 -> Paused 상태로 전환.");
        }
        else
        {
            // 2. Stage 2/3/4 (다음 스테이지 로드):
            Debug.Log("InGameButtonManager: Stage 2+ 진입. 플레이어 스폰 대기.");

            // SpawnPlayerCoroutine이 완료될 때까지 안전하게 대기
            yield return StartCoroutine(SpawnPlayerCoroutine(selectedPlayerPrefab));

            // EnemySpawn 초기화 및 스폰 시작
            EnemySpawn enemySpawn = Object.FindFirstObjectByType<EnemySpawn>();
            if (enemySpawn != null)
            {
                // EnemySpawn 초기화
                enemySpawn.enabled = true;
                enemySpawn.Initialize(GameManager.instance.GetCurrentStageData());
                enemySpawn.StartSpawning();
                Debug.Log("InGameButtonManager: EnemySpawn 초기화 및 스폰 시작 완료.");
            }
            else
            {
                Debug.LogError("InGameButtonManager: EnemySpawn 컴포넌트를 찾을 수 없습니다. 스폰 실패.");
            }

            // ChangeState 호출 직전에 로그를 찍어 실행 보장
            Debug.Log("InGameButtonManager: Stage 2+ 진입 -> Playing 상태로 전환 요청.");

            // [핵심 해결] 이 줄이 실행되면 모든 것이 시작됩니다.
            GameManager.instance.ChangeState(GameState.Playing);
        }
    }

    /// <summary>
    /// 아이템 선택 패널(itemSelectionPanel)에서
    /// 아이템을 고르고 '게임 시작' 버튼을 눌렀을 때 호출
    /// </summary>
    public void OnItemSelectionConfirmed()
    {
        // 1. 아이템 선택창을 끈다
        itemSelectionPanel.SetActive(false);

        // 2. Update() 함수가 자동으로 GameState를 Playing으로 변경할 것입니다.
    }

    public void OnMainButtonClick()
    {
        Time.timeScale = 1f;
        selectedPlayerPrefab = null; // 초기화
        SceneManager.LoadScene("TitleScene"); // 메인 화면 버튼 클릭 시 메인 화면으로 이동
        // LevelUpPanelLogic.ResetOpenCount(); 
    }

    public void OnReplayButtonClick()
    {
        Time.timeScale = 1f;
        selectedPlayerPrefab = null; // 초기화
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name); // 다시 시작 버튼 클릭 시 현재 씬 재시작
        // LevelUpPanelLogic.ResetOpenCount(); 
    }

    public void OnGameExitButtonClick()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnSetButtonClick()
    {
        escPanel.SetActive(false);
        setPanel.SetActive(true);
        GameManager.instance.ChangeState(GameState.Paused);
    }

    public void OnCloseButtonClick()
    {
        escPanel.SetActive(false); // 닫기 버튼 클릭 시 Esc 창 끄기
    }

    public void OnSetExitButtonClick()
    {
        setPanel.SetActive(false); // 설정창 닫기 버튼 클릭 시 설정창 끄기
    }

    public void OnGirlCharacterSelect()
    {
        selectedPlayerPrefab = girlPlayerPrefab;
        StartCoroutine(SpawnPlayerCoroutine(girlPlayerPrefab)); // 코루틴으로 호출

        characterSelectionPanel.SetActive(false);
        itemSelectionPanel.SetActive(true);
        GameManager.instance.ChangeState(GameState.Paused);
    }

    public void OnBoyCharacterSelect()
    {
        selectedPlayerPrefab = boyPlayerPrefab;
        StartCoroutine(SpawnPlayerCoroutine(boyPlayerPrefab)); // 코루틴으로 호출

        characterSelectionPanel.SetActive(false);
        itemSelectionPanel.SetActive(true);
        GameManager.instance.ChangeState(GameState.Paused);
    }

    // [유지] 코루틴으로 변경하여 캐릭터 생성 후 한 프레임 대기
    IEnumerator SpawnPlayerCoroutine(GameObject playerPrefab)
    {
        if (playerPrefab == null)
            yield break;

        Vector3 spawnPos = Vector3.zero;
        if (playerSpawnPoint != null)
            spawnPos = playerSpawnPoint.position;
        else
            Debug.LogWarning("PlayerSpawnPoint가 할당되지 않았습니다.");

        // 기존 플레이어 정리
        if (PlayerController.instance != null)
        {
            PlayerController.instance.OnLevelUp -= HandlePlayerLevelUp;
            Destroy(PlayerController.instance.gameObject);
            // 파괴 후 한 프레임 기다려 PlayerController.instance가 null이 되는 것을 확실히 보장
            yield return null;
        }

        // 새 플레이어 생성
        Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        // 새 플레이어의 Awake/Start 완료 및 instance 설정 대기
        yield return null;

        if (PlayerController.instance != null)
        {
            // 이벤트 연결
            PlayerController.instance.OnLevelUp += HandlePlayerLevelUp;
        }
    }

    void HandlePlayerLevelUp()
    {
        itemSelectionPanel.SetActive(true);
    }

    void OnDestroy()
    {
        if (PlayerController.instance != null)
        {
            PlayerController.instance.OnLevelUp -= HandlePlayerLevelUp;
        }
    }

    public void OnItemButtonClick()
    {
        PlayerController playerCnt = Object.FindFirstObjectByType<PlayerController>();

        if (playerCnt == null)
            return;

        if (playerCnt.itemUI > 0)
        {
            playerCnt.UseItemUI();
        }
        else
        {
            Debug.Log("아이템이 없어서 사용할 수 없습니다.");
        }
    }

    public void OnLevelUpPanelButtonClick()
    {
        itemLevelUpPanel.SetActive(true);
        itemSelectionPanel.SetActive(false);
        GameManager.instance.ChangeState(GameState.Paused);
    }

    public void OnLevelUpPanelCloseButtonClick()
    {
        itemLevelUpPanel.SetActive(false);
    }

    public void OnItemSelectionPanelCloseButtonClick()
    {
        itemSelectionPanel.SetActive(false);
    }

    public void OnNextStageButtonClick()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.AdvanceToNextStage(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}