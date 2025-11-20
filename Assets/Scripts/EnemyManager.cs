using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    private int currentKills = 0;

    public int KillTargetStage3 = 100;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void EnemyKilled()
    {
        if (GameManager.instance != null && GameManager.instance.currentGameState == GameState.Playing)
        {
            currentKills++;
            CheckClearCondition();
        }
    }

    void CheckClearCondition()
    {
        if (GameManager.instance.currentStageIndex == 3 && currentKills >= KillTargetStage3)
        {
            GameManager.instance.HandleStageClear();
        }
    }
}
