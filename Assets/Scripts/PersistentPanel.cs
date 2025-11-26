using UnityEngine;

public class PersistentPanel : MonoBehaviour
{
    // 정적 변수를 선언하여 유일한 인스턴스를 저장합니다.
    private static PersistentPanel instance;

    private void Awake()
    {
        // 1. 유일성 검사 (Singleton Check)
        // 씬에서 이 타입의 다른 인스턴스가 이미 존재하는지 확인합니다.
        // FindAnyObjectByType이 Deprecated된 FindObjectOfType보다 빠릅니다.
        if (instance == null)
        {
            // 아직 인스턴스가 없다면, 현재 오브젝트를 인스턴스로 지정하고 DontDestroyOnLoad를 적용합니다.
            instance = this;

            // 이 오브젝트와 그 하위 요소들을 씬이 바뀌어도 파괴하지 않도록 설정
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // 이미 인스턴스가 존재한다면, 새로 생성된 오브젝트(중복)를 즉시 파괴합니다.
            // (새 씬이 로드될 때마다 이 오브젝트가 씬에 배치되어 있다면 실행됩니다.)
            Debug.LogWarning("PersistentPanel: 중복 인스턴스가 발견되어 파괴되었습니다.");
            Destroy(this.gameObject);
        }
    }

    // 오브젝트가 파괴될 때 정적 인스턴스를 초기화하여 씬 전환 시 오류를 방지합니다.
    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}