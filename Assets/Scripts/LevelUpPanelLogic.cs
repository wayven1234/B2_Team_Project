using UnityEngine;
using UnityEngine.UI;

public class LevelUpPanelLogic : MonoBehaviour
{
    [SerializeField] private Button LevelUpPanelButton;

    private static int panelOpenCount = 0;

    void OnEnable()
    {
        panelOpenCount++;
        if (panelOpenCount == 1)
        {
            LevelUpPanelButton.gameObject.SetActive(false);
        }
        else if (panelOpenCount > 1)
        {
            LevelUpPanelButton.gameObject.SetActive(true);
        }
    }

    public static void ResetOpenCount()
    {
        panelOpenCount = 0;
    }
}
