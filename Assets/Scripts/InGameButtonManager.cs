using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameButtonManager : MonoBehaviour
{
    // Esc
    [SerializeField] private GameObject escPanel;   // Esc 창

    [SerializeField] private GameObject setPanel;   // 설정창

    private void Start()
    {
        escPanel.SetActive(false);  // 게임 시작 시 Esc 창 비활성화
        setPanel.SetActive(false);  // 게임 시작 시 설정창 비활성화
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escPanel.SetActive(!escPanel.activeSelf);   // Esc 키를 누르면 Esc 창 토글
            setPanel.SetActive(false);                  // Esc 창이 켜질 때 설정창 끄기
        }

        // esc창, 설정창의 상태를 보고 게임을 일시정지 또는 재개
        if (escPanel.activeSelf || setPanel.activeSelf)
        {
            Time.timeScale = 0f; // 참이 하나라도 켜져있으면 게임 일시정지
        }
        else
        {
            Time.timeScale = 1f; // 모두 닫혀 있으면 게임 재개
        }
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
    }

    public void OnCloseButtonClick()
    {
        escPanel.SetActive(false); // 닫기 버튼 클릭 시 Esc 창 끄기
    }

    public void OnSetExitButtonClick()
    {
        setPanel.SetActive(false); // 설정창 닫기 버튼 클릭 시 설정창 끄기
    }
}
