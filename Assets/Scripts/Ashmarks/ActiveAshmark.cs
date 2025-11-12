using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Ashmarks
{
    /// <summary>
    /// Active Ashmark - manually activated ability with cooldown
    /// </summary>
    public class ActiveAshmark : BaseAshmark
    {
        public override void Activate(GameObject owner)
        {
            base.Activate(owner);

            // Execute ability effect based on prefab type
            if (data.abilityPrefab != null)
            {
                ExecuteAbilityEffect(owner);
            }

            // Auto-deactivate after duration
            if (data.duration > 0)
            {
                GameManager.Instance.StartCoroutine(DeactivateAfterDuration(owner, data.duration));
            }
            else
            {
                Deactivate(owner);
            }
        }

        private void ExecuteAbilityEffect(GameObject owner)
        {
            // Spawn ability effect/projectile
            if (GameManager.Instance != null)
            {
                Vector3 spawnPosition = owner.transform.position;

                // For projectile abilities, spawn in forward direction
                PlayerController controller = owner.GetComponent<PlayerController>();
                Vector3 direction = controller != null ? controller.AimDirection : Vector2.right;

                GameObject abilityObj = GameManager.Instance.PoolManager.SpawnFromPool(
                    data.abilityPrefab.name,
                    spawnPosition,
                    Quaternion.LookRotation(Vector3.forward, direction)
                );

                // Configure ability based on type
                ConfigureAbilityObject(abilityObj, owner, direction);
            }
        }

        private void ConfigureAbilityObject(GameObject abilityObj, GameObject owner, Vector3 direction)
        {
            if (abilityObj == null) return;

            // Check if it's a projectile
            Weapons.Projectile projectile = abilityObj.GetComponent<Weapons.Projectile>();
            if (projectile != null)
            {
                projectile.Initialize(
                    data.abilityDamage,
                    15f, // Speed
                    direction,
                    5f, // Lifetime
                    owner,
                    999 // High pierce count for abilities
                );
            }
            // Check if it's an AOE zone
            else if (data.abilityRadius > 0)
            {
                // Create AOE damage zone
                Weapons.AOEDamageZone zone = abilityObj.AddComponent<Weapons.AOEDamageZone>();
                zone.Initialize(
                    data.abilityDamage,
                    data.abilityRadius,
                    data.duration,
                    owner,
                    true // Continuous damage
                );
            }
        }

        private System.Collections.IEnumerator DeactivateAfterDuration(GameObject owner, float duration)
        {
            yield return new WaitForSeconds(duration);
            Deactivate(owner);
        }
    }
}
