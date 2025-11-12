using UnityEngine;
using VampireSurvivor.Core;
using VampireSurvivor.Player;

namespace VampireSurvivor.Ashmarks
{
    /// <summary>
    /// Passive Ashmark - provides permanent stat bonuses
    /// </summary>
    public class PassiveAshmark : BaseAshmark
    {
        public override void Activate(GameObject owner)
        {
            base.Activate(owner);
            ApplyStatModifiers(owner);
        }

        public override void ApplyStatModifiers(GameObject owner)
        {
            // Apply health modifier
            if (data.healthModifier != 0)
            {
                Health health = owner.GetComponent<Health>();
                if (health != null)
                {
                    health.SetMaxHealth(health.MaxHealth + data.healthModifier, healToMax: false);
                }
            }

            // Apply speed modifier
            if (data.speedModifier != 0)
            {
                PlayerController controller = owner.GetComponent<PlayerController>();
                if (controller != null)
                {
                    // Speed modification would be handled via a stat system
                    // For now, this is a placeholder
                }
            }

            // Damage and other modifiers would be applied through a central stats system
            // This is a simplified implementation
        }

        public override void RemoveStatModifiers(GameObject owner)
        {
            // Remove health modifier
            if (data.healthModifier != 0)
            {
                Health health = owner.GetComponent<Health>();
                if (health != null)
                {
                    health.SetMaxHealth(health.MaxHealth - data.healthModifier, healToMax: false);
                }
            }

            // Remove other modifiers
        }
    }
}
