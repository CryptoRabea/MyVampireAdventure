using UnityEngine;
using System.Collections.Generic;
using VampireSurvivor.Core;

namespace VampireSurvivor.Procedural
{
    /// <summary>
    /// ScriptableObject defining room properties for procedural generation
    /// Create via: Assets > Create > Vampire Survivor > Room Data
    /// </summary>
    [CreateAssetMenu(fileName = "New Room", menuName = "Vampire Survivor/Room Data", order = 4)]
    public class RoomData : ScriptableObject
    {
        [Header("Basic Info")]
        public string roomName = "New Room";
        public RoomType roomType = RoomType.Combat;
        public GameObject roomPrefab;

        [Header("Room Size")]
        public Vector2Int gridSize = new Vector2Int(10, 10); // Size in tiles
        public Vector2 worldSize = new Vector2(20f, 20f); // Actual world size

        [Header("Connections")]
        public bool hasNorthDoor = true;
        public bool hasSouthDoor = true;
        public bool hasEastDoor = true;
        public bool hasWestDoor = true;

        [Header("Enemy Spawns (for Combat rooms)")]
        public List<EnemySpawnData> enemySpawns = new List<EnemySpawnData>();
        public int minEnemies = 3;
        public int maxEnemies = 8;

        [Header("Loot (for Loot rooms)")]
        public int guaranteedLootDrops = 1;
        public int randomLootDrops = 2;

        [Header("Difficulty")]
        [Range(1, 10)]
        public int difficultyRating = 1;
        public int minimumFloor = 1;

        [System.Serializable]
        public class EnemySpawnData
        {
            public Enemies.EnemyData enemyData;
            [Range(0f, 1f)]
            public float spawnWeight = 1f; // Higher = more likely to spawn
            public Vector2 spawnPosition; // Relative to room center
        }
    }
}
