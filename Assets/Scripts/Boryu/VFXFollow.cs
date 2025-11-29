using UnityEngine;

public class VFXFollow : MonoBehaviour
{
    private Transform target;
    private Vector3 positionOffset;

    /// <summary>
    /// 이펙트가 따라갈 목표와 초기 오프셋을 설정합니다.
    /// </summary>
    public void SetupFollow(Transform targetToFollow, Vector3 offset)
    {
        target = targetToFollow;
        positionOffset = offset;
    }

    void LateUpdate()
    {
        // 플레이어가 파괴되었는지 확인합니다. (씬 전환 등)
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // LateUpdate에서 위치를 업데이트하여 플레이어의 이동 후 프레임에 맞춥니다.
        transform.position = target.position + positionOffset;
    }
}