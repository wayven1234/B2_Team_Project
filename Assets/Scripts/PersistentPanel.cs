using UnityEngine;

public class PersistentPanel : MonoBehaviour
{
    private void Awake()
    {
        // Unity에서 권장하는 FindObjectsByType을 사용합니다.
        // SortMode.None을 사용하여 성능을 최적화합니다.
        PersistentPanel[] existingPanels = FindObjectsByType<PersistentPanel>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        // 현재 씬에서 이 스크립트를 가진 오브젝트가 1개 초과로 발견되면,
        // 현재 로드된 오브젝트를 파괴하여 중복을 방지합니다.
        if (existingPanels.Length > 1)
        {
            // 이 컴포넌트가 붙은 게임 오브젝트를 파괴합니다.
            Destroy(gameObject);
            return;
        }

        // 씬 전환 시 파괴되지 않도록 설정합니다.
        DontDestroyOnLoad(gameObject);
    }
}