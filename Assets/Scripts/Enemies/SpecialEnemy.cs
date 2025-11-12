using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Enemies
{
    /// <summary>
    /// Special enemy with unique abilities (teleport, summon, AOE, etc.)
    /// Highly customizable through EnemyData
    /// </summary>
    public class SpecialEnemy : BaseEnemy
    {
        [Header("Special Ability Settings")]
        [SerializeField] private SpecialAbilityType abilityType = SpecialAbilityType.Teleport;
        [SerializeField] private float teleportDistance = 5f;
        [SerializeField] private int summonCount = 3;
        [SerializeField] private GameObject summonPrefab;

        public enum SpecialAbilityType
        {
            Teleport,
            SummonMinions,
            AOEAttack,
            Shield,
            Heal
        }

        protected override void Update()
        {
            base.Update();

            // Use special ability when available
            if (specialAbilityCooldownTimer <= 0 && currentState == AIState.Attack)
            {
                UseSpecialAbility();
                specialAbilityCooldownTimer = data.specialAbilityCooldown;
            }
        }

        protected override void PerformAttack()
        {
            if (target == null) return;

            // Basic melee attack
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                float scaledDamage = data.GetDamageForFloor(currentFloor);
                damageable.TakeDamage(scaledDamage, target.position, gameObject);

                if (data.attackSound != null)
                {
                    AudioSource.PlayClipAtPoint(data.attackSound, transform.position);
                }

                if (animator != null)
                {
                    animator.SetTrigger("Attack");
                }
            }
        }

        private void UseSpecialAbility()
        {
            switch (abilityType)
            {
                case SpecialAbilityType.Teleport:
                    TeleportAbility();
                    break;
                case SpecialAbilityType.SummonMinions:
                    SummonMinionsAbility();
                    break;
                case SpecialAbilityType.AOEAttack:
                    AOEAttackAbility();
                    break;
                case SpecialAbilityType.Shield:
                    ShieldAbility();
                    break;
                case SpecialAbilityType.Heal:
                    HealAbility();
                    break;
            }

            // Spawn ability VFX
            if (data.abilityPrefab != null && GameManager.Instance != null)
            {
                GameManager.Instance.PoolManager.SpawnFromPool(
                    data.abilityPrefab.name,
                    transform.position,
                    Quaternion.identity
                );
            }

            if (animator != null)
            {
                animator.SetTrigger("SpecialAbility");
            }
        }

        private void TeleportAbility()
        {
            if (target == null) return;

            // Teleport near the player
            Vector2 directionToPlayer = (target.position - transform.position).normalized;
            Vector2 teleportPosition = (Vector2)target.position - directionToPlayer * teleportDistance;

            transform.position = teleportPosition;
            Debug.Log($"[SpecialEnemy] {data.enemyName} teleported");
        }

        private void SummonMinionsAbility()
        {
            if (summonPrefab == null) return;

            // Summon minions around the special enemy
            for (int i = 0; i < summonCount; i++)
            {
                float angle = (360f / summonCount) * i;
                Vector2 offset = new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                ) * 2f;

                Vector3 spawnPosition = transform.position + (Vector3)offset;
                Instantiate(summonPrefab, spawnPosition, Quaternion.identity);
            }

            Debug.Log($"[SpecialEnemy] {data.enemyName} summoned {summonCount} minions");
        }

        private void AOEAttackAbility()
        {
            if (target == null) return;

            // Deal AOE damage around the enemy
            float aoeRadius = data.attackRange * 2f;
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aoeRadius);

            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    IDamageable damageable = hit.GetComponent<IDamageable>();
                    if (damageable != null && damageable.IsAlive)
                    {
                        float scaledDamage = data.GetDamageForFloor(currentFloor) * 1.5f;
                        damageable.TakeDamage(scaledDamage, hit.transform.position, gameObject);
                    }
                }
            }

            Debug.Log($"[SpecialEnemy] {data.enemyName} used AOE attack");
        }

        private void ShieldAbility()
        {
            // Grant temporary invulnerability
            StartCoroutine(ShieldCoroutine());
            Debug.Log($"[SpecialEnemy] {data.enemyName} activated shield");
        }

        private System.Collections.IEnumerator ShieldCoroutine()
        {
            health.SetInvulnerable(true);
            yield return new WaitForSeconds(3f);
            health.SetInvulnerable(false);
        }

        private void HealAbility()
        {
            // Heal self
            float healAmount = data.maxHealth * 0.2f; // Heal 20% of max health
            health.Heal(healAmount);
            Debug.Log($"[SpecialEnemy] {data.enemyName} healed for {healAmount}");
        }
    }
}
