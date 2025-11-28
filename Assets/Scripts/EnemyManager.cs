using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    private int currentKills = 0;

    public int CurrentKills => currentKills;

    public int KillTargetStage3 = 100;

    [SerializeField] private TextMeshProUGUI killCountText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 씬 로드 완료 시 호출되어 킬 카운트를 초기화합니다.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 매 스테이지 시작 시 킬 카운트 리셋
        ResetKillCount();

        UpdateKillCountUI();
        Debug.Log($"EnemyManager: {scene.name} 로드 완료. 킬 카운트 리셋.");
    }

    public void ResetKillCount()
    {
        currentKills = 0;
    }

    private void UpdateKillCountUI()
    {
        if (killCountText == null) return;

        if (GameManager.instance != null && GameManager.instance.currentStageIndex == 3)
            killCountText.text = $"킬 수: {currentKills}";
    }

    public void EnemyKilled()
    {
        if (GameManager.instance != null && GameManager.instance.currentGameState == GameState.Playing)
        {
            currentKills++;
            UpdateKillCountUI();

            if (GameManager.instance.currentStageIndex == 3 && currentKills >= KillTargetStage3)
            {
                Debug.Log($"Stage 3 목표 달성! {currentKills} / {KillTargetStage3} 마리 처치. 즉시 클리어 처리.");
                GameManager.instance.HandleStageClear();
            }
        }
    }

    public void CheckClearConditionOnTimeOut()
    {
        if (GameManager.instance == null) return;

        if (GameManager.instance.currentStageIndex == 3)
        {
            if (currentKills < KillTargetStage3)
            {
                Debug.Log($"Stage 3 클리어 실패: {currentKills} / {KillTargetStage3} 마리 처치. (시간 초과)");
                GameManager.instance.ChangeState(GameState.GameOver);
            }
            else
            {
                Debug.Log("Stage 3 시간 초과 확인. 킬 목표는 이미 충족되었습니다.");
            }
        }
    }
}
