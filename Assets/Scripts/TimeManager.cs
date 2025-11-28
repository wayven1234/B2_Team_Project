using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Stage별 목표 시간 (Inspector에서 설정)")]
    public float targetTimeStage1 = 90.0f;
    public float targetTimeStage2 = 120.0f;
    public float targetTimeStage3 = 60.0f;
    public float targetTimeStage4 = 300.0f;

    // 공개
    [Header("시간 설정")]
    public bool _isCountDown = true;
    public float _gameTime;

    [Header("연결")]
    public PlayerController _playerCnt;
    public TextMeshProUGUI _timeText;

    [Header("현재 상태")]
    public bool _isTimeOver = false;

    // 비공개
    private float _displayTime;
    private float _times = -1f;
    private bool _isStageEndCalled = false;

    public event System.Action OnTimeUp;

    private void OnEnable()
    {
        if (_times < 0)
        {
            LoadStageTime();

            _times = 0.0f;
            _isTimeOver = false;
            _isStageEndCalled = false;

            if (_isCountDown)
                _displayTime = _gameTime;
        }

        UpdateUIText();
        Debug.Log("TimeManager: OnEnable 호출. 초기화는 GameManager의 OnSceneLoaded 이벤트에 위임.");
    }

    private void Update()
    {
        if (GameManager.instance == null) return;
        if (GameManager.instance.currentGameState != GameState.Playing)
        {
            return;
        }

        TimeCalculation();
        UpdateUIText();
        CheckStageEnd();
    }

    /// <summary>
    /// 현재 스테이지 인덱스를 기반으로 _gameTime에 목표 시간을 설정합니다.
    /// </summary>
    void LoadStageTime()
    {
        if (GameManager.instance == null) return;

        int currentStage = GameManager.instance.currentStageIndex;
        _isCountDown = true;

        switch (currentStage)
        {
            case 1:
                _gameTime = targetTimeStage1;
                break;
            case 2:
                _gameTime = targetTimeStage2;
                break;
            case 3:
                _gameTime = targetTimeStage3;
                break;
            case 4:
                _gameTime = targetTimeStage4;
                break;
            default:
                _gameTime = 60.0f;
                break;
        }

        Debug.Log($"TimeManager: Stage {currentStage} 목표 시간 {_gameTime}초로 설정.");
    }

    void TimeCalculation()
    {
        if (_isTimeOver) return;

        _times += Time.deltaTime;

        if (_isCountDown)
        {
            _displayTime = _gameTime - _times;
            if (_displayTime <= 0.0f)
            {
                _displayTime = 0.0f;
                _isTimeOver = true;
            }
        }
    }

    void UpdateUIText()
    {
        if (_timeText == null) return;

        int totalSeconds = (int)_displayTime;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        _timeText.text = $"{minutes:D2}:{seconds:D2}";
    }

    /// <summary>
    /// 게임 시간이 모두 소진되었는지 확인하고, Stage/Game Clear 처리를 Game Manager에 요청합니다.
    /// </summary>
    void CheckStageEnd()
    {
        if (GameManager.instance == null) return;

        if (GameManager.instance.currentGameState != GameState.Playing || _isStageEndCalled)
        {
            return;
        }

        if (_isTimeOver)
        {
            OnTimeUp?.Invoke();
            _isStageEndCalled = true;

            if (GameManager.instance.currentStageIndex == 3)
            {
                if (EnemyManager.instance != null)
                    EnemyManager.instance.CheckClearConditionOnTimeOut();
                else
                    Debug.LogError("EnemeyManager 인스턴트를 찾을 수 없습니다! Stage 3 클리어 조건 확인 실패.");
            }
            else
            {
                if (_playerCnt != null)
                    _playerCnt.GameStop();

                GameManager.instance.HandleStageClear();
            }
        }
    }

    /// <summary>
    /// GameManager가 다음 스테이지로 전환될 때 타이머를 초기 상태로 초기화합니다.
    /// </summary>
    public void ResetTimer()
    {
        LoadStageTime();

        _times = 0.0f;
        _isTimeOver = false;
        _isStageEndCalled = false;

        if (_isCountDown)
            _displayTime = _gameTime;

        UpdateUIText();
        Debug.Log($"TimeManager: ResetTimer() 호출됨. Stage {GameManager.instance.currentStageIndex} 시간 재설정 완료.");
    }
}