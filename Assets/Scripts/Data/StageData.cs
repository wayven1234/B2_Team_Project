using System;
using UnityEngine;

[System.Serializable]

public class StageData
{
    public enum StageType { Normal, Vertical }

    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject enemyPrefab;

        [Range(0f, 1f)]
        public float spawnChance = 1f;
    }

    [Header("Stage Type 설정")]
    public StageType stageType;

    [Header("Spawn Enemy 목록")]
    public EnemySpawnData[] enemies;

    [Header("Spawn 타이밍 설정 (공통/Normal)")]
    public float normalInitialDelay = 5f;
    public float normalSpawnInterval = 2f;

    [Header("Vertical Stage 전용")]
    public float verticalInitialDelay = 5f;
    public float verticalSpawnInterval = 2f;

    [Header("Boss Enemy 설정")]
    public GameObject bossEnemyPrefab;
    public float bossSpawnTime = 150f;
}
