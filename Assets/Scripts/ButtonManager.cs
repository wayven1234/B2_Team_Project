using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    // Main Set
    [SerializeField] private Button startButton;    // 게임 시작 버튼 (Scene 전환)
    [SerializeField] private Button gameExButton;   // 게임 설명 버튼 (설명창 켜기)
    [SerializeField] private Button setButton;      // 게임 설정 버튼 (설정창 켜기)
    [SerializeField] private Button exitButton;     // 게임 종료 버튼 (게임 종료)

    // Difficulty Window Set
    [SerializeField] private GameObject difficultyWindow;   // 게임 난이도 패널 (난이도 설정 창)
    [SerializeField] private Button easyButton;             // 게임 난이도 버튼 (쉬움)
    [SerializeField] private Button nomalButton;            // 게임 난이도 버튼 (보통)
    [SerializeField] private Button hardButton;             // 게임 난이도 버튼 (어려움)

    // Description Window Set
    [SerializeField] private GameObject descriptionPanel;   // 게임 설명창 패널
    [SerializeField] private GameObject firstWindow;        // 게임 설명창 1
    [SerializeField] private Button nextButton;             // 설명창 버튼 (다음 페이지로)
    [SerializeField] private GameObject secondWindow;       // 게임 설명창 2
    [SerializeField] private Button backButton;             // 설명창 버튼 (이전 페이지로)

    // Setting Set
    [SerializeField] private GameObject setPanel;   // 설정창
    [SerializeField] private Button setExitButton;  // 설정창 닫기 버튼 (설정창 끄기)

    void Start()
    {
        // Main Set
        startButton.onClick.AddListener(OnStartButtonClick);        // 게임 시작 버튼 클릭 이벤트 리스너 등록
        gameExButton.onClick.AddListener(OnGameExButtonClick);      // 게임 설명 버튼 클릭 이벤트 리스너 등록
        setButton.onClick.AddListener(OnSetButton);                 // 게임 설정 버튼 클릭 이벤트 리스너 등록
        exitButton.onClick.AddListener(OnExitButtonClick);          // 게임 종료 버튼 클릭 이벤트 리스너 등록

        // Difficulty Window Set
        difficultyWindow.SetActive(false);                          // 게임 난이도 패널 비활성화
        easyButton.interactable = false;                            // 게임 난이도 버튼 (쉬움) 비활성화
        //esayButton.onClick.AddListener(OnEasyButtonClick);        // 게임 난이도 버튼 (쉬움) 클릭 이벤트 리스너 등록
        nomalButton.onClick.AddListener(OnNomalButtonClick);        // 게임 난이도 버튼 (보통) 클릭 이벤트 리스너 등록
        hardButton.interactable = false;                            // 게임 난이도 버튼 (어려움) 비활성화
        //hardButton.onClick.AddListener(OnHardButtonClick);        // 게임 난이도 버튼 (어려움) 클릭 이벤트 리스너 등록

        // Description Window Set
        descriptionPanel.SetActive(false);                          // 게임 설명창 패널 비활성화
        nextButton.onClick.AddListener(OnNextButtonClick);                      // 게임 설명창 버튼 클릭 이벤트 리스너 등록
        backButton.onClick.AddListener(OnBackButtonClick);                      // 게임 설명창 버튼 클릭 이벤트 리스너 등록

        // Setting Set
        setPanel.SetActive(false);                                  // 설정창 비활성화
        setExitButton.onClick.AddListener(OnSetExitButtonClick);    // 설정창 닫기 버튼 클릭 이벤트 리스너 등록
    }


    // 버튼 클릭 메서드
    public void OnStartButtonClick()
    {
        difficultyWindow.SetActive(true);
    }

    public void OnGameExButtonClick()
    {
        // //게임 설명 버튼 클릭 시 설명창 활성화
        descriptionPanel.SetActive(true);
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

    //public void OnEasyButtonClick()
    //{
    //  SceneManager.LoadScene("Stage1");
    //}

    public void OnNomalButtonClick()
    {
        SceneManager.LoadScene("Stage1");
        // Debug.Log("스테이지 1 이동");
    }

    //public void OnHardButtonClick()
    //{
    //  SceneManager.LoadScene("Stage1");
    //}

    public void OnNextButtonClick()
    {
        firstWindow.SetActive(false);
        // Debug.Log("설명창 1 비활성화");
        secondWindow.SetActive(true);
        // Debug.Log("설명창 2 활성화");
    }
    
    public void OnBackButtonClick()
    {
        firstWindow.SetActive(true);
        // Debug.Log("설명창 1 활성화");
        secondWindow.SetActive(false);
        // Debug.Log("설명창 2 비활성화");
    }

}
