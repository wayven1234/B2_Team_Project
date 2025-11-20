using Mono.Cecil;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("스테이지 설정")]
    [SerializeField] private StageData stageData;

    [Header("UI 연결")]
    public GameObject stageClearPanel;

    [Header("현재 게임 상태")]
    public GameState currentGameState;
    public int currentStageIndex = 1;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (stageData != null)
        {
            Debug.Log("현재 스테이지 Type:" + stageData.stageType);
        }

        ChangeState(GameState.Playing);

        if (stageClearPanel != null)
            stageClearPanel.SetActive(false);
    }

    public void ChangeState(GameState newState)
    {
        if (currentGameState == newState) return;

        currentGameState = newState;
        Debug.Log("게임 상태 변경: " + newState);

        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.StageClear:
                Time.timeScale = 0f;
                break;
            case GameState.GameClear:
            case GameState.GameOver:
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
        }
    }

    public StageData.StageType GetStageType()
    {
        return stageData.stageType;
    }

    public void HandleStageClear()
    {
        bool isFinalStage = (currentStageIndex == 4);

        if (isFinalStage)
        {
            ChangeState(GameState.GameClear);
        }
        else
        {
            ChangeState(GameState.StageClear);

            // StageClear 시 Panel 활성화 로직 추가 (기존 ExecuteStageClear의 내용)
            if (stageClearPanel != null)
            {
                stageClearPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("Stage Clear Panel이 GameManager에 연결되지 않았습니다. UI를 띄울 수 없습니다.");
            }
        }
    }
}