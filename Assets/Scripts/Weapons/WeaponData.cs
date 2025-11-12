using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Weapons
{
    /// <summary>
    /// ScriptableObject defining weapon properties
    /// Create via: Assets > Create > Vampire Survivor > Weapon Data
    /// </summary>
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Vampire Survivor/Weapon Data", order = 1)]
    public class WeaponData : ScriptableObject
    {
        [Header("Basic Info")]
        public string weaponName = "New Weapon";
        [TextArea(2, 4)]
        public string description = "Weapon description";
        public Sprite icon;
        public WeaponType weaponType = WeaponType.Projectile;

        [Header("Stats")]
        public float damage = 10f;
        public float cooldown = 0.5f;
        public float projectileSpeed = 10f;
        public float range = 20f;
        public int pierceCount = 0; // Number of enemies a projectile can pierce
        public int projectileCount = 1; // Number of projectiles per shot

        [Header("AOE Settings (for AOE weapons)")]
        public float aoeRadius = 3f;
        public float aoeDuration = 2f;

        [Header("Projectile Settings")]
        public GameObject projectilePrefab;
        public float projectileLifetime = 5f;

        [Header("Visual/Audio")]
        public GameObject muzzleFlashPrefab;
        public AudioClip fireSound;
        public float fireVolume = 1f;

        [Header("Upgrade Path")]
        public int maxUpgradeLevel = 5;
        public float damagePerLevel = 5f;
        public float cooldownReductionPerLevel = 0.05f;

        /// <summary>
        /// Create a weapon instance from this data
        /// </summary>
        public BaseWeapon CreateWeaponInstance()
        {
            BaseWeapon weapon = null;

            switch (weaponType)
            {
                case WeaponType.Projectile:
                    weapon = new ProjectileWeapon();
                    break;
                case WeaponType.AOE:
                    weapon = new AOEWeapon();
                    break;
                case WeaponType.Beam:
                    weapon = new BeamWeapon();
                    break;
                // Add more weapon types here
                default:
                    Debug.LogError($"[WeaponData] Weapon type {weaponType} not implemented");
                    break;
            }

            if (weapon != null)
            {
                weapon.Initialize(this);
            }

            return weapon;
        }

        /// <summary>
        /// Get stat value for a specific upgrade level
        /// </summary>
        public float GetDamageAtLevel(int level)
        {
            return damage + (damagePerLevel * (level - 1));
        }

        public float GetCooldownAtLevel(int level)
        {
            return Mathf.Max(0.1f, cooldown - (cooldownReductionPerLevel * (level - 1)));
        }
    }
}
