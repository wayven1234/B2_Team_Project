using Mono.Cecil;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private TimeController _timeCnt;
    public GameObject _timeBar;
    public GameObject _timeText;

    private void Start()
    {
        TimeCnt(); // 시간 제한이 없으면 시간 표시 숨기기
    }

    private void Update()
    {
        GameState();
    }

    void GameState()
    {
        if (PlayerController.gameState == "playing")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            PlayerController playerCnt = player.GetComponent<PlayerController>();

            if (_timeCnt != null)
            {
                if (_timeCnt.gameTime > 0.0f)
                {
                    int totalSeconds = (int)_timeCnt._displayTime;
                    int minutes = totalSeconds / 60;
                    int seconds = totalSeconds % 60;

                    _timeText.GetComponent<TextMeshProUGUI>().text = $"{minutes:D2}:{seconds:D2}";
                    if (totalSeconds == 0)
                        playerCnt.GameOver();
                }
            }
        }
        if (PlayerController.gameState == "GameClear")
        {
            PlayerController.gameState = "gameend";

            if (_timeCnt != null)
            {
                _timeCnt._isTimeOver = true;

                int time = (int)_timeCnt._displayTime;
            }
        }
        if (PlayerController.gameState == "GameOver")
        {
            PlayerController.gameState = "gameend";

            if (_timeCnt != null)
            {
                _timeCnt._isTimeOver = true;

                int time = (int)_timeCnt._displayTime;
            }
        }
    }

    void TimeCnt()
    {
        _timeCnt = GetComponent<TimeController>();
        if (_timeCnt != null)
        {
            if (_timeCnt.gameTime == 0.0f)
                _timeBar.SetActive(false);
        }
    }
}