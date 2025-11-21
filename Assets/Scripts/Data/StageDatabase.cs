using UnityEngine;

[CreateAssetMenu(fileName = "StageDatabase", menuName = "Game Data/Stage Database")]
public class StageDatabase : ScriptableObject
{
    // Stage 1, Stage 2, Stage 3 ... 의 데이터를 담을 리스트 (배열)
    // 인덱스 0 = Stage 1 (배열 인덱스는 0부터 시작하므로)
    public StageData[] stages;
}