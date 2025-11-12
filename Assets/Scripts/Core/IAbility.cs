using System;
using UnityEngine;

namespace VampireSurvivor.Core
{
    /// <summary>
    /// Interface for abilities (Ashmarks)
    /// </summary>
    public interface IAbility
    {
        string AbilityName { get; }
        bool CanActivate { get; }
        float CurrentCooldown { get; }
        bool IsPassive { get; }

        event Action OnAbilityActivated;
        event Action<float> OnCooldownChanged;

        void Activate(GameObject owner);
        void Deactivate(GameObject owner);
        void UpdateCooldown(float deltaTime);
    }
}
