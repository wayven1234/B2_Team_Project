using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    // Main
    [SerializeField] private Button startButton;    // 게임 시작 버튼 (Scene 전환)
    [SerializeField] private Button gameExButton;   // 게임 설명 버튼 (설명창 켜기)
    [SerializeField] private Button setButton;      // 게임 설정 버튼 (설정창 켜기)
    [SerializeField] private Button exitButton;     // 게임 종료 버튼 (게임 종료)

    // Setting
    [SerializeField] private GameObject setPanel;   // 설정창
    [SerializeField] private Button setExitButton;  // 설정창 닫기 버튼 (설정창 끄기)

    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClick);        // 게임 시작 버튼 클릭 이벤트 리스너 등록
        gameExButton.onClick.AddListener(OnGameExButtonClick);      // 게임 설명 버튼 클릭 이벤트 리스너 등록
        setButton.onClick.AddListener(OnSetButton);                 // 게임 설정 버튼 클릭 이벤트 리스너 등록
        exitButton.onClick.AddListener(OnExitButtonClick);          // 게임 종료 버튼 클릭 이벤트 리스너 등록

        setPanel.SetActive(false);                                  // 설정창 비활성화
        setExitButton.onClick.AddListener(OnSetExitButtonClick);    // 설정창 닫기 버튼 클릭 이벤트 리스너 등록
    }

    public void OnStartButtonClick()
    {
        //ScneneManager.LoadScene("Stage1"); // 게임 시작 버튼 클릭 시 씬 전환 (씬 이름 입력)
        //Debug.Log("Start Button Clicked");
    }

    public void OnGameExButtonClick()
    {
        // //게임 설명 버튼 클릭 시 설명창 활성화
        //Debug.Log("Game Ex Button Clicked");
    }

    public void OnSetButton()
    {
        setPanel.SetActive(true); // 게임 설명 버튼 클릭 시 설명창 활성화
        //Debug.Log("Set Button Clicked");
    }

    public void OnExitButtonClick()
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

    public void OnSetExitButtonClick()
    {
        setPanel.SetActive(false); // 설정창 닫기 버튼 클릭 시 설정창 비활성화
        //Debug.Log("Set Exit Button Clicked");
    }

}
