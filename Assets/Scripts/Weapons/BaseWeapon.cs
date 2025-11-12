using System;
using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Weapons
{
    /// <summary>
    /// Base class for all weapon types
    /// Implements IWeapon interface
    /// </summary>
    public abstract class BaseWeapon : IWeapon
    {
        protected WeaponData data;
        protected int currentLevel = 1;
        protected float currentCooldown = 0f;

        // IWeapon Properties
        public string WeaponName => data != null ? data.weaponName : "Unknown";
        public bool CanFire => currentCooldown <= 0;
        public float CurrentCooldown => currentCooldown;
        public int CurrentAmmo => -1; // -1 means infinite
        public int MaxAmmo => -1;

        // IWeapon Events
        public event Action OnWeaponFired;
        public event Action OnWeaponReloaded;
        public event Action<float> OnCooldownChanged;

        // Properties
        public WeaponData Data => data;
        public int CurrentLevel => currentLevel;

        /// <summary>
        /// Initialize weapon with data
        /// </summary>
        public virtual void Initialize(WeaponData weaponData)
        {
            data = weaponData;
            currentLevel = 1;
            currentCooldown = 0f;
        }

        /// <summary>
        /// Fire the weapon
        /// </summary>
        public virtual void Fire(Vector3 origin, Vector3 direction, GameObject owner)
        {
            if (!CanFire || data == null) return;

            // Reset cooldown
            currentCooldown = GetCurrentCooldown();

            // Play fire sound
            if (data.fireSound != null)
            {
                AudioSource.PlayClipAtPoint(data.fireSound, origin, data.fireVolume);
            }

            // Spawn muzzle flash
            if (data.muzzleFlashPrefab != null && GameManager.Instance != null)
            {
                GameManager.Instance.PoolManager.SpawnFromPool(
                    data.muzzleFlashPrefab.name,
                    origin,
                    Quaternion.LookRotation(Vector3.forward, direction)
                );
            }

            OnWeaponFired?.Invoke();
            GameEvents.WeaponFired(WeaponName);
        }

        /// <summary>
        /// Update weapon cooldown
        /// </summary>
        public virtual void UpdateCooldown(float deltaTime)
        {
            if (currentCooldown > 0)
            {
                currentCooldown -= deltaTime;
                OnCooldownChanged?.Invoke(currentCooldown);
            }
        }

        /// <summary>
        /// Reload weapon (not used by most weapons in this game)
        /// </summary>
        public virtual void Reload()
        {
            OnWeaponReloaded?.Invoke();
        }

        /// <summary>
        /// Upgrade weapon to next level
        /// </summary>
        public virtual bool Upgrade()
        {
            if (currentLevel >= data.maxUpgradeLevel)
            {
                return false;
            }

            currentLevel++;
            Debug.Log($"[BaseWeapon] {WeaponName} upgraded to level {currentLevel}");
            return true;
        }

        /// <summary>
        /// Get current damage value
        /// </summary>
        public virtual float GetCurrentDamage()
        {
            return data.GetDamageAtLevel(currentLevel);
        }

        /// <summary>
        /// Get current cooldown value
        /// </summary>
        public virtual float GetCurrentCooldown()
        {
            return data.GetCooldownAtLevel(currentLevel);
        }
    }
}
