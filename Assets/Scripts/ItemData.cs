using UnityEngine;

[System.Serializable]
public class ItemData
{
    public enum ItemType {Tray, Book, Bar}

    [Header("아이템 설정")]
    public ItemType type;       // 아이템
    public string display;      // 아이템 이름
    public Sprite icon;         // 아이템 이미지

    [Header("레벨당 속성")]
    public float baseDamage;
    public float baseSpeed;
    public float baseDelay;

    public float[] damages;
    public int[] speeds;
}