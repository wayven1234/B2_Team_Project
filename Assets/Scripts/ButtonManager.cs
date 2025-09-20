using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    // Main
    [SerializeField] private Button startButton;    // ���� ���� ��ư (Scene ��ȯ)
    [SerializeField] private Button gameExButton;   // ���� ���� ��ư (����â �ѱ�)
    [SerializeField] private Button setButton;      // ���� ���� ��ư (����â �ѱ�)
    [SerializeField] private Button exitButton;     // ���� ���� ��ư (���� ����)

    // Setting
    [SerializeField] private GameObject setPanel;   // ����â
    [SerializeField] private Button setExitButton;  // ����â �ݱ� ��ư (����â ����)

    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClick);        // ���� ���� ��ư Ŭ�� �̺�Ʈ ������ ���
        gameExButton.onClick.AddListener(OnGameExButtonClick);      // ���� ���� ��ư Ŭ�� �̺�Ʈ ������ ���
        setButton.onClick.AddListener(OnSetButton);                 // ���� ���� ��ư Ŭ�� �̺�Ʈ ������ ���
        exitButton.onClick.AddListener(OnExitButtonClick);          // ���� ���� ��ư Ŭ�� �̺�Ʈ ������ ���

        setPanel.SetActive(false);                                  // ����â ��Ȱ��ȭ
        setExitButton.onClick.AddListener(OnSetExitButtonClick);    // ����â �ݱ� ��ư Ŭ�� �̺�Ʈ ������ ���
    }

    public void OnStartButtonClick()
    {
        //ScneneManager.LoadScene("Stage1"); // ���� ���� ��ư Ŭ�� �� �� ��ȯ (�� �̸� �Է�)
        //Debug.Log("Start Button Clicked");
    }

    public void OnGameExButtonClick()
    {
        // //���� ���� ��ư Ŭ�� �� ����â Ȱ��ȭ
        //Debug.Log("Game Ex Button Clicked");
    }

    public void OnSetButton()
    {
        setPanel.SetActive(true); // ���� ���� ��ư Ŭ�� �� ����â Ȱ��ȭ
        //Debug.Log("Set Button Clicked");
    }

    public void OnExitButtonClick()
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

    public void OnSetExitButtonClick()
    {
        setPanel.SetActive(false); // ����â �ݱ� ��ư Ŭ�� �� ����â ��Ȱ��ȭ
        //Debug.Log("Set Exit Button Clicked");
    }

}
