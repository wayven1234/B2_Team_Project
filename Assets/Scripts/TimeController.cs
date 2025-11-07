using UnityEngine;

public class TimeController : MonoBehaviour
{
    public bool _isCountDown = true;
    public float gameTime;
    public bool _isTimeOver = false;
    public float _displayTime;

    private float _times;

    private void Start()
    {
        if (_isCountDown)
            _displayTime = gameTime;
    }

    private void Update()
    {
        TimeCountDown();
    }

    void TimeCountDown()
    {
        if (_isTimeOver == false)
        {
            _times += Time.deltaTime;
            if (_isCountDown)
            {
                _displayTime = gameTime - _times;
                if (_displayTime <= 0.0f)
                {
                    _displayTime = 0.0f;
                    _isTimeOver = true;
                }
            }
            else
            {
                _displayTime = _times;
                if (_displayTime >= gameTime)
                {
                    _displayTime = gameTime;
                    _isTimeOver = true;
                }
            }
        }
    }
}
