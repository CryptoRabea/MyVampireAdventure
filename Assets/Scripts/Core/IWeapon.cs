using System;
using UnityEngine;

namespace VampireSurvivor.Core
{
    /// <summary>
    /// Interface for weapon systems
    /// </summary>
    public interface IWeapon
    {
        string WeaponName { get; }
        bool CanFire { get; }
        float CurrentCooldown { get; }
        int CurrentAmmo { get; }
        int MaxAmmo { get; }

        event Action OnWeaponFired;
        event Action OnWeaponReloaded;
        event Action<float> OnCooldownChanged; // cooldown remaining

        void Fire(Vector3 origin, Vector3 direction, GameObject owner);
        void Reload();
        void UpdateCooldown(float deltaTime);
    }
}
