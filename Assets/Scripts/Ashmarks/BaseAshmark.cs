using System;
using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Ashmarks
{
    /// <summary>
    /// Base class for all Ashmark abilities
    /// Implements IAbility interface
    /// </summary>
    public abstract class BaseAshmark : IAbility
    {
        protected AshmarkData data;
        protected GameObject owner;
        protected float currentCooldown = 0f;
        protected bool isActive = false;

        // IAbility Properties
        public string AbilityName => data != null ? data.ashmarkName : "Unknown";
        public bool CanActivate => currentCooldown <= 0 && !isActive;
        public float CurrentCooldown => currentCooldown;
        public bool IsPassive => data != null && data.type == AshmarkType.Passive;

        // IAbility Events
        public event Action OnAbilityActivated;
        public event Action<float> OnCooldownChanged;

        // Properties
        public AshmarkData Data => data;
        public bool IsActive => isActive;

        /// <summary>
        /// Initialize Ashmark with data and owner
        /// </summary>
        public virtual void Initialize(AshmarkData ashmarkData, GameObject owner)
        {
            data = ashmarkData;
            this.owner = owner;
            currentCooldown = 0f;

            // Apply passive modifiers immediately
            if (IsPassive)
            {
                Activate(owner);
            }
        }

        /// <summary>
        /// Activate the ability
        /// </summary>
        public virtual void Activate(GameObject owner)
        {
            if (!CanActivate && !IsPassive) return;

            this.owner = owner;
            isActive = true;

            // Reset cooldown
            if (!IsPassive)
            {
                currentCooldown = data.cooldown;
            }

            // Play activation sound
            if (data.activationSound != null)
            {
                AudioSource.PlayClipAtPoint(data.activationSound, owner.transform.position);
            }

            // Spawn activation VFX
            if (data.activationVFXPrefab != null && GameManager.Instance != null)
            {
                GameManager.Instance.PoolManager.SpawnFromPool(
                    data.activationVFXPrefab.name,
                    owner.transform.position,
                    Quaternion.identity
                );
            }

            OnAbilityActivated?.Invoke();
            GameEvents.AshmarkActivated(AbilityName);
        }

        /// <summary>
        /// Deactivate the ability
        /// </summary>
        public virtual void Deactivate(GameObject owner)
        {
            isActive = false;
        }

        /// <summary>
        /// Update ability cooldown
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
        /// Apply stat modifiers to owner
        /// </summary>
        public virtual void ApplyStatModifiers(GameObject owner)
        {
            // Override in derived classes to apply specific stat modifications
        }

        /// <summary>
        /// Remove stat modifiers from owner
        /// </summary>
        public virtual void RemoveStatModifiers(GameObject owner)
        {
            // Override in derived classes to remove specific stat modifications
        }
    }
}
