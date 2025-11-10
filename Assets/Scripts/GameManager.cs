using Mono.Cecil;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("스테이지 설정")]
    public StageType currentStageType;

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
        ChangeState(GameState.Playing);
    }

    public void ChangeState(GameState newState)
    {
        currentGameState = newState;
    }

    public StageType GetStageType()
    {
        return currentStageType;
    }
}