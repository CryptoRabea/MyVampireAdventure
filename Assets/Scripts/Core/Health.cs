using System;
using UnityEngine;

namespace VampireSurvivor.Core
{
    /// <summary>
    /// Health component implementing IHealth and IDamageable
    /// Can be attached to any GameObject that needs health management
    /// </summary>
    public class Health : MonoBehaviour, IHealth, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
        [SerializeField] private bool invulnerable = false;
        [SerializeField] private bool destroyOnDeath = true;

        [Header("Visual Feedback")]
        [SerializeField] private GameObject damageVFXPrefab;
        [SerializeField] private GameObject deathVFXPrefab;

        // IHealth Properties
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsAlive => currentHealth > 0;

        // IHealth Events
        public event Action<float, float> OnHealthChanged;
        public event Action<float, Vector3, GameObject> OnDamaged;
        public event Action<GameObject> OnDeath;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        /// <summary>
        /// Apply damage to this entity
        /// </summary>
        public void TakeDamage(float damage, Vector3 hitPoint, GameObject attacker)
        {
            if (!IsAlive || invulnerable || damage <= 0) return;

            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);

            OnDamaged?.Invoke(damage, hitPoint, attacker);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            // Spawn damage VFX
            if (damageVFXPrefab != null && GameManager.Instance != null)
            {
                GameManager.Instance.PoolManager.SpawnFromPool(
                    damageVFXPrefab.name,
                    hitPoint,
                    Quaternion.identity
                );
            }

            if (!IsAlive)
            {
                Die(attacker);
            }
        }

        /// <summary>
        /// Heal this entity
        /// </summary>
        public void Heal(float amount)
        {
            if (!IsAlive || amount <= 0) return;

            currentHealth += amount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        /// <summary>
        /// Set new max health, optionally healing to full
        /// </summary>
        public void SetMaxHealth(float newMaxHealth, bool healToMax = false)
        {
            maxHealth = newMaxHealth;

            if (healToMax)
            {
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth = Mathf.Min(currentHealth, maxHealth);
            }

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        /// <summary>
        /// Set invulnerability state
        /// </summary>
        public void SetInvulnerable(bool state)
        {
            invulnerable = state;
        }

        /// <summary>
        /// Handle death
        /// </summary>
        private void Die(GameObject killer)
        {
            OnDeath?.Invoke(killer);

            // Spawn death VFX
            if (deathVFXPrefab != null && GameManager.Instance != null)
            {
                GameManager.Instance.PoolManager.SpawnFromPool(
                    deathVFXPrefab.name,
                    transform.position,
                    Quaternion.identity
                );
            }

            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Reset health to max (useful for object pooling)
        /// </summary>
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }
}
