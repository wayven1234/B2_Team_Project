using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InGameManager : MonoBehaviour
{
    // 필드를 private으로 변경하고 코드로 찾도록 합니다.
    private GameObject hpLvPanel;
    private GameObject timePanel;
    private GameObject closeButton;

    // 스크립트/슬롯 변수
    private PlayerHealthBar healthBarScript;
    private PlayerLevelBar levelBarScript;
    private Image[] itemImageSlots;

    private PlayerController player;
    private Canvas persistentCanvas;

    private void Awake()
    {
        // 1. Persistent Canvas 찾기
        persistentCanvas = FindFirstObjectByType<PersistentPanel>(FindObjectsInactive.Include)?.GetComponent<Canvas>();
        if (persistentCanvas == null)
        {
            Debug.LogError("InGameManager: Persistent Canvas를 찾을 수 없습니다! UI 연결 실패.");
            return;
        }

        // 2. UI 요소 이름으로 찾기 (Hierarchy 이름을 확인하고 수정)
        hpLvPanel = FindChildRecursive(persistentCanvas.transform, "HPLVPanel");
        timePanel = FindChildRecursive(persistentCanvas.transform, "Time");
        closeButton = FindChildRecursive(persistentCanvas.transform, "CloseButton");

        // 3. UI 스크립트 컴포넌트 찾기
        healthBarScript = FindFirstObjectByType<PlayerHealthBar>(FindObjectsInactive.Include);
        levelBarScript = FindFirstObjectByType<PlayerLevelBar>(FindObjectsInactive.Include);

        // 4. [핵심 수정] 아이템 슬롯 Image 배열 찾기 (ItemPanel -> HealthItem -> Slot 구조 가정)
        GameObject itemPanelContainer = FindChildRecursive(persistentCanvas.transform, "ItemPanel");

        if (itemPanelContainer != null)
        {
            // ItemPanel의 하위 자식들(HealthItem -> Slot)에 있는 모든 Image 컴포넌트를 가져옵니다.
            itemImageSlots = itemPanelContainer.GetComponentsInChildren<Image>(true)
                                               // ItemPanel 자체나 HealthItem 컨테이너에 Image가 붙어있을 경우 제외합니다.
                                               .Where(img => img.transform.parent.name.Contains("Slot") || img.transform.parent.name.Contains("HealthItem"))
                                               .OrderBy(img => img.name)
                                               .ToArray();

            Debug.Log($"[InGameManager] Item Image Slot {itemImageSlots.Length}개 찾음.");

            if (itemImageSlots.Length == 0)
            {
                Debug.LogError("ItemPanel 하위에서 Item Image 슬롯(Image 컴포넌트)을 찾을 수 없습니다! Hierarchy 구조를 다시 확인하세요.");
            }
        }
        else
        {
            Debug.LogError("아이템 슬롯 부모 오브젝트 (이름: 'ItemPanel')를 찾지 못했습니다. Item Image 연결 실패.");
        }
    }

    void Start()
    {
        if (persistentCanvas == null) return;

        // 찾은 UI 요소들을 초기 비활성화 상태로 만듭니다.
        if (hpLvPanel != null) hpLvPanel.SetActive(false);
        if (timePanel != null) timePanel.SetActive(false);
        if (closeButton != null) closeButton.SetActive(false);

        StartCoroutine(InitializeUI());
    }

    IEnumerator InitializeUI()
    {
        while (PlayerController.instance == null)
        {
            yield return null;
        }

        PlayerController player = PlayerController.instance;

        // UI 활성화
        if (hpLvPanel != null) hpLvPanel.SetActive(true);
        if (timePanel != null) timePanel.SetActive(true);
        if (closeButton != null) closeButton.SetActive(true);

        // PlayerController에게 UI 연결
        if (player != null && healthBarScript != null)
        {
            player.LinkUI(healthBarScript, levelBarScript, itemImageSlots);
        }
        else
        {
            Debug.LogError("InGameManager: Player 또는 UI 스크립트를 찾지 못했습니다");
        }
    }

    // [추가] 재귀적으로 자식 오브젝트를 찾는 헬퍼 함수
    private GameObject FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child.gameObject;
            GameObject found = FindChildRecursive(child, name);
            if (found != null) return found;
        }
        return null;
    }
}