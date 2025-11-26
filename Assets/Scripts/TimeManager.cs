using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Stage별 목표 시간 (Inspector에서 설정)")]
    public float targetTimeStage1 = 90.0f; // Stage 1: 1분 30초
    public float targetTimeStage2 = 120.0f; // Stage 2: 2분
    public float targetTimeStage3 = 60.0f;  // Stage 3: 60초
    public float targetTimeStage4 = 300.0f; // Stage 4: 5분

    // 공개
    [Header("시간 설정")]
    public bool _isCountDown = true;    // true: 카운트다운, false: 카운드업
    public float _gameTime;             // 게임 시간

    [Header("연결")]
    public PlayerController _playerCnt; // PlayerController 연결
    public TextMeshProUGUI _timeText;    // TimeText 연결

    [Header("현재 상태")]
    public bool _isTimeOver = false;    // 시간 종료 여부

    // 비공개
    private float _displayTime;             // UI에 표시될 계산된 시간
    private float _times = -1f;             // -1f은 초기화 전 상태를 의미합니다.
    private bool _isStageEndCalled = false; // Stage/Game Clear 호출 여부

    public event System.Action OnTimeUp;

    // 이 오브젝트가 활성화될 때 (GameManager에서 ResetTimer 호출 예정)
    private void OnEnable()
    {
        if (_times < 0)
        {
            // ResetTimer가 호출되지 않았을 경우, 수동으로 초기화
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

    // 이 오브젝트가 활성화되어 있는 동안 매 프레임 호출
    private void Update()
    {
        if (GameManager.instance == null) return;
        if (GameManager.instance.currentGameState != GameState.Playing)
        {
            return;
        }

        // 1. 시간 계산
        TimeCalculation();
        // 2. UI 텍스트 업데이트
        UpdateUIText();
        // 3. 게임 오버 조건 확인
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

    // 1. 시간 계산 함수
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
                _isTimeOver = true; // 시간 종료
            }
        }
    }

    // 2. UI 텍스트 MM:SS 형식으로 업데이트 하는 함수
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

            if (GameManager.instance.currentStageIndex != 3)
            {
                if (_playerCnt != null)
                {
                    _playerCnt.GameStop();
                }

                GameManager.instance.HandleStageClear();

                _isStageEndCalled = true; // 중복 호출 방지
            }
            else
            {
                Debug.Log("TimeManager: Stage 3 시간 종료. 킬 수 조건 확인 중.");
            }
        }
    }

    /// <summary>
    /// GameManager가 다음 스테이지로 전환될 때 타이머를 초기 상태로 초기화합니다.
    /// </summary>
    public void ResetTimer()
    {
        LoadStageTime(); // 현재 Stage Index에 맞는 시간 재 로드

        _times = 0.0f;
        _isTimeOver = false;
        _isStageEndCalled = false;

        if (_isCountDown)
            _displayTime = _gameTime;

        UpdateUIText();
        Debug.Log($"TimeManager: ResetTimer() 호출됨. Stage {GameManager.instance.currentStageIndex} 시간 재설정 완료.");
    }
}