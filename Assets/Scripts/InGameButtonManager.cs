using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class InGameButtonManager : MonoBehaviour
{
    public static InGameButtonManager instance { get; private set; }

    private static GameObject selectedPlayerPrefab = null;

    [Header("Player Prefabs & Spawn")]
    [SerializeField] private GameObject girlPlayerPrefab;
    [SerializeField] private GameObject boyPlayerPrefab;
    [SerializeField] private Transform playerSpawnPoint;

    private GameObject escPanel;
    private GameObject setPanel;
    private GameObject characterSelectionPanel;
    private GameObject itemSelectionPanel;
    private GameObject itemLevelUpPanel;
    private GameObject stageClearPanel;

    public string nextSceneName;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        Canvas persistentCanvas = FindFirstObjectByType<PersistentPanel>(FindObjectsInactive.Include)?.GetComponent<Canvas>();
        if (persistentCanvas == null)
        {
            Debug.LogError("InGameButtonManager: Persistent Canvas를 찾을 수 없습니다.");
            return;
        }

        escPanel = FindChildRecursive(persistentCanvas.transform, "EscPanel");
        setPanel = FindChildRecursive(persistentCanvas.transform, "SetPanel");
        characterSelectionPanel = FindChildRecursive(persistentCanvas.transform, "CharacterSelectionPanel");
        itemSelectionPanel = FindChildRecursive(persistentCanvas.transform, "ItemSelectionPanel");
        itemLevelUpPanel = FindChildRecursive(persistentCanvas.transform, "ItemLevelUpPanel");
        stageClearPanel = FindChildRecursive(persistentCanvas.transform, "StageClearPanel");

        if (escPanel != null) escPanel.SetActive(false);
        if (setPanel != null) setPanel.SetActive(false);
        if (characterSelectionPanel != null) characterSelectionPanel.SetActive(false);
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(false);
        if (itemLevelUpPanel != null) itemLevelUpPanel.SetActive(false);
        if (stageClearPanel != null) stageClearPanel.SetActive(false);

        StartCoroutine(StageStartFlow());
    }

    private GameObject FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child.gameObject;
            GameObject found = FindChildRecursive(child, name);
            if (found != null) return found;
        }
        return null;
    }

    private bool IsPanelActive(GameObject panel)
    {
        return panel != null && panel.activeSelf;
    }

    private void Update()
    {
        if (GameManager.instance == null) return;

        if (GameManager.instance.currentGameState == GameState.GameOver ||
        GameManager.instance.currentGameState == GameState.GameClear ||
        GameManager.instance.currentGameState == GameState.StageClear)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool isSelectionPanelActive = IsPanelActive(characterSelectionPanel) || IsPanelActive(itemSelectionPanel) || IsPanelActive(itemLevelUpPanel);

            if (!isSelectionPanelActive)
            {
                if (escPanel != null) escPanel.SetActive(!escPanel.activeSelf);
                if (setPanel != null) setPanel.SetActive(false);
            }
        }

        bool isAnyPausePanelActive = IsPanelActive(escPanel) ||
                                     IsPanelActive(setPanel) ||
                                     IsPanelActive(characterSelectionPanel) ||
                                     IsPanelActive(itemSelectionPanel) ||
                                     IsPanelActive(itemLevelUpPanel);

        if (isAnyPausePanelActive)
        {
            if (GameManager.instance.currentGameState != GameState.Paused)
            {
                GameManager.instance.ChangeState(GameState.Paused);
            }
        }
        else
        {
            if (GameManager.instance.currentGameState != GameState.Playing)
            {
                GameManager.instance.ChangeState(GameState.Playing);
            }
        }
    }

    IEnumerator StageStartFlow()
    {
        while (GameManager.instance == null)
        {
            yield return null;
        }

        if (selectedPlayerPrefab == null)
        {
            // 1. Stage 1 (첫 진입): 캐릭터 선택창 활성화
            if (characterSelectionPanel != null) characterSelectionPanel.SetActive(true);
            GameManager.instance.ChangeState(GameState.Paused);
            Debug.Log("InGameButtonManager: Stage 1 진입 -> Paused 상태로 전환 및 캐릭터 선택창 활성화.");
        }
        else
        {
            // 2. Stage 2/3/4 (다음 스테이지 로드):
            Debug.Log("InGameButtonManager: Stage 2+ 진입. 플레이어 스폰 및 게임 시작.");

            // [수정] Stage Clear Panel 비활성화: 로컬 변수(Awake에서 찾은)를 사용합니다.
            if (stageClearPanel != null)
            {
                stageClearPanel.SetActive(false);
            }

            // SpawnPlayerCoroutine이 완료될 때까지 안전하게 대기
            yield return StartCoroutine(SpawnPlayerCoroutine(selectedPlayerPrefab));

            // EnemySpawn 초기화 및 스폰 정보 전달
            EnemySpawn enemySpawn = Object.FindFirstObjectByType<EnemySpawn>();
            if (enemySpawn != null && GameManager.instance != null)
            {
                enemySpawn.enabled = true;
                // EnemySpawn.Initialize만 여기서 호출하여 Stage Data를 전달합니다.
                enemySpawn.Initialize(GameManager.instance.GetCurrentStageData());
            }

            GameManager.instance.ChangeState(GameState.Playing);
            // Update() 함수가 모든 패널이 꺼진 것을 보고 ChangeState(Playing)을 호출하도록 위임합니다.
            Debug.Log("InGameButtonManager: Stage 2+ 진입 -> Update()에게 상태 전환을 위임.");
        }
    }

    /// <summary>
    /// 아이템 선택 패널(itemSelectionPanel)에서
    /// 아이템을 고르고 '게임 시작' 버튼을 눌렀을 때 호출
    /// </summary>
    public void OnItemSelectionConfirmed()
    {
        // 1. 아이템 선택창을 끈다
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(false);

        // 2. Update() 함수가 패널이 비활성화된 것을 감지하고 GameState를 Playing으로 변경합니다.
    }

    public void OnMainButtonClick()
    {
        Time.timeScale = 1f;
        selectedPlayerPrefab = null; // 초기화
        SceneManager.LoadScene("TitleScene"); // 메인 화면 버튼 클릭 시 메인 화면으로 이동
    }

    public void OnReplayButtonClick()
    {
        Time.timeScale = 1f;
        selectedPlayerPrefab = null; // 초기화
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name); // 다시 시작 버튼 클릭 시 현재 씬 재시작
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
        if (escPanel != null) escPanel.SetActive(false);
        if (setPanel != null) setPanel.SetActive(true);
        GameManager.instance.ChangeState(GameState.Paused);
    }

    public void OnCloseButtonClick()
    {
        if (escPanel != null) escPanel.SetActive(false); // 닫기 버튼 클릭 시 Esc 창 끄기
    }

    public void OnSetExitButtonClick()
    {
        if (setPanel != null) setPanel.SetActive(false); // 설정창 닫기 버튼 클릭 시 설정창 끄기
    }

    public void OnGirlCharacterSelect()
    {
        selectedPlayerPrefab = girlPlayerPrefab;
        StartCoroutine(SpawnPlayerCoroutine(girlPlayerPrefab)); // 코루틴으로 호출

        if (characterSelectionPanel != null) characterSelectionPanel.SetActive(false);
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(true);
        if (GameManager.instance != null)
        {
            GameManager.instance.ChangeState(GameState.Paused); // 아이템 선택하는 동안 Paused 유지
        }
    }

    public void OnBoyCharacterSelect()
    {
        selectedPlayerPrefab = boyPlayerPrefab;
        StartCoroutine(SpawnPlayerCoroutine(boyPlayerPrefab)); // 코루틴으로 호출

        if (characterSelectionPanel != null) characterSelectionPanel.SetActive(false);
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(true);
        if (GameManager.instance != null)
        {
            GameManager.instance.ChangeState(GameState.Paused); // 아이템 선택하는 동안 Paused 유지
        }
    }

    IEnumerator SpawnPlayerCoroutine(GameObject playerPrefab)
    {
        if (playerPrefab == null)
            yield break;

        Vector3 spawnPos = Vector3.zero;
        if (playerSpawnPoint != null)
            spawnPos = playerSpawnPoint.position;
        else
            Debug.LogWarning("PlayerSpawnPoint가 할당되지 않았습니다.");

        if (PlayerController.instance != null)
        {
            PlayerController.instance.OnLevelUp -= HandlePlayerLevelUp;
            Destroy(PlayerController.instance.gameObject);
            yield return null;
        }

        Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        yield return null;

        if (PlayerController.instance != null)
        {
            ItemPrefab[] itemPrefabsInScene = FindObjectsByType<ItemPrefab>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            ItemData[] allItemData = itemPrefabsInScene
                .Select(ip => ip.GetData())
                .Where(data => data != null)
                .GroupBy(data => data.type)
                .Select(g => g.First())
                .ToArray();

            PlayerController.instance.LoadPlayerData(allItemData);

            PlayerController.instance.OnLevelUp += HandlePlayerLevelUp;
        }
    }

    void HandlePlayerLevelUp()
    {
        if (characterSelectionPanel != null) characterSelectionPanel.SetActive(false);
        if (itemLevelUpPanel != null) itemLevelUpPanel.SetActive(false);

        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(true);

        // 레벨업 패널이 열렸으므로 게임 상태를 Paused로 변경
        if (GameManager.instance != null)
        {
            GameManager.instance.ChangeState(GameState.Paused);
        }
    }

    void OnDestroy()
    {
        if (PlayerController.instance != null)
        {
            PlayerController.instance.OnLevelUp -= HandlePlayerLevelUp;
        }

        if (instance == this)
        {
            instance = null;
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
        if (itemLevelUpPanel != null) itemLevelUpPanel.SetActive(true);
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(false);
        GameManager.instance.ChangeState(GameState.Paused);
    }

    public void OnLevelUpPanelCloseButtonClick()
    {
        if (itemLevelUpPanel != null) itemLevelUpPanel.SetActive(false);
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(true);
    }

    public void OnItemSelectionPanelCloseButtonClick()
    {
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(false);
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

    public void ShowStageClearPanel()
    {
        if (stageClearPanel != null)
        {
            stageClearPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("InGameButtonManager: StageClearPanel을 찾지 못했습니다.");
        }
    }
}