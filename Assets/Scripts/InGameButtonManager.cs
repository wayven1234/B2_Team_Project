using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameButtonManager : MonoBehaviour
{
    [Header("Player Prefabs & Spawn")]
    [SerializeField] private GameObject girlPlayerPrefab;
    [SerializeField] private GameObject boyPlayerPrefab;
    [SerializeField] private Transform playerSpawnPoint;

    // Esc
    [SerializeField] private GameObject escPanel;   // Esc 창
    // Setting
    [SerializeField] private GameObject setPanel;   // 설정창

    [SerializeField] private GameObject characterSelectionPanel;
    [SerializeField] private GameObject itemSelectionPanel;
    [SerializeField] private GameObject itemLevelUpPanel;

    private void Start()
    {
        GameManager.instance.ChangeState(GameState.Paused);

        escPanel.SetActive(false);  // 게임 시작 시 Esc 창 비활성화
        setPanel.SetActive(false);  // 게임 시작 시 설정창 비활성화

        characterSelectionPanel.SetActive(true);
        itemSelectionPanel.SetActive(false);
        itemLevelUpPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escPanel.SetActive(!escPanel.activeSelf);   // Esc 키를 누르면 Esc 창 토글
            setPanel.SetActive(false);                  // Esc 창이 켜질 때 설정창 끄기
        }

        bool isAnyPausePanelActive = escPanel.activeSelf || 
                                     setPanel.activeSelf ||
                                     characterSelectionPanel.activeSelf ||
                                     itemSelectionPanel.activeSelf ||
                                     itemLevelUpPanel.activeSelf;

        if (isAnyPausePanelActive)
        {
            // 패널이 하나라도 켜져 있다면, 게임 상태를 Paused로 설정
            GameManager.instance.ChangeState(GameState.Paused);
        }
        else
        {
            // 위의 패널이 *모두* 꺼져 있을 때만 Playing 상태로 변경
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

        // 2. 이제 모든 패널이 꺼졌으므로, Update() 함수가
        //    자동으로 GameState를 Playing으로 변경할 것입니다.
        //    (여기서 Playing으로 변경해도 됨)
        // GameManager.instance.ChangeState(GameState.Playing);
    }

    public void OnMainButtonClick()
    {
        SceneManager.LoadScene("TitleScene"); // 메인 화면 버튼 클릭 시 메인 화면으로 이동
        GameManager.instance.ChangeState(GameState.Playing);
    }

    public void OnReplayButtonClick()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name); // 다시 시작 버튼 클릭 시 현재 씬 재시작
        GameManager.instance.ChangeState(GameState.Playing);
    }

    public void OnGameExitButtonClick()
    {
        //Debug.Log("게임 종료 버튼 클릭됨!");

#if UNITY_EDITOR
        // 유니티 에디터에서 Play 모드 종료
        EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서 종료
        Application.Quit();
#endif
    }

    public void OnSetButtonClick()
    {
        escPanel.SetActive(false);  // 설정 버튼 클릭 시 Esc 창 끄기
        setPanel.SetActive(true);   // 설정 버튼 클릭 시 설정창 활성화
        GameManager.instance.ChangeState(GameState.Paused);
    }

    public void OnCloseButtonClick()
    {
        escPanel.SetActive(false); // 닫기 버튼 클릭 시 Esc 창 끄기
        GameManager.instance.ChangeState(GameState.Playing);
    }

    public void OnSetExitButtonClick()
    {
        setPanel.SetActive(false); // 설정창 닫기 버튼 클릭 시 설정창 끄기
        GameManager.instance.ChangeState(GameState.Playing);
    }

    public void OnGirlCharacterSelect()
    {
        SpawnPlayer(girlPlayerPrefab);

        characterSelectionPanel.SetActive(false);
        itemSelectionPanel.SetActive(true);
        GameManager.instance.ChangeState(GameState.Paused);
    }

    public void OnBoyCharacterSelect()
    {
        SpawnPlayer(boyPlayerPrefab);

        characterSelectionPanel.SetActive(false);
        itemSelectionPanel.SetActive(true);
        GameManager.instance.ChangeState(GameState.Paused);
    }

    void SpawnPlayer(GameObject playerPrefab)
    {
        if (playerPrefab == null)
            return;

        Vector3 spawnPos = Vector3.zero;
        if (playerSpawnPoint != null)
            spawnPos = playerSpawnPoint.position;
        else
            Debug.LogWarning("PlayerSpawnPoint가 할당되지 않았습니다.");

        if (PlayerController.instance != null)
            Destroy(PlayerController.instance.gameObject);

        Instantiate(playerPrefab, spawnPos, Quaternion.identity);
    }

    public void OnItemButtonClick()
    {
        PlayerController playerCnt = Object.FindFirstObjectByType<PlayerController>();

        if (playerCnt == null)
            return;

        // 아이템이 1개 이상 있을 때만 사용 가능
        if (playerCnt.itemUI > 0)
        {
            playerCnt.UseItemUI();
        }
        else
        {
            Debug.Log("아이템이 없어서 사용할 수 없습니다.");
        }
    }
}
