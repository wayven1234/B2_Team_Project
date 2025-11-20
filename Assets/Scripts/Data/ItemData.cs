using UnityEngine;

[System.Serializable]
public class ItemData
{
    public enum ItemType
    {
        Talk = 0,
        Book,
        Bar
    }

    [Header("아이템 설정")]
    public ItemType type;       // 아이템
    public string display;      // 아이템 이름
    public Sprite icon;         // 아이템 이미지
    [HideInInspector] public int level;           // 아이템 레벨
    public int maxLevel = 4;    // 최대 레벨 (4로 고정)


    [Header("레벨당 속성")]
    public float baseDamage;
    public float baseSpeed;

    public float[] damages;
    public float[] speeds;

    [Header("공격 아이템 Prefab")]
    public GameObject bookPrefab;
    public GameObject talkPrefab;
    public GameObject barPrefab;

    [Header("Talk 사거리 속성")]
    public float rangeIncreasePerLevel = 3f;
    public float baseRange = 3f;
    public float maxRange = 30f;
}