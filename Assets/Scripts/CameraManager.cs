using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float _leftLimit;
    [SerializeField] private float _rightLimit;
    [SerializeField] private float _topLimit;
    [SerializeField] private float _bottomLimit;

    public bool _isForceScrollX = false;
    public float _forceScrollSpeedX;
    public bool _isForceScrollY = false;
    public float _forceScrollSpeedY;

    void Update()
    {
        CameraScroll();
    }
    
    void CameraScroll()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            float x = player.transform.position.x;
            float y = player.transform.position.y;
            float z = transform.position.z;

            if (_isForceScrollX)
                x = transform.position.x + (_forceScrollSpeedX * Time.deltaTime);

            if (x < _leftLimit)
                x = _leftLimit;
            else if (x > _rightLimit)
                x = _rightLimit;

            if (_isForceScrollY)
                y = transform.position.y + (_forceScrollSpeedY * Time.deltaTime);

            if (y < _bottomLimit)
                y = _bottomLimit;
            else if (y > _topLimit)
                y = _topLimit;

            Vector3 v3 = new Vector3(x, y, z);
            transform.position = v3;
        }
    }
}
