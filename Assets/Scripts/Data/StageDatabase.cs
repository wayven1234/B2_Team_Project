using UnityEngine;

[CreateAssetMenu(fileName = "StageDatabase", menuName = "Game Data/Stage Database")]
public class StageDatabase : ScriptableObject
{
    public StageData[] stages;
}