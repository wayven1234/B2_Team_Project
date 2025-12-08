using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InGameManager : MonoBehaviour
{
    private GameObject hpLvPanel;
    private GameObject timePanel;
    private GameObject closeButton;

    private PlayerHealthBar healthBarScript;
    private PlayerLevelBar levelBarScript;
    private Image[] itemImageSlots;

    private PlayerController player;
    private Canvas persistentCanvas;

    private void Awake()
    {
        persistentCanvas = FindFirstObjectByType<PersistentPanel>(FindObjectsInactive.Include)?.GetComponent<Canvas>();
        if (persistentCanvas == null)
        {
            Debug.LogError("InGameManager: Persistent Canvas를 찾을 수 없습니다! UI 연결 실패.");
            return;
        }

        hpLvPanel = FindChildRecursive(persistentCanvas.transform, "HpLvPanel");
        timePanel = FindChildRecursive(persistentCanvas.transform, "Time");
        closeButton = FindChildRecursive(persistentCanvas.transform, "CloseButton");

        healthBarScript = FindFirstObjectByType<PlayerHealthBar>(FindObjectsInactive.Include);
        levelBarScript = FindFirstObjectByType<PlayerLevelBar>(FindObjectsInactive.Include);

        GameObject itemPanelContainer = FindChildRecursive(persistentCanvas.transform, "ItemPanel");

        if (itemPanelContainer != null)
        {
            itemImageSlots = itemPanelContainer.GetComponentsInChildren<Image>(true)
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

        if (hpLvPanel != null) hpLvPanel.SetActive(false);
        if (timePanel != null) timePanel.SetActive(false);
        if (closeButton != null) closeButton.SetActive(false);

        //StartCoroutine(InitializeUI());

        // InGameButtonManager가 PlayerController 스폰 후 PlayerController.UpdateUIFromData()를 호출할 때
        // PlayerController가 LinkUI를 스스로 호출하도록 Start()에 추가해 봅니다.
        if (PlayerController.instance != null)
        {
            LinkUIToPlayer();
        }
    }

    public void LinkUIToPlayer()
    {
        if (PlayerController.instance != null && healthBarScript != null)
        {
            PlayerController.instance.LinkUI(healthBarScript, levelBarScript, itemImageSlots);

            // UI 활성화 (이동)
            if (hpLvPanel != null) hpLvPanel.SetActive(true);
            if (timePanel != null) timePanel.SetActive(true);
            if (closeButton != null) closeButton.SetActive(true);
        }
        else
        {
            Debug.LogError("InGameManager: Player 또는 UI 스크립트를 찾지 못했습니다 (LinkUIToPlayer).");
        }
    }

    //IEnumerator InitializeUI()
    //{
    //    while (PlayerController.instance == null)
    //    {
    //        yield return null;
    //    }

    //    PlayerController player = PlayerController.instance;

    //    // UI 활성화
    //    if (hpLvPanel != null) hpLvPanel.SetActive(true);
    //    if (timePanel != null) timePanel.SetActive(true);
    //    if (closeButton != null) closeButton.SetActive(true);

    //    if (player != null && healthBarScript != null)
    //    {
    //        player.LinkUI(healthBarScript, levelBarScript, itemImageSlots);
    //    }
    //    else
    //    {
    //        Debug.LogError("InGameManager: Player 또는 UI 스크립트를 찾지 못했습니다");
    //    }
    //}

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