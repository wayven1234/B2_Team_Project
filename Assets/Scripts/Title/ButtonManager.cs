using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject difficultyWindow;

    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private GameObject firstWindow;
    [SerializeField] private GameObject secondWindow;

    [SerializeField] private GameObject escPanel;
    [SerializeField] private GameObject setPanel;

    void Start()
    {
        difficultyWindow?.SetActive(false);

        descriptionPanel?.SetActive(false);

        escPanel?.SetActive(false);

        setPanel?.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (descriptionPanel != null && descriptionPanel.activeSelf)
            {
                descriptionPanel.SetActive(false);
                return;
            }
            
            if (setPanel != null && setPanel.activeSelf)
            {
                setPanel.SetActive(false);
                return;
            }

            if (setPanel != null && !setPanel.activeSelf)
            {
                if (escPanel != null)
                {
                    bool isActive = escPanel.activeSelf;
                    escPanel.SetActive(!isActive);
                }
            }
        }
    }

    public void OnStartButtonClick()
    {
        difficultyWindow?.SetActive(true);
    }

    public void OnGameExButtonClick()
    {
        descriptionPanel?.SetActive(true);
    }
    
    public void OnSetButton()
    {
        setPanel?.SetActive(true);
    }

    public void OnExitButtonClick()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnSetExitButtonClick()
    {
        setPanel?.SetActive(false);
    }

    public void OnNomalButtonClick()
    {
        SceneManager.LoadScene("Story");
    }

    public void OnNextButtonClick()
    {
        firstWindow?.SetActive(false);
        secondWindow?.SetActive(true);
    }

    public void OnBackButtonClick()
    {
        firstWindow?.SetActive(true);
        secondWindow?.SetActive(false);
    }

    public void OnMainButtonClick()
    {
        SceneManager.LoadScene("TitleScene"); // 메인 화면 버튼 클릭 시 메인 화면으로 이동
    }

    public void OnReplayButtonClick()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name); // 다시 시작 버튼 클릭 시 현재 씬 재시작
    }

    public void OnGameExitButtonClick()
    {
#if UNITY_EDITOR
        // 유니티 에디터에서 Play 모드 종료
        EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서 종료
        Application.Quit();
#endif
    }

    public void OnCloseButtonClick()
    {
        escPanel.SetActive(false); // 닫기 버튼 클릭 시 Esc 창 끄기
    }

    public void OnSetButtonClick()
    {
        escPanel.SetActive(false);  // 설정 버튼 클릭 시 Esc 창 끄기
        setPanel.SetActive(true);   // 설정 버튼 클릭 시 설정창 활성화
    }
}
