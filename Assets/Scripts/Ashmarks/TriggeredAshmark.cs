using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Ashmarks
{
    /// <summary>
    /// Triggered Ashmark - automatically activates on specific events
    /// Examples: on kill, on hit, on damaged, etc.
    /// </summary>
    public class TriggeredAshmark : BaseAshmark
    {
        public override void Activate(GameObject owner)
        {
            base.Activate(owner);

            // Execute triggered effect
            ExecuteTriggeredEffect(owner);

            // Triggered abilities have instant effect
            Deactivate(owner);
        }

        private void ExecuteTriggeredEffect(GameObject owner)
        {
            switch (data.triggerType)
            {
                case AshmarkData.TriggerType.OnKill:
                    OnKillEffect(owner);
                    break;
                case AshmarkData.TriggerType.OnHit:
                    OnHitEffect(owner);
                    break;
                case AshmarkData.TriggerType.OnDamaged:
                    OnDamagedEffect(owner);
                    break;
                case AshmarkData.TriggerType.OnDodge:
                    OnDodgeEffect(owner);
                    break;
                case AshmarkData.TriggerType.OnLowHealth:
                    OnLowHealthEffect(owner);
                    break;
            }

            // Spawn ability VFX
            if (data.abilityPrefab != null && GameManager.Instance != null)
            {
                GameManager.Instance.PoolManager.SpawnFromPool(
                    data.abilityPrefab.name,
                    owner.transform.position,
                    Quaternion.identity
                );
            }
        }

        private void OnKillEffect(GameObject owner)
        {
            // Example: Heal on kill
            Health health = owner.GetComponent<Health>();
            if (health != null)
            {
                health.Heal(data.abilityDamage); // Using abilityDamage as heal amount
            }

            Debug.Log($"[TriggeredAshmark] {data.ashmarkName} triggered on kill");
        }

        private void OnHitEffect(GameObject owner)
        {
            // Example: Spawn explosion on hit
            if (data.abilityRadius > 0)
            {
                DamageNearbyEnemies(owner, data.abilityRadius, data.abilityDamage);
            }

            Debug.Log($"[TriggeredAshmark] {data.ashmarkName} triggered on hit");
        }

        private void OnDamagedEffect(GameObject owner)
        {
            // Example: Create shield or knockback enemies
            DamageNearbyEnemies(owner, data.abilityRadius, data.abilityDamage);

            Debug.Log($"[TriggeredAshmark] {data.ashmarkName} triggered on damaged");
        }

        private void OnDodgeEffect(GameObject owner)
        {
            // Example: Leave fire trail or become invisible
            if (data.abilityPrefab != null && GameManager.Instance != null)
            {
                GameManager.Instance.PoolManager.SpawnFromPool(
                    data.abilityPrefab.name,
                    owner.transform.position,
                    Quaternion.identity
                );
            }

            Debug.Log($"[TriggeredAshmark] {data.ashmarkName} triggered on dodge");
        }

        private void OnLowHealthEffect(GameObject owner)
        {
            // Example: Temporary invulnerability or damage boost
            Health health = owner.GetComponent<Health>();
            if (health != null && health.CurrentHealth / health.MaxHealth <= 0.3f)
            {
                health.SetInvulnerable(true);
                GameManager.Instance.StartCoroutine(RemoveInvulnerabilityAfterDelay(health, 2f));
            }

            Debug.Log($"[TriggeredAshmark] {data.ashmarkName} triggered on low health");
        }

        private void DamageNearbyEnemies(GameObject owner, float radius, float damage)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(owner.transform.position, radius);

            foreach (Collider2D enemy in enemies)
            {
                if (enemy.gameObject != owner)
                {
                    IDamageable damageable = enemy.GetComponent<IDamageable>();
                    if (damageable != null && damageable.IsAlive)
                    {
                        damageable.TakeDamage(damage, enemy.transform.position, owner);
                    }
                }
            }
        }

        private System.Collections.IEnumerator RemoveInvulnerabilityAfterDelay(Health health, float delay)
        {
            yield return new WaitForSeconds(delay);
            health.SetInvulnerable(false);
        }
    }
}
