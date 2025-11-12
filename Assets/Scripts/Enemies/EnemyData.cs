using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Enemies
{
    /// <summary>
    /// ScriptableObject defining enemy properties
    /// Create via: Assets > Create > Vampire Survivor > Enemy Data
    /// </summary>
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Vampire Survivor/Enemy Data", order = 2)]
    public class EnemyData : ScriptableObject
    {
        [Header("Basic Info")]
        public string enemyName = "New Enemy";
        [TextArea(2, 4)]
        public string description = "Enemy description";
        public Sprite sprite;
        public EnemyType enemyType = EnemyType.Melee;

        [Header("Stats")]
        public float maxHealth = 50f;
        public float moveSpeed = 3f;
        public float damage = 10f;
        public float attackRange = 1.5f;
        public float attackCooldown = 1f;

        [Header("Detection")]
        public float detectionRange = 10f;
        public float loseTargetRange = 15f;

        [Header("Ranged Settings (for ranged enemies)")]
        public GameObject projectilePrefab;
        public float projectileSpeed = 8f;
        public float projectileLifetime = 3f;
        public float optimalRange = 6f; // Preferred distance from player

        [Header("Special Abilities")]
        public float specialAbilityCooldown = 5f;
        public GameObject abilityPrefab;

        [Header("Rewards")]
        public float experienceValue = 10f;
        public int soulsCurrency = 1;
        [Range(0f, 1f)]
        public float lootDropChance = 0.1f;

        [Header("Visual/Audio")]
        public RuntimeAnimatorController animatorController;
        public GameObject deathEffectPrefab;
        public AudioClip attackSound;
        public AudioClip deathSound;

        [Header("Scaling")]
        public float healthScalingPerFloor = 10f;
        public float damageScalingPerFloor = 2f;

        /// <summary>
        /// Get scaled stats for a specific floor
        /// </summary>
        public float GetHealthForFloor(int floor)
        {
            return maxHealth + (healthScalingPerFloor * (floor - 1));
        }

        public float GetDamageForFloor(int floor)
        {
            return damage + (damageScalingPerFloor * (floor - 1));
        }
    }
}
