using UnityEngine;

namespace VampireSurvivor.Core
{
    /// <summary>
    /// Interface for entities that can receive damage (simpler than IHealth)
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float damage, Vector3 hitPoint, GameObject attacker);
        bool IsAlive { get; }
    }
}
