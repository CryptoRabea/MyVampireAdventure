using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Weapons
{
    /// <summary>
    /// Projectile-based weapon implementation
    /// Fires projectiles that travel in a direction
    /// </summary>
    public class ProjectileWeapon : BaseWeapon
    {
        public override void Fire(Vector3 origin, Vector3 direction, GameObject owner)
        {
            base.Fire(origin, direction, owner);

            if (data.projectilePrefab == null || GameManager.Instance == null)
            {
                Debug.LogWarning($"[ProjectileWeapon] {WeaponName} has no projectile prefab");
                return;
            }

            // Fire multiple projectiles if configured
            int projectileCount = data.projectileCount;
            float spreadAngle = projectileCount > 1 ? 15f : 0f; // 15 degree spread for multi-projectile

            for (int i = 0; i < projectileCount; i++)
            {
                // Calculate spread
                float angle = 0f;
                if (projectileCount > 1)
                {
                    float step = spreadAngle / (projectileCount - 1);
                    angle = -spreadAngle / 2f + (step * i);
                }

                Vector3 spreadDirection = Quaternion.Euler(0, 0, angle) * direction;

                // Spawn projectile from pool
                GameObject projectileObj = GameManager.Instance.PoolManager.SpawnFromPool(
                    data.projectilePrefab.name,
                    origin,
                    Quaternion.LookRotation(Vector3.forward, spreadDirection)
                );

                if (projectileObj != null)
                {
                    Projectile projectile = projectileObj.GetComponent<Projectile>();
                    if (projectile != null)
                    {
                        projectile.Initialize(
                            GetCurrentDamage(),
                            data.projectileSpeed,
                            spreadDirection,
                            data.projectileLifetime,
                            owner,
                            data.pierceCount
                        );
                    }
                }
            }
        }
    }
}
