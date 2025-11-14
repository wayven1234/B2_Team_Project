using Mono.Cecil;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("스테이지 설정")]
    [SerializeField] private StageData stageData;

    [Header("게임 상태")]
    public GameState currentGameState;

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
                Time.timeScale = 0f;
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
        }
    }

    public StageData.StageType GetStageType()
    {
        return stageData.stageType;
    }
}