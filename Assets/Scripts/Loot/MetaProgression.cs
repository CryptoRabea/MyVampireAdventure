using System.Collections.Generic;
using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Loot
{
    /// <summary>
    /// Manages persistent meta-progression upgrades
    /// Unlocks, permanent stat boosts, character upgrades, etc.
    /// </summary>
    public class MetaProgression : MonoBehaviour
    {
        [System.Serializable]
        public class MetaUpgrade
        {
            public string upgradeName;
            public string description;
            public int cost;
            public ProgressionCurrency costType;
            public int currentLevel;
            public int maxLevel;
            public MetaUpgradeType upgradeType;
            public float valuePerLevel;
        }

        public enum MetaUpgradeType
        {
            MaxHealth,
            Damage,
            Speed,
            CritChance,
            CritDamage,
            StartingGold,
            LootQuality,
            ExperienceGain
        }

        [Header("Available Upgrades")]
        [SerializeField] private List<MetaUpgrade> availableUpgrades = new List<MetaUpgrade>();

        [Header("Current Currency")]
        private Dictionary<ProgressionCurrency, int> currencies = new Dictionary<ProgressionCurrency, int>()
        {
            { ProgressionCurrency.Souls, 0 },
            { ProgressionCurrency.Essence, 0 },
            { ProgressionCurrency.Fragments, 0 }
        };

        private void OnEnable()
        {
            GameEvents.OnCurrencyGained += HandleCurrencyGained;
        }

        private void OnDisable()
        {
            GameEvents.OnCurrencyGained -= HandleCurrencyGained;
        }

        private void Start()
        {
            InitializeDefaultUpgrades();
        }

        /// <summary>
        /// Initialize default meta upgrades
        /// </summary>
        private void InitializeDefaultUpgrades()
        {
            if (availableUpgrades.Count > 0) return;

            availableUpgrades.Add(new MetaUpgrade
            {
                upgradeName = "Fortitude",
                description = "Increase maximum health",
                cost = 10,
                costType = ProgressionCurrency.Essence,
                currentLevel = 0,
                maxLevel = 10,
                upgradeType = MetaUpgradeType.MaxHealth,
                valuePerLevel = 10f
            });

            availableUpgrades.Add(new MetaUpgrade
            {
                upgradeName = "Might",
                description = "Increase damage dealt",
                cost = 15,
                costType = ProgressionCurrency.Essence,
                currentLevel = 0,
                maxLevel = 10,
                upgradeType = MetaUpgradeType.Damage,
                valuePerLevel = 0.05f // 5% per level
            });

            availableUpgrades.Add(new MetaUpgrade
            {
                upgradeName = "Swiftness",
                description = "Increase movement speed",
                cost = 12,
                costType = ProgressionCurrency.Essence,
                currentLevel = 0,
                maxLevel = 5,
                upgradeType = MetaUpgradeType.Speed,
                valuePerLevel = 0.5f
            });
        }

        /// <summary>
        /// Purchase an upgrade
        /// </summary>
        public bool PurchaseUpgrade(string upgradeName)
        {
            MetaUpgrade upgrade = availableUpgrades.Find(u => u.upgradeName == upgradeName);
            if (upgrade == null)
            {
                Debug.LogWarning($"[MetaProgression] Upgrade not found: {upgradeName}");
                return false;
            }

            // Check if at max level
            if (upgrade.currentLevel >= upgrade.maxLevel)
            {
                Debug.LogWarning($"[MetaProgression] Upgrade already at max level: {upgradeName}");
                return false;
            }

            // Check currency
            int cost = GetUpgradeCost(upgrade);
            if (!HasCurrency(upgrade.costType, cost))
            {
                Debug.LogWarning($"[MetaProgression] Not enough {upgrade.costType} for {upgradeName}");
                return false;
            }

            // Purchase
            SpendCurrency(upgrade.costType, cost);
            upgrade.currentLevel++;

            GameEvents.UpgradePurchased(upgradeName);
            Debug.Log($"[MetaProgression] Purchased {upgradeName} level {upgrade.currentLevel}");

            return true;
        }

        /// <summary>
        /// Get upgrade cost (can scale with level)
        /// </summary>
        private int GetUpgradeCost(MetaUpgrade upgrade)
        {
            return upgrade.cost + (upgrade.currentLevel * 5); // Cost increases by 5 per level
        }

        /// <summary>
        /// Get total stat bonus from upgrades
        /// </summary>
        public float GetStatBonus(MetaUpgradeType statType)
        {
            float total = 0f;

            foreach (MetaUpgrade upgrade in availableUpgrades)
            {
                if (upgrade.upgradeType == statType)
                {
                    total += upgrade.valuePerLevel * upgrade.currentLevel;
                }
            }

            return total;
        }

        /// <summary>
        /// Handle currency gained events
        /// </summary>
        private void HandleCurrencyGained(ProgressionCurrency type, int amount)
        {
            if (!currencies.ContainsKey(type))
            {
                currencies[type] = 0;
            }

            currencies[type] += amount;
            Debug.Log($"[MetaProgression] Gained {amount} {type}. Total: {currencies[type]}");
        }

        /// <summary>
        /// Check if player has enough currency
        /// </summary>
        public bool HasCurrency(ProgressionCurrency type, int amount)
        {
            return currencies.ContainsKey(type) && currencies[type] >= amount;
        }

        /// <summary>
        /// Spend currency
        /// </summary>
        public bool SpendCurrency(ProgressionCurrency type, int amount)
        {
            if (!HasCurrency(type, amount)) return false;

            currencies[type] -= amount;
            return true;
        }

        /// <summary>
        /// Get currency amount
        /// </summary>
        public int GetCurrency(ProgressionCurrency type)
        {
            return currencies.ContainsKey(type) ? currencies[type] : 0;
        }

        /// <summary>
        /// Get save data for meta progression
        /// </summary>
        public MetaProgressionSaveData GetSaveData()
        {
            return new MetaProgressionSaveData
            {
                upgrades = new List<MetaUpgrade>(availableUpgrades),
                currencies = new Dictionary<ProgressionCurrency, int>(currencies)
            };
        }

        /// <summary>
        /// Load save data
        /// </summary>
        public void LoadSaveData(MetaProgressionSaveData data)
        {
            if (data == null) return;

            availableUpgrades = new List<MetaUpgrade>(data.upgrades);
            currencies = new Dictionary<ProgressionCurrency, int>(data.currencies);
        }
    }

    [System.Serializable]
    public class MetaProgressionSaveData
    {
        public List<MetaProgression.MetaUpgrade> upgrades;
        public Dictionary<ProgressionCurrency, int> currencies;
    }
}
