using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Weapons
{
    /// <summary>
    /// Beam weapon implementation
    /// Fires a continuous or instant beam that damages all enemies in line
    /// </summary>
    public class BeamWeapon : BaseWeapon
    {
        public override void Fire(Vector3 origin, Vector3 direction, GameObject owner)
        {
            base.Fire(origin, direction, owner);

            // Perform raycast to hit all enemies in line
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, data.range);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject == owner) continue;

                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                {
                    damageable.TakeDamage(GetCurrentDamage(), hit.point, owner);

                    // Spawn hit effect
                    if (GameManager.Instance != null && data.muzzleFlashPrefab != null)
                    {
                        GameManager.Instance.PoolManager.SpawnFromPool(
                            data.muzzleFlashPrefab.name,
                            hit.point,
                            Quaternion.identity
                        );
                    }
                }
            }

            // Spawn beam visual effect
            if (data.projectilePrefab != null && GameManager.Instance != null)
            {
                GameObject beamEffect = GameManager.Instance.PoolManager.SpawnFromPool(
                    data.projectilePrefab.name,
                    origin,
                    Quaternion.LookRotation(Vector3.forward, direction)
                );

                // Scale beam to range
                if (beamEffect != null)
                {
                    LineRenderer lineRenderer = beamEffect.GetComponent<LineRenderer>();
                    if (lineRenderer != null)
                    {
                        lineRenderer.SetPosition(0, origin);
                        lineRenderer.SetPosition(1, origin + direction * data.range);
                    }

                    // Return to pool after short duration
                    GameManager.Instance.PoolManager.ReturnToPoolDelayed(beamEffect, 0.1f);
                }
            }
        }
    }
}
