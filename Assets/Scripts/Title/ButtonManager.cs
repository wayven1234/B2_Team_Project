using Unity.VisualScripting;
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
            if (difficultyWindow != null && difficultyWindow.activeSelf)
            {
                difficultyWindow.SetActive(false);
                return;
            }

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
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.ButtonClick);
        }
        difficultyWindow?.SetActive(true);
    }

    public void OnDifficultyWindowCloseButtonClick()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.ButtonClick);
        }
        difficultyWindow?.SetActive(false);
    }

    public void OnGameExButtonClick()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.ButtonClick);
        }
        descriptionPanel?.SetActive(true);

        firstWindow?.SetActive(true);
        secondWindow?.SetActive(false);
    }
    
    public void OnSetButton()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.ButtonClick);
        }
        setPanel?.SetActive(true);
    }

    public void OnExitButtonClick()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.ButtonClick);
        }
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnSetExitButtonClick()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.ButtonClick);
        }
        setPanel?.SetActive(false);
    }

    public void OnNomalButtonClick()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.ButtonClick);
        }
        SceneManager.LoadScene("Story");
    }

    public void OnNextButtonClick()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.ButtonClick);
        }
        firstWindow?.SetActive(false);
        secondWindow?.SetActive(true);
    }

    public void OnBackButtonClick()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.ButtonClick);
        }
        firstWindow?.SetActive(true);
        secondWindow?.SetActive(false);
    }

    public void OnMainButtonClick()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.ButtonClick);
        }
        SceneManager.LoadScene("TitleScene");
    }

    public void OnReplayButtonClick()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.ButtonClick);
        }
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void OnGameExitButtonClick()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.ButtonClick);
        }
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
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.ButtonClick);
        }
        escPanel.SetActive(false);
    }

    public void OnSetButtonClick()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.ButtonClick);
        }
        escPanel.SetActive(false);
        setPanel.SetActive(true);
    }

    public void OnDescriptionCloseButtonClick()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(SFXType.ButtonClick);
        }
        descriptionPanel.SetActive(false);
    }
}
