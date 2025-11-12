using UnityEngine;
using VampireSurvivor.Core;
using VampireSurvivor.Weapons;

namespace VampireSurvivor.Enemies
{
    /// <summary>
    /// Ranged enemy - maintains distance and shoots projectiles at player
    /// </summary>
    public class RangedEnemy : BaseEnemy
    {
        [Header("Ranged Settings")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private float minDistance = 4f; // Minimum distance to maintain from player

        protected override void Awake()
        {
            base.Awake();

            if (firePoint == null)
            {
                firePoint = transform;
            }
        }

        protected override void StateChase()
        {
            if (target == null)
            {
                ChangeState(AIState.Idle);
                return;
            }

            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            // Out of range - move closer
            if (distanceToTarget > data.optimalRange)
            {
                MoveTowardsTarget();
            }
            // Too close - back away
            else if (distanceToTarget < minDistance)
            {
                MoveAwayFromTarget();
            }
            // In optimal range - attack
            else if (distanceToTarget <= data.attackRange)
            {
                ChangeState(AIState.Attack);
            }
            // At optimal range - stay still
            else
            {
                rb.velocity = Vector2.zero;
            }
        }

        protected override void MoveTowardsTarget()
        {
            if (target == null) return;

            Vector2 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * data.moveSpeed;

            // Flip sprite
            if (spriteRenderer != null && direction.x != 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }

        private void MoveAwayFromTarget()
        {
            if (target == null) return;

            Vector2 direction = (transform.position - target.position).normalized;
            rb.velocity = direction * data.moveSpeed;

            // Flip sprite
            if (spriteRenderer != null && direction.x != 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }

        protected override void PerformAttack()
        {
            if (target == null || data.projectilePrefab == null) return;

            // Stop movement while attacking
            rb.velocity = Vector2.zero;

            // Calculate direction to target
            Vector2 direction = (target.position - firePoint.position).normalized;

            // Spawn projectile
            if (GameManager.Instance != null)
            {
                GameObject projectileObj = GameManager.Instance.PoolManager.SpawnFromPool(
                    data.projectilePrefab.name,
                    firePoint.position,
                    Quaternion.LookRotation(Vector3.forward, direction)
                );

                if (projectileObj != null)
                {
                    Projectile projectile = projectileObj.GetComponent<Projectile>();
                    if (projectile != null)
                    {
                        float scaledDamage = data.GetDamageForFloor(currentFloor);
                        projectile.Initialize(
                            scaledDamage,
                            data.projectileSpeed,
                            direction,
                            data.projectileLifetime,
                            gameObject,
                            0 // No pierce for enemy projectiles
                        );
                    }
                }
            }

            // Play attack sound
            if (data.attackSound != null)
            {
                AudioSource.PlayClipAtPoint(data.attackSound, transform.position);
            }

            // Play attack animation
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }
    }
}
