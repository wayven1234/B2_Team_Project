using UnityEngine;

public class GameManager : MonoBehaviour
{
    private TimeController _timeCnt;
    public GameObject _timeBar;
    public GameObject _timeText;

    private void Start()
    {
        TimeCntBar(); // 시간 제한이 없으면 시간 표시 숨기기
    }

    private void Update()
    {
        
    }

    void TimeCntBar()
    {
        _timeCnt = GetComponent<TimeController>();
        if (_timeCnt != null)
        {
            if (_timeCnt.gameTime == 0.0f)
                _timeBar.SetActive(false);
        }
    }
}
