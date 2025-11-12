using System.Collections.Generic;
using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Procedural
{
    /// <summary>
    /// Generates procedural dungeons using pre-designed room prefabs
    /// Creates interconnected rooms with varied content
    /// </summary>
    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Generation Settings")]
        [SerializeField] private int roomCount = 10;
        [SerializeField] private int currentFloor = 1;
        [SerializeField] private Vector2 roomSpacing = new Vector2(25f, 25f);

        [Header("Room Pools")]
        [SerializeField] private List<RoomData> combatRooms = new List<RoomData>();
        [SerializeField] private List<RoomData> lootRooms = new List<RoomData>();
        [SerializeField] private List<RoomData> shopRooms = new List<RoomData>();
        [SerializeField] private List<RoomData> bossRooms = new List<RoomData>();

        [Header("Generation Rules")]
        [Range(0f, 1f)]
        [SerializeField] private float combatRoomChance = 0.7f;
        [Range(0f, 1f)]
        [SerializeField] private float lootRoomChance = 0.15f;
        [Range(0f, 1f)]
        [SerializeField] private float shopRoomChance = 0.1f;

        [Header("References")]
        [SerializeField] private Transform roomContainer;

        private List<Room> generatedRooms = new List<Room>();
        private Dictionary<Vector2Int, Room> roomGrid = new Dictionary<Vector2Int, Room>();
        private Room currentRoom;

        public List<Room> GeneratedRooms => generatedRooms;
        public Room CurrentRoom => currentRoom;

        private void Start()
        {
            if (roomContainer == null)
            {
                roomContainer = new GameObject("RoomContainer").transform;
            }
        }

        /// <summary>
        /// Generate a new dungeon floor
        /// </summary>
        public void GenerateDungeon(int floor)
        {
            currentFloor = floor;

            // Clear existing dungeon
            ClearDungeon();

            // Generate layout
            List<Vector2Int> roomPositions = GenerateLayout();

            // Create rooms
            for (int i = 0; i < roomPositions.Count; i++)
            {
                Vector2Int gridPos = roomPositions[i];
                RoomType roomType = DetermineRoomType(i, roomPositions.Count);
                RoomData roomData = SelectRoomData(roomType);

                if (roomData != null)
                {
                    CreateRoom(roomData, gridPos, i == 0);
                }
            }

            Debug.Log($"[DungeonGenerator] Generated {generatedRooms.Count} rooms for floor {floor}");
        }

        /// <summary>
        /// Generate dungeon layout using simple branching algorithm
        /// </summary>
        private List<Vector2Int> GenerateLayout()
        {
            List<Vector2Int> positions = new List<Vector2Int>();
            HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();

            // Start at origin
            Vector2Int current = Vector2Int.zero;
            positions.Add(current);
            occupied.Add(current);

            // Generate path
            for (int i = 1; i < roomCount; i++)
            {
                // Try to find valid neighbor
                List<Vector2Int> neighbors = GetNeighbors(current);
                neighbors.RemoveAll(n => occupied.Contains(n));

                if (neighbors.Count > 0)
                {
                    // Choose random neighbor
                    Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];
                    positions.Add(next);
                    occupied.Add(next);
                    current = next;
                }
                else
                {
                    // Backtrack if stuck
                    current = positions[Random.Range(0, positions.Count)];
                }

                // Occasionally branch
                if (Random.value < 0.3f && i < roomCount - 1)
                {
                    current = positions[Random.Range(0, positions.Count)];
                }
            }

            return positions;
        }

        /// <summary>
        /// Get neighboring grid positions
        /// </summary>
        private List<Vector2Int> GetNeighbors(Vector2Int pos)
        {
            return new List<Vector2Int>
            {
                pos + Vector2Int.up,
                pos + Vector2Int.down,
                pos + Vector2Int.left,
                pos + Vector2Int.right
            };
        }

        /// <summary>
        /// Determine room type based on position in dungeon
        /// </summary>
        private RoomType DetermineRoomType(int index, int totalRooms)
        {
            // First room is always safe
            if (index == 0) return RoomType.Safe;

            // Last room is boss
            if (index == totalRooms - 1) return RoomType.Boss;

            // Random room type based on chances
            float roll = Random.value;
            if (roll < combatRoomChance)
                return RoomType.Combat;
            else if (roll < combatRoomChance + lootRoomChance)
                return RoomType.Loot;
            else if (roll < combatRoomChance + lootRoomChance + shopRoomChance)
                return RoomType.Shop;
            else
                return RoomType.Combat; // Default fallback
        }

        /// <summary>
        /// Select appropriate room data based on type
        /// </summary>
        private RoomData SelectRoomData(RoomType type)
        {
            List<RoomData> pool = type switch
            {
                RoomType.Combat => combatRooms,
                RoomType.Loot => lootRooms,
                RoomType.Shop => shopRooms,
                RoomType.Boss => bossRooms,
                _ => combatRooms
            };

            if (pool.Count == 0)
            {
                Debug.LogWarning($"[DungeonGenerator] No rooms available for type {type}");
                return null;
            }

            // Filter by floor difficulty
            List<RoomData> validRooms = pool.FindAll(r => r.minimumFloor <= currentFloor);
            if (validRooms.Count == 0) validRooms = pool;

            return validRooms[Random.Range(0, validRooms.Count)];
        }

        /// <summary>
        /// Create a room instance
        /// </summary>
        private void CreateRoom(RoomData data, Vector2Int gridPos, bool isStartRoom)
        {
            Vector3 worldPos = new Vector3(gridPos.x * roomSpacing.x, gridPos.y * roomSpacing.y, 0);

            GameObject roomObj = Instantiate(data.roomPrefab, worldPos, Quaternion.identity, roomContainer);
            roomObj.name = $"Room_{gridPos.x}_{gridPos.y}_{data.roomType}";

            Room room = roomObj.GetComponent<Room>();
            if (room == null)
            {
                room = roomObj.AddComponent<Room>();
            }

            room.Initialize(data, currentFloor, isStartRoom);

            generatedRooms.Add(room);
            roomGrid[gridPos] = room;

            if (isStartRoom)
            {
                currentRoom = room;
                room.EnterRoom();
            }
            else
            {
                room.gameObject.SetActive(false); // Deactivate until player enters
            }
        }

        /// <summary>
        /// Clear existing dungeon
        /// </summary>
        public void ClearDungeon()
        {
            foreach (Room room in generatedRooms)
            {
                if (room != null)
                {
                    Destroy(room.gameObject);
                }
            }

            generatedRooms.Clear();
            roomGrid.Clear();
            currentRoom = null;
        }

        /// <summary>
        /// Move to adjacent room
        /// </summary>
        public void MoveToRoom(Vector2Int direction)
        {
            if (currentRoom == null) return;

            // Calculate target grid position
            // This would need to track room grid positions
            // Simplified for now
        }
    }

    /// <summary>
    /// Individual room instance
    /// </summary>
    public class Room : MonoBehaviour
    {
        private RoomData data;
        private int floor;
        private bool isStartRoom;
        private bool isCleared = false;
        private List<GameObject> spawnedEnemies = new List<GameObject>();

        public RoomData Data => data;
        public bool IsCleared => isCleared;
        public RoomType Type => data != null ? data.roomType : RoomType.Combat;

        public void Initialize(RoomData roomData, int floorNumber, bool startRoom)
        {
            data = roomData;
            floor = floorNumber;
            isStartRoom = startRoom;
        }

        /// <summary>
        /// Called when player enters the room
        /// </summary>
        public void EnterRoom()
        {
            gameObject.SetActive(true);
            GameEvents.RoomEntered(Type);

            if (!isCleared && Type == RoomType.Combat)
            {
                SpawnEnemies();
            }
        }

        /// <summary>
        /// Spawn enemies in the room
        /// </summary>
        private void SpawnEnemies()
        {
            if (data == null || data.enemySpawns.Count == 0) return;

            int enemyCount = Random.Range(data.minEnemies, data.maxEnemies + 1);

            for (int i = 0; i < enemyCount; i++)
            {
                // Select random enemy from spawn data
                RoomData.EnemySpawnData spawnData = SelectEnemySpawn();
                if (spawnData?.enemyData == null) continue;

                // Spawn position (random within room bounds or defined spawn point)
                Vector3 spawnPos = transform.position + (Vector3)spawnData.spawnPosition;
                spawnPos += new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);

                // Instantiate enemy
                // In production, this would use object pooling
                GameObject enemyPrefab = spawnData.enemyData.sprite != null
                    ? CreateEnemyPrefab(spawnData.enemyData)
                    : null;

                if (enemyPrefab != null)
                {
                    GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);
                    spawnedEnemies.Add(enemy);
                }
            }

            // Subscribe to enemy death events to track room clear
            InvokeRepeating(nameof(CheckRoomClear), 1f, 0.5f);
        }

        private RoomData.EnemySpawnData SelectEnemySpawn()
        {
            float totalWeight = 0f;
            foreach (var spawn in data.enemySpawns)
            {
                totalWeight += spawn.spawnWeight;
            }

            float roll = Random.Range(0f, totalWeight);
            float current = 0f;

            foreach (var spawn in data.enemySpawns)
            {
                current += spawn.spawnWeight;
                if (roll <= current)
                {
                    return spawn;
                }
            }

            return data.enemySpawns[0];
        }

        private void CheckRoomClear()
        {
            spawnedEnemies.RemoveAll(e => e == null);

            if (spawnedEnemies.Count == 0 && !isCleared)
            {
                isCleared = true;
                CancelInvoke(nameof(CheckRoomClear));
                GameEvents.RoomCleared();
                Debug.Log($"[Room] Room cleared: {data.roomName}");
            }
        }

        private GameObject CreateEnemyPrefab(Enemies.EnemyData enemyData)
        {
            // This is a placeholder - in production you'd have proper enemy prefabs
            // For now, create a simple enemy GameObject
            return new GameObject($"Enemy_{enemyData.enemyName}");
        }
    }
}
