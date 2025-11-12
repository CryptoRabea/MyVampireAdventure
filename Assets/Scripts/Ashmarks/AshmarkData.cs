using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Ashmarks
{
    /// <summary>
    /// ScriptableObject defining Ashmark (ability/equipment) properties
    /// Create via: Assets > Create > Vampire Survivor > Ashmark Data
    /// </summary>
    [CreateAssetMenu(fileName = "New Ashmark", menuName = "Vampire Survivor/Ashmark Data", order = 3)]
    public class AshmarkData : ScriptableObject
    {
        [Header("Basic Info")]
        public string ashmarkName = "New Ashmark";
        [TextArea(3, 6)]
        public string description = "Ashmark description";
        public Sprite icon;
        public AshmarkType type = AshmarkType.Active;
        public AshmarkRarity rarity = AshmarkRarity.Common;

        [Header("Activation")]
        public float cooldown = 10f;
        public float duration = 5f; // For active abilities
        public bool isPermanent = false; // Permanent passive effect

        [Header("Stat Modifiers (Passive)")]
        public float healthModifier = 0f;
        public float damageModifier = 0f; // Multiplier (1.2 = +20%)
        public float speedModifier = 0f;
        public float attackSpeedModifier = 0f; // Cooldown reduction
        public float critChanceModifier = 0f;
        public float critDamageModifier = 0f;

        [Header("Active Ability Settings")]
        public GameObject abilityPrefab; // VFX or projectile
        public float abilityDamage = 50f;
        public float abilityRange = 10f;
        public float abilityRadius = 5f; // For AOE abilities

        [Header("Triggered Ability Settings")]
        public TriggerType triggerType = TriggerType.OnKill;
        [Range(0f, 1f)]
        public float triggerChance = 0.1f; // 10% chance

        [Header("Visual/Audio")]
        public GameObject activationVFXPrefab;
        public AudioClip activationSound;
        public Color rarityColor = Color.white;

        [Header("Unlock Requirements")]
        public bool isUnlocked = false;
        public int unlockCost = 100; // Essence cost
        [TextArea(2, 3)]
        public string loreText = "Ancient power...";

        public enum TriggerType
        {
            OnKill,
            OnHit,
            OnDamaged,
            OnDodge,
            OnLowHealth
        }

        /// <summary>
        /// Get rarity color
        /// </summary>
        public Color GetRarityColor()
        {
            switch (rarity)
            {
                case AshmarkRarity.Common: return Color.gray;
                case AshmarkRarity.Uncommon: return Color.green;
                case AshmarkRarity.Rare: return Color.blue;
                case AshmarkRarity.Epic: return new Color(0.6f, 0f, 1f); // Purple
                case AshmarkRarity.Legendary: return new Color(1f, 0.5f, 0f); // Orange
                default: return Color.white;
            }
        }
    }
}
