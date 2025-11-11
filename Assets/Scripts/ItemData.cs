using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string display;
    public Sprite icon;
    public ItemType type;
    public float value;

    public enum ItemType
    {
    }

}