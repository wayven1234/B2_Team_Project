using UnityEngine;
using UnityEngine.Ui;

public class ItemPrefab : MonoBehaviour
{
    [SerializeField] private GameObject itemUiPrefab;
    [SerializeField] private ItemData itemdata;
    Image image;
    TMP_Text text;
    void Awake()
    {
        image = GetComponentInChildren<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }


    void Start()
    {
        
    }
}
