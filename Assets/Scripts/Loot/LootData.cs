using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Loot
{
    /// <summary>
    /// ScriptableObject defining loot drop properties
    /// Create via: Assets > Create > Vampire Survivor > Loot Data
    /// </summary>
    [CreateAssetMenu(fileName = "New Loot", menuName = "Vampire Survivor/Loot Data", order = 5)]
    public class LootData : ScriptableObject
    {
        [Header("Basic Info")]
        public string lootName = "New Loot";
        public LootType lootType = LootType.Currency;
        public Sprite icon;

        [Header("Currency")]
        public ProgressionCurrency currencyType = ProgressionCurrency.Souls;
        public int minAmount = 1;
        public int maxAmount = 5;

        [Header("Item References")]
        public Weapons.WeaponData weaponData;
        public Ashmarks.AshmarkData ashmarkData;
        public Narrative.CodexEntryData codexEntry;

        [Header("Drop Settings")]
        [Range(0f, 1f)]
        public float dropChance = 0.1f;
        public GameObject lootPrefab; // Visual pickup object

        [Header("Magnet Settings")]
        public bool autoCollect = false;
        public float magnetRange = 5f;
        public float magnetSpeed = 10f;
    }
}
