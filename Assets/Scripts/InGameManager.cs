using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    [SerializeField] private GameObject hpLvPanel;
    [SerializeField] private GameObject timePanel;
    [SerializeField] private GameObject itemPanel;
    [SerializeField] private GameObject closeButton;

    [Header("UI Scripts & Slots")]
    [SerializeField] private PlayerHealthBar healthBarScript;
    [SerializeField] private PlayerLevelBar levelBarScript;
    [SerializeField] private Image[] itemImageSlots;

    private PlayerController player;

    void Start()
    {
        hpLvPanel.SetActive(false);
        timePanel.SetActive(false);
        itemPanel.SetActive(false);
        closeButton.SetActive(false);

        // (안전 장치) 인스펙터에서 UI 스크립트 연결을 깜빡했다면 찾기
        if (healthBarScript == null && hpLvPanel != null)
            healthBarScript = hpLvPanel.GetComponentInChildren<PlayerHealthBar>(true);
        if (levelBarScript == null && hpLvPanel != null)
            levelBarScript = hpLvPanel.GetComponentInChildren<PlayerLevelBar>(true);

        StartCoroutine(InitializeUI());
    }

    IEnumerator InitializeUI()
    {
        while (PlayerController.instance == null)
        {
            yield return null;
        }

        player = PlayerController.instance;

        // [수정] 0.1f 딜레이를 제거하거나, 한 프레임만 기다리도록 수정
        //yield return new WaitForSeconds(0.1f); // 삭제

        hpLvPanel.SetActive(true);
        timePanel.SetActive(true);
        itemPanel.SetActive(true);
        closeButton.SetActive(true);

        if (player != null && healthBarScript != null)
            player.LinkUI(healthBarScript, levelBarScript, itemImageSlots);
        else
            Debug.LogError("InGameManager: Player 또는 UI 스크립트를 찾지 못했습니다");
    }
}
