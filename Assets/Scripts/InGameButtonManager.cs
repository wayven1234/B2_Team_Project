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
    private GameObject gameOverPanel;
    private GameObject gameClearPanel;

    public string nextSceneName;

    public void StartNextStageFlow()
    {
        StartCoroutine(StageStartFlow());
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

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
        gameOverPanel = FindChildRecursive(persistentCanvas.transform, "GameOverPanel");
        gameClearPanel = FindChildRecursive(persistentCanvas.transform, "GameClearPanel");

        if (escPanel != null) escPanel.SetActive(false);
        if (setPanel != null) setPanel.SetActive(false);
        if (characterSelectionPanel != null) characterSelectionPanel.SetActive(false);
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(false);
        if (itemLevelUpPanel != null) itemLevelUpPanel.SetActive(false);
        if (stageClearPanel != null) stageClearPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameClearPanel != null) gameClearPanel.SetActive(false);

        if (GameManager.instance != null && GameManager.instance.currentStageIndex == 1)
        {
            StartCoroutine(StageStartFlow());
        }
    }

    public void SetPlayerSpawnPoint(Transform newSpawnPoint)
    {
        playerSpawnPoint = newSpawnPoint;
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
            if (characterSelectionPanel != null) characterSelectionPanel.SetActive(true);
            GameManager.instance.ChangeState(GameState.Paused);
            Debug.Log("InGameButtonManager: Stage 1 진입 -> Paused 상태로 전환 및 캐릭터 선택창 활성화.");
        }
        else
        {
            Debug.Log("InGameButtonManager: Stage 2+ 진입. 플레이어 스폰 및 게임 시작.");

            if (stageClearPanel != null)
            {
                stageClearPanel.SetActive(false);
            }

            yield return StartCoroutine(SpawnPlayerCoroutine(selectedPlayerPrefab));

            EnemySpawn enemySpawn = Object.FindFirstObjectByType<EnemySpawn>();
            if (enemySpawn != null && GameManager.instance != null)
            {
                enemySpawn.enabled = true;
            }

            GameManager.instance.ChangeState(GameState.Playing);
            Debug.Log("InGameButtonManager: Stage 2+ 진입 -> Update()에게 상태 전환을 위임.");
        }
    }

    /// <summary>
    /// 아이템 선택 패널(itemSelectionPanel)에서
    /// 아이템을 고르고 '게임 시작' 버튼을 눌렀을 때 호출
    /// </summary>
    public void OnItemSelectionConfirmed()
    {
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(false);
    }

    public void OnMainButtonClick()
    {
        Time.timeScale = 1f;
        selectedPlayerPrefab = null;
        SceneManager.LoadScene("TitleScene");
    }

    public void OnReplayButtonClick()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.StartRetryFlow();
        }
        else
        {
            Time.timeScale = 1f;
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
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
        if (escPanel != null) escPanel.SetActive(false);
    }

    public void OnSetExitButtonClick()
    {
        if (setPanel != null) setPanel.SetActive(false);
    }

    public void OnGirlCharacterSelect()
    {
        selectedPlayerPrefab = girlPlayerPrefab;
        StartCoroutine(SpawnPlayerCoroutine(girlPlayerPrefab));

        if (characterSelectionPanel != null) characterSelectionPanel.SetActive(false);
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(true);
        if (GameManager.instance != null)
        {
            GameManager.instance.ChangeState(GameState.Paused);
        }
    }

    public void OnBoyCharacterSelect()
    {
        selectedPlayerPrefab = boyPlayerPrefab;
        StartCoroutine(SpawnPlayerCoroutine(boyPlayerPrefab));

        if (characterSelectionPanel != null) characterSelectionPanel.SetActive(false);
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(true);
        if (GameManager.instance != null)
        {
            GameManager.instance.ChangeState(GameState.Paused);
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

            PlayerController.instance.UpdateUIFromData();

            PlayerController.instance.OnLevelUp += HandlePlayerLevelUp;
        }
    }

    void HandlePlayerLevelUp()
    {
        if (characterSelectionPanel != null) characterSelectionPanel.SetActive(false);
        if (itemLevelUpPanel != null) itemLevelUpPanel.SetActive(false);

        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(true);

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
            GameManager.instance.AdvanceToNextStageByCurrentIndex();
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

    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("InGameButtonManager: GameOverPanel을 찾지 못했습니다.");
        }
    }

    public void ShowGameClearPanel()
    {
        if (gameClearPanel != null)
        {
            gameClearPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("InGameButtonManager: GameClearPanel을 찾지 못했습니다.");
        }
    }

    public void ResetAllPanelsAndState()
    {
        if (escPanel != null) escPanel.SetActive(false);
        if (setPanel != null) setPanel.SetActive(false);
        if (characterSelectionPanel != null) characterSelectionPanel.SetActive(false);
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(false);
        if (itemLevelUpPanel != null) itemLevelUpPanel.SetActive(false);

        if (stageClearPanel != null) stageClearPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameClearPanel != null) gameClearPanel.SetActive(false);

        if (GameManager.instance != null && GameManager.instance.currentGameState != GameState.Paused)
        {
            GameManager.instance.ChangeState(GameState.Paused);
        }

        StartCoroutine(StageStartFlow());
    }
}