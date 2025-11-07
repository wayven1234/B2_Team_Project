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

    [SerializeField] private GameObject setPanel;

    void Start()
    {
        difficultyWindow.SetActive(false);

        descriptionPanel.SetActive(false);

        setPanel.SetActive(false);
    }

    public void OnStartButtonClick()
    {
        difficultyWindow.SetActive(true);
    }

    public void OnGameExButtonClick()
    {
        descriptionPanel.SetActive(true);
    }
    
    public void OnSetButton()
    {
        setPanel.SetActive(true);
    }

    public void OnExitButtonClick()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        // ����� ���ӿ��� ����
        Application.Quit();
#endif
    }

    public void OnSetExitButtonClick()
    {
        setPanel.SetActive(false);
    }

    public void OnNomalButtonClick()
    {
        SceneManager.LoadScene("Story");
    }

    public void OnNextButtonClick()
    {
        firstWindow.SetActive(false);
        secondWindow.SetActive(true);
    }

    public void OnBackButtonClick()
    {
        firstWindow.SetActive(true);
        secondWindow.SetActive(false);
    }
}
