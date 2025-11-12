using System.Collections.Generic;
using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Loot
{
    /// <summary>
    /// Manages loot drops and pickups during runs
    /// Handles currency, weapon drops, and Ashmark drops
    /// </summary>
    public class LootManager : MonoBehaviour
    {
        [Header("Loot Tables")]
        [SerializeField] private List<LootData> commonLoot = new List<LootData>();
        [SerializeField] private List<LootData> rareLoot = new List<LootData>();
        [SerializeField] private List<LootData> epicLoot = new List<LootData>();

        [Header("Settings")]
        [SerializeField] private float lootDespawnTime = 30f;
        [SerializeField] private Transform lootContainer;

        private List<GameObject> activeLoot = new List<GameObject>();

        private void Awake()
        {
            if (lootContainer == null)
            {
                lootContainer = new GameObject("LootContainer").transform;
                lootContainer.SetParent(transform);
            }
        }

        private void OnEnable()
        {
            GameEvents.OnEnemyKilled += HandleEnemyKilled;
        }

        private void OnDisable()
        {
            GameEvents.OnEnemyKilled -= HandleEnemyKilled;
        }

        /// <summary>
        /// Handle enemy death and spawn loot
        /// </summary>
        private void HandleEnemyKilled(GameObject enemy, float damage, Vector3 position)
        {
            // Always drop experience orbs (handled by enemy)
            // Randomly drop other loot
            if (Random.value <= 0.2f) // 20% chance for loot
            {
                SpawnLoot(position);
            }
        }

        /// <summary>
        /// Spawn loot at position
        /// </summary>
        public void SpawnLoot(Vector3 position, LootData specificLoot = null)
        {
            LootData lootData = specificLoot ?? SelectRandomLoot();

            if (lootData == null || lootData.lootPrefab == null)
            {
                // Spawn default currency
                SpawnCurrency(position, ProgressionCurrency.Souls, Random.Range(1, 5));
                return;
            }

            GameObject lootObj = Instantiate(lootData.lootPrefab, position, Quaternion.identity, lootContainer);
            LootPickup pickup = lootObj.GetComponent<LootPickup>();

            if (pickup == null)
            {
                pickup = lootObj.AddComponent<LootPickup>();
            }

            pickup.Initialize(lootData);
            activeLoot.Add(lootObj);

            // Auto-despawn after time
            Destroy(lootObj, lootDespawnTime);
        }

        /// <summary>
        /// Spawn currency directly
        /// </summary>
        public void SpawnCurrency(Vector3 position, ProgressionCurrency type, int amount)
        {
            // Create simple currency pickup
            GameObject currencyObj = new GameObject($"Currency_{type}");
            currencyObj.transform.position = position;
            currencyObj.transform.SetParent(lootContainer);

            // Add sprite renderer
            SpriteRenderer sr = currencyObj.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 10;

            // Add collider
            CircleCollider2D col = currencyObj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;

            // Add pickup component
            CurrencyPickup pickup = currencyObj.AddComponent<CurrencyPickup>();
            pickup.Initialize(type, amount);

            activeLoot.Add(currencyObj);
            Destroy(currencyObj, lootDespawnTime);
        }

        /// <summary>
        /// Select random loot from tables
        /// </summary>
        private LootData SelectRandomLoot()
        {
            float roll = Random.value;

            List<LootData> selectedTable;
            if (roll < 0.7f)
                selectedTable = commonLoot;
            else if (roll < 0.95f)
                selectedTable = rareLoot;
            else
                selectedTable = epicLoot;

            if (selectedTable.Count == 0) return null;

            return selectedTable[Random.Range(0, selectedTable.Count)];
        }

        /// <summary>
        /// Clear all active loot
        /// </summary>
        public void ClearAllLoot()
        {
            foreach (GameObject loot in activeLoot)
            {
                if (loot != null) Destroy(loot);
            }
            activeLoot.Clear();
        }
    }

    /// <summary>
    /// Pickup component for loot items
    /// </summary>
    public class LootPickup : MonoBehaviour
    {
        private LootData data;
        private Transform player;
        private bool isBeingMagnetized = false;

        public void Initialize(LootData lootData)
        {
            data = lootData;
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        private void Update()
        {
            if (player == null || data == null) return;

            float distance = Vector3.Distance(transform.position, player.position);

            // Auto-collect or magnet towards player
            if (data.autoCollect || distance <= data.magnetRange)
            {
                isBeingMagnetized = true;
                Vector3 direction = (player.position - transform.position).normalized;
                transform.position += direction * data.magnetSpeed * Time.deltaTime;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            CollectLoot(other.gameObject);
        }

        private void CollectLoot(GameObject collector)
        {
            if (data == null) return;

            switch (data.lootType)
            {
                case LootType.Currency:
                    int amount = Random.Range(data.minAmount, data.maxAmount + 1);
                    GameEvents.CurrencyGained(data.currencyType, amount);
                    break;

                case LootType.Weapon:
                    if (data.weaponData != null)
                    {
                        // Try to equip weapon
                        Player.PlayerCombat combat = collector.GetComponent<Player.PlayerCombat>();
                        if (combat != null && combat.HasWeaponSlot)
                        {
                            combat.EquipWeapon(data.weaponData);
                        }
                    }
                    break;

                case LootType.Ashmark:
                    if (data.ashmarkData != null)
                    {
                        // Offer Ashmark to player (usually via UI)
                        GameEvents.LootCollected(LootType.Ashmark, data.ashmarkData.ashmarkName);
                    }
                    break;

                case LootType.Codex:
                    if (data.codexEntry != null)
                    {
                        GameEvents.CodexUnlocked(data.codexEntry.entryId);
                    }
                    break;
            }

            GameEvents.LootCollected(data.lootType, data.lootName);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Simple currency pickup
    /// </summary>
    public class CurrencyPickup : MonoBehaviour
    {
        private ProgressionCurrency currencyType;
        private int amount;

        public void Initialize(ProgressionCurrency type, int value)
        {
            currencyType = type;
            amount = value;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GameEvents.CurrencyGained(currencyType, amount);
                Destroy(gameObject);
            }
        }
    }
}
