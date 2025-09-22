using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameButtonManager : MonoBehaviour
{
    // Esc
    [SerializeField] private GameObject escPanel;   // Esc â
    [SerializeField] private Button mainButton;     // ���� ȭ�� ��ư (���� ȭ������ �̵�)
    [SerializeField] private Button replayButton;   // �ٽ� ���� ��ư (���� �� �����)
    [SerializeField] private Button gameExitButton; // ���� ���� ��ư (���� ����)
    [SerializeField] private Button setButton;      // ���� ��ư (����â �ѱ�)
    [SerializeField] private Button closeButton;    // �ݱ� ��ư (Esc â ����)

    // Setting
    [SerializeField] private GameObject setPanel;   // ����â
    [SerializeField] private Button setExitButton;  // ����â �ݱ� ��ư (����â ����)

    private void Start()
    {
        // Esc
        escPanel.SetActive(false);                                  // Esc â ��Ȱ��ȭ
        mainButton.onClick.AddListener(OnMainButtonClick);          // ���� ȭ�� ��ư Ŭ�� �̺�Ʈ ������ ���
        replayButton.onClick.AddListener(OnReplayButtonClick);      // �ٽ� ���� ��ư Ŭ�� �̺�Ʈ ������ ���
        gameExitButton.onClick.AddListener(OnGameExitButtonClick);  // ���� ���� ��ư Ŭ�� �̺�Ʈ ������ ���
        setButton.onClick.AddListener(OnSetButtonClick);            // ���� ��ư Ŭ�� �̺�Ʈ ������ ���
        closeButton.onClick.AddListener(OnCloseButtonClick);        // �ݱ� ��ư Ŭ�� �̺�Ʈ ������ ���

        // Setting
        setPanel.SetActive(false);                                  // ����â ��Ȱ��ȭ
        setExitButton.onClick.AddListener(OnSetExitButtonClick);    // ����â �ݱ� ��ư Ŭ�� �̺�Ʈ ������ ���
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escPanel.SetActive(!escPanel.activeSelf);   // Esc Ű�� ������ Esc â ���
            setPanel.SetActive(false);                  // Esc â�� ���� �� ����â ����
        }
    }

    void OnMainButtonClick()
    {
        SceneManager.LoadScene("TitleScene"); // ���� ȭ�� ��ư Ŭ�� �� ���� ȭ������ �̵�
        // Debug.Log("Main Button Clicked");
    }

    void OnReplayButtonClick()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name); // �ٽ� ���� ��ư Ŭ�� �� ���� �� �����
        // Debug.Log("Replay Button Clicked");
    }

    void OnGameExitButtonClick()
    {
        //Debug.Log("���� ���� ��ư Ŭ����!");

#if UNITY_EDITOR
        // ����Ƽ �����Ϳ��� Play ��� ����
        EditorApplication.isPlaying = false;
#else
        // ����� ���ӿ��� ����
        Application.Quit();
#endif
    }

    void OnSetButtonClick()
    {
        escPanel.SetActive(false);  // ���� ��ư Ŭ�� �� Esc â ����
        setPanel.SetActive(true);   // ���� ��ư Ŭ�� �� ����â Ȱ��ȭ
        // Debug.Log("Set Button Clicked");
    }

    void OnCloseButtonClick()
    {
        escPanel.SetActive(false); // �ݱ� ��ư Ŭ�� �� Esc â ����
        // Debug.Log("Close Button Clicked");
    }

    void OnSetExitButtonClick()
    {
        setPanel.SetActive(false); // ����â �ݱ� ��ư Ŭ�� �� ����â ����
        // Debug.Log("Set Exit Button Clicked");
    }
}
