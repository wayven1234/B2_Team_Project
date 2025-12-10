using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Item Data", menuName = "Custom/Item Data")]
public class ItemData
{
    public enum ItemType
    {
        Talk = 0,
        Book,
        Bar
    }

    [Header("아이템 설정")]
    public ItemType type;
    public string display;
    public Sprite icon;
    [HideInInspector] public int level;
    public int maxLevel = 4;


    [Header("레벨당 속성")]
    public float baseDamage;
    public float baseSpeed;

    public float[] damages;
    public float[] speeds;

    [Header("공격 아이템 Prefab")]
    public GameObject bookPrefab;
    public GameObject talkPrefab;
    public GameObject barPrefab;

    [Header("Visualization")]
    public GameObject barRangeVisualPrefab;
    public GameObject bookRangeVisualPrefab;

    [Header("Talk 사거리 속성")]
    public float rangeIncreasePerLevel = 3f;
    public float baseRange = 3f;
    public float maxRange = 30f;
}