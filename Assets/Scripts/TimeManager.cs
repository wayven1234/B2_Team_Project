using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
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
    private float _displayTime;          // UI에 표시될 계산된 시간
    private float _times;               // 게임 시작 후 누적 시간
    private bool _isGameClearCalled = false; // GameClear() 호출 여부

    // 이 오브젝트가 활성화될 때 (Start 대신 OnEnable 사용)
    private void OnEnable()
    {
        // 타이머 초기화
        _times = 0.0f;
        _isTimeOver = false;
        _isGameClearCalled = false;

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

        // 이미 Game Over, Stage Clear 등 Playing 상태가 아니면 추가 처리하지 않습니다.
        if (GameManager.instance.currentGameState != GameState.Playing)
        {
            return;
        }

        // 시간이 종료되었고, 아직 StageClear 처리를 호출하지 않았을 때 (Stage 1~3: StageClear, Stage 4: GameClear)
        if (_isTimeOver && !_isGameClearCalled)
        {
            // 플레이어 관련 상태 처리 (예: 움직임 멈춤)
            if (_playerCnt != null)
            {
                _playerCnt.GameStop();
            }

            // GameManager에 Stage Clear 처리를 요청합니다.
            // GameManager가 현재 Stage Index를 확인하여 GameClear/StageClear를 결정합니다.
            GameManager.instance.HandleStageClear();

            _isGameClearCalled = true; // 중복 호출 방지
        }
    }
}