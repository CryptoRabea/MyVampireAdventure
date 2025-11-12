using System;
using UnityEngine;

namespace VampireSurvivor.Core
{
    /// <summary>
    /// Interface for any entity that has health and can take damage
    /// </summary>
    public interface IHealth
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsAlive { get; }

        event Action<float, float> OnHealthChanged; // currentHealth, maxHealth
        event Action<float, Vector3, GameObject> OnDamaged; // damage, hitPoint, attacker
        event Action<GameObject> OnDeath; // killer

        void TakeDamage(float damage, Vector3 hitPoint, GameObject attacker);
        void Heal(float amount);
        void SetMaxHealth(float newMaxHealth, bool healToMax = false);
    }
}
