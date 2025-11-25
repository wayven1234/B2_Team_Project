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
    private float _displayTime;           // UI에 표시될 계산된 시간
    private float _times;                // 게임 시작 후 누적 시간
    private bool _isStageEndCalled = false; // Stage/Game Clear 호출 여부

    public event System.Action OnTimeUp;

    // 이 오브젝트가 활성화될 때 (Start 대신 OnEnable 사용)
    private void OnEnable()
    {
        LoadStageTime();

        // 타이머 초기화
        _times = 0.0f;
        _isTimeOver = false;
        _isStageEndCalled = false; // 변수명 통일

        if (_isCountDown)
            _displayTime = _gameTime;   // 카운트다운은 gameTime에서 시작

        // 텍스트가 연결되어 있다면 UI도 즉시 업데이트
        UpdateUIText();
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
        // 시간이 이미 끝났으면 계산 중지
        if (_isTimeOver) return;

        // 누적 시간 증가
        _times += Time.deltaTime;

        if (_isCountDown)
        {
            // 카운트다운
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
        // _timeText가 Inspector에서 연결되지 않았다면 실행하지 않음
        if (_timeText == null) return;

        // 소수점 버리고 정수로 변환
        int totalSeconds = (int)_displayTime;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        // "D2" 서식을 사용해 항상 2자리로 표시 (예: "00:00")
        _timeText.text = $"{minutes:D2}:{seconds:D2}";
    }

    /// <summary>
    /// 게임 시간이 모두 소진되었는지 확인하고, Stage/Game Clear 처리를 Game Manager에 요청합니다.
    /// </summary>
    void CheckStageEnd()
    {
        if (GameManager.instance == null) return;

        // Playing 상태가 아니거나 이미 처리했으면 리턴
        if (GameManager.instance.currentGameState != GameState.Playing || _isStageEndCalled)
        {
            return;
        }

        // 시간이 종료되었을 때
        if (_isTimeOver)
        {
            // 시간이 끝났음을 알리는 이벤트를 발생시킵니다. (SurvivalStageController에서 구독할 예정)
            OnTimeUp?.Invoke();

            // Stage 3이 아닌 경우에만 시간 만료를 클리어 조건으로 간주합니다.
            if (GameManager.instance.currentStageIndex != 3)
            {
                // 플레이어 관련 상태 처리 (예: 움직임 멈춤)
                if (_playerCnt != null)
                {
                    _playerCnt.GameStop();
                }

                // GameManager에 Stage Clear 처리를 요청합니다.
                GameManager.instance.HandleStageClear();

                _isStageEndCalled = true; // 중복 호출 방지
            }
            else
            {
                // Stage 3에서 시간이 끝났지만 킬 수 조건 미달성 시에는 게임 오버가 아닐 수 있으므로 
                Debug.Log("TimeManager: Stage 3 시간 종료. 킬 수 조건 확인 중.");
            }
        }
    }

    /// <summary>
    /// GameManager가 다음 스테이지로 전환될 때 타이머를 초기 상태로 초기화합니다.
    /// </summary>
    public void ResetTimer()
    {
        LoadStageTime(); // 스테이지 전환 시 시간 재 로드

        // [수정] _times도 0.0f로 초기화하여 누적 시간 초기화 보장
        _times = 0.0f;
        _isTimeOver = false;
        _isStageEndCalled = false; // 변수명 통일

        if (_isCountDown)
            _displayTime = _gameTime;

        UpdateUIText();
        Debug.Log("TimeManager: ResetTimer() 호출됨. 시간 재설정 완료.");
    }
}