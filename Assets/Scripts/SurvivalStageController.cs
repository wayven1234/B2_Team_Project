using UnityEngine;

public class SurvivalStageController : MonoBehaviour
{
    private TimeManager timeManager;
    private bool isSubscribed = false;

    void Start()
    {
        timeManager = FindFirstObjectByType<TimeManager>();
        SubscribeEvents();
    }

    void SubscribeEvents()
    {
        if (timeManager != null && !isSubscribed)
        {
            timeManager.OnTimeUp += HandleTimeUpStageClear;
            isSubscribed = true;
        }
    }

    void HandleTimeUpStageClear()
    {
        if (GameManager.instance == null) return;

        if (GameManager.instance.currentStageIndex == 3)
        {
            if (EnemyManager.instance != null && EnemyManager.instance.CurrentKills < EnemyManager.instance.KillTargetStage3)
            {
                Debug.Log("Stage 3 시간 초과 및 킬 수 미달성. 게임 오버 처리.");
                GameManager.instance.ChangeState(GameState.GameOver);
            }
        }
    }

    void OnDestroy()
    {
        if (timeManager != null && isSubscribed)
        {
            timeManager.OnTimeUp -= HandleTimeUpStageClear;
        }
    }
}