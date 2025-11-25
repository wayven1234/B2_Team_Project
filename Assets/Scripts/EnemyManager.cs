using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    private int currentKills = 0;

    public int CurrentKills => currentKills;

    public int KillTargetStage3 = 100;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void ResetKillCount()
    {
        currentKills = 0;
    }

    public void EnemyKilled()
    {
        if (GameManager.instance != null && GameManager.instance.currentGameState == GameState.Playing)
        {
            currentKills++;

            if (GameManager.instance.currentStageIndex == 3)
            {
                CheckClearCondition();
            }
        }
    }

    void CheckClearCondition()
    {
        if (GameManager.instance.currentStageIndex == 3 && currentKills >= KillTargetStage3)
        {
            Debug.Log($"Stage 3 클리어: {currentKills} / {KillTargetStage3} 마리 처치.");
            GameManager.instance.HandleStageClear();
        }
    }
}
