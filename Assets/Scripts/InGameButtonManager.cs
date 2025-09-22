using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameButtonManager : MonoBehaviour
{
    // Esc
    [SerializeField] private GameObject escPanel;   // Esc 창
    [SerializeField] private Button mainButton;     // 메인 화면 버튼 (메인 화면으로 이동)
    [SerializeField] private Button replayButton;   // 다시 시작 버튼 (현재 씬 재시작)
    [SerializeField] private Button gameExitButton; // 게임 종료 버튼 (게임 종료)
    [SerializeField] private Button setButton;      // 설정 버튼 (설정창 켜기)
    [SerializeField] private Button closeButton;    // 닫기 버튼 (Esc 창 끄기)

    // Setting
    [SerializeField] private GameObject setPanel;   // 설정창
    [SerializeField] private Button setExitButton;  // 설정창 닫기 버튼 (설정창 끄기)

    private void Start()
    {
        // Esc
        escPanel.SetActive(false);                                  // Esc 창 비활성화
        mainButton.onClick.AddListener(OnMainButtonClick);          // 메인 화면 버튼 클릭 이벤트 리스너 등록
        replayButton.onClick.AddListener(OnReplayButtonClick);      // 다시 시작 버튼 클릭 이벤트 리스너 등록
        gameExitButton.onClick.AddListener(OnGameExitButtonClick);  // 게임 종료 버튼 클릭 이벤트 리스너 등록
        setButton.onClick.AddListener(OnSetButtonClick);            // 설정 버튼 클릭 이벤트 리스너 등록
        closeButton.onClick.AddListener(OnCloseButtonClick);        // 닫기 버튼 클릭 이벤트 리스너 등록

        // Setting
        setPanel.SetActive(false);                                  // 설정창 비활성화
        setExitButton.onClick.AddListener(OnSetExitButtonClick);    // 설정창 닫기 버튼 클릭 이벤트 리스너 등록
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escPanel.SetActive(!escPanel.activeSelf);   // Esc 키를 누르면 Esc 창 토글
            setPanel.SetActive(false);                  // Esc 창이 켜질 때 설정창 끄기
        }
    }

    void OnMainButtonClick()
    {
        SceneManager.LoadScene("TitleScene"); // 메인 화면 버튼 클릭 시 메인 화면으로 이동
        // Debug.Log("Main Button Clicked");
    }

    void OnReplayButtonClick()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name); // 다시 시작 버튼 클릭 시 현재 씬 재시작
        // Debug.Log("Replay Button Clicked");
    }

    void OnGameExitButtonClick()
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

    void OnSetButtonClick()
    {
        escPanel.SetActive(false);  // 설정 버튼 클릭 시 Esc 창 끄기
        setPanel.SetActive(true);   // 설정 버튼 클릭 시 설정창 활성화
        // Debug.Log("Set Button Clicked");
    }

    void OnCloseButtonClick()
    {
        escPanel.SetActive(false); // 닫기 버튼 클릭 시 Esc 창 끄기
        // Debug.Log("Close Button Clicked");
    }

    void OnSetExitButtonClick()
    {
        setPanel.SetActive(false); // 설정창 닫기 버튼 클릭 시 설정창 끄기
        // Debug.Log("Set Exit Button Clicked");
    }
}
