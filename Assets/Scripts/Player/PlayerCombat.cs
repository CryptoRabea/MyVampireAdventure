using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VampireSurvivor.Core;
using VampireSurvivor.Weapons;

namespace VampireSurvivor.Player
{
    /// <summary>
    /// Handles player combat - weapon management and firing
    /// </summary>
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Weapon Settings")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private int maxWeapons = 6; // Mobile-friendly limit

        [Header("Auto-Fire")]
        [SerializeField] private bool autoFire = true;
        [SerializeField] private float autoFireRate = 0.2f;

        [Header("Starting Loadout")]
        [SerializeField] private List<WeaponData> startingWeapons = new List<WeaponData>();

        private List<BaseWeapon> equippedWeapons = new List<BaseWeapon>();
        private PlayerController playerController;
        private float autoFireTimer = 0f;
        private bool manualFireInput = false;

        public List<BaseWeapon> EquippedWeapons => equippedWeapons;
        public bool HasWeaponSlot => equippedWeapons.Count < maxWeapons;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();

            if (firePoint == null)
            {
                firePoint = transform;
            }
        }

        private void Start()
        {
            // Equip starting weapons
            foreach (WeaponData weaponData in startingWeapons)
            {
                if (weaponData != null)
                {
                    EquipWeapon(weaponData);
                }
            }
        }

        private void Update()
        {
            UpdateWeaponCooldowns();

            if (autoFire)
            {
                HandleAutoFire();
            }
            else if (manualFireInput)
            {
                FireAllWeapons();
            }
        }

        /// <summary>
        /// Update all weapon cooldowns
        /// </summary>
        private void UpdateWeaponCooldowns()
        {
            foreach (BaseWeapon weapon in equippedWeapons)
            {
                weapon.UpdateCooldown(Time.deltaTime);
            }
        }

        /// <summary>
        /// Handle automatic firing
        /// </summary>
        private void HandleAutoFire()
        {
            autoFireTimer -= Time.deltaTime;

            if (autoFireTimer <= 0)
            {
                FireAllWeapons();
                autoFireTimer = autoFireRate;
            }
        }

        /// <summary>
        /// Fire all equipped weapons that are ready
        /// </summary>
        private void FireAllWeapons()
        {
            if (playerController == null) return;

            Vector2 aimDirection = playerController.AimDirection;

            foreach (BaseWeapon weapon in equippedWeapons)
            {
                if (weapon.CanFire)
                {
                    weapon.Fire(firePoint.position, aimDirection, gameObject);
                }
            }
        }

        /// <summary>
        /// Equip a new weapon
        /// </summary>
        public bool EquipWeapon(WeaponData weaponData)
        {
            if (weaponData == null)
            {
                Debug.LogWarning("[PlayerCombat] Cannot equip null weapon");
                return false;
            }

            if (equippedWeapons.Count >= maxWeapons)
            {
                Debug.LogWarning($"[PlayerCombat] Max weapons reached ({maxWeapons})");
                return false;
            }

            // Create weapon instance
            BaseWeapon weapon = weaponData.CreateWeaponInstance();
            if (weapon == null)
            {
                Debug.LogError($"[PlayerCombat] Failed to create weapon instance for {weaponData.weaponName}");
                return false;
            }

            equippedWeapons.Add(weapon);
            GameEvents.WeaponEquipped(weaponData.weaponName);

            Debug.Log($"[PlayerCombat] Equipped weapon: {weaponData.weaponName}");
            return true;
        }

        /// <summary>
        /// Unequip a weapon by index
        /// </summary>
        public bool UnequipWeapon(int index)
        {
            if (index < 0 || index >= equippedWeapons.Count) return false;

            BaseWeapon weapon = equippedWeapons[index];
            equippedWeapons.RemoveAt(index);

            Debug.Log($"[PlayerCombat] Unequipped weapon: {weapon.WeaponName}");
            return true;
        }

        /// <summary>
        /// Upgrade a weapon
        /// </summary>
        public bool UpgradeWeapon(int index)
        {
            if (index < 0 || index >= equippedWeapons.Count) return false;

            BaseWeapon weapon = equippedWeapons[index];
            // Weapon upgrade logic would go here
            // For now, just a placeholder

            Debug.Log($"[PlayerCombat] Upgraded weapon: {weapon.WeaponName}");
            return true;
        }

        /// <summary>
        /// Get weapon by name
        /// </summary>
        public BaseWeapon GetWeapon(string weaponName)
        {
            return equippedWeapons.Find(w => w.WeaponName == weaponName);
        }

        /// <summary>
        /// Check if player has a specific weapon
        /// </summary>
        public bool HasWeapon(string weaponName)
        {
            return GetWeapon(weaponName) != null;
        }

        #region Input System Callbacks

        /// <summary>
        /// Called by Input System for fire input (manual mode)
        /// </summary>
        public void OnFire(InputValue value)
        {
            manualFireInput = value.isPressed;
        }

        #endregion
    }
}
