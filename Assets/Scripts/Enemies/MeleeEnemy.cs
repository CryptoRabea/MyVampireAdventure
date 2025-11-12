using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Enemies
{
    /// <summary>
    /// Melee enemy - charges at player and attacks in close range
    /// </summary>
    public class MeleeEnemy : BaseEnemy
    {
        [Header("Melee Settings")]
        [SerializeField] private float chargeSpeedMultiplier = 1.5f;
        [SerializeField] private float chargeDuration = 0.5f;

        private bool isCharging = false;
        private float chargeTimer = 0f;

        protected override void Update()
        {
            base.Update();

            if (isCharging)
            {
                chargeTimer -= Time.deltaTime;
                if (chargeTimer <= 0)
                {
                    isCharging = false;
                }
            }
        }

        protected override void MoveTowardsTarget()
        {
            if (target == null) return;

            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            // Charge when getting close
            if (distanceToTarget <= data.attackRange * 2f && !isCharging && attackCooldownTimer <= 0)
            {
                StartCharge();
            }

            Vector2 direction = (target.position - transform.position).normalized;
            float speed = isCharging ? data.moveSpeed * chargeSpeedMultiplier : data.moveSpeed;
            rb.velocity = direction * speed;

            // Flip sprite
            if (spriteRenderer != null && direction.x != 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }

        protected override void PerformAttack()
        {
            if (target == null) return;

            // Deal damage to player
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                float scaledDamage = data.GetDamageForFloor(currentFloor);
                damageable.TakeDamage(scaledDamage, target.position, gameObject);

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

        private void StartCharge()
        {
            isCharging = true;
            chargeTimer = chargeDuration;

            if (animator != null)
            {
                animator.SetTrigger("Charge");
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Deal damage on collision with player during charge
            if (isCharging && collision.gameObject.CompareTag("Player"))
            {
                IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                {
                    float scaledDamage = data.GetDamageForFloor(currentFloor) * 0.5f; // Reduced charge damage
                    damageable.TakeDamage(scaledDamage, collision.contacts[0].point, gameObject);
                }

                isCharging = false;
            }
        }
    }
}
