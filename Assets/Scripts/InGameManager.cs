using System.Collections;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    [SerializeField] private GameObject hpLvPanel;
    [SerializeField] private GameObject timePanel;
    [SerializeField] private GameObject itemPanel;
    [SerializeField] private GameObject closeButton;

    public PlayerController player;

    private bool isUiActive = false;

    void Start()
    {
        hpLvPanel.SetActive(false);
        timePanel.SetActive(false);
        itemPanel.SetActive(false);
        closeButton.SetActive(false);
    }
    
    void Update()
    {
        if (isUiActive) return;

        SetActiveTrue();
    }

    void SetActiveTrue()
    {
        if (player == null)
            player = FindFirstObjectByType<PlayerController>();

        if (player != null && player.gameObject.activeSelf)
        {
            StartCoroutine(AcitveTrue());

            isUiActive = true;
        }
    }

    IEnumerator AcitveTrue()
    {
        yield return new WaitForSeconds(0.5f);

        hpLvPanel.SetActive(true);
        timePanel.SetActive(true);
        itemPanel.SetActive(true);
        closeButton.SetActive(true);
    }
}
