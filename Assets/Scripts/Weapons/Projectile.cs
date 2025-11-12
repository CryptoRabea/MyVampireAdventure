using UnityEngine;
using System.Collections.Generic;
using VampireSurvivor.Core;

namespace VampireSurvivor.Weapons
{
    /// <summary>
    /// Projectile behavior - travels in a direction and damages enemies
    /// Uses object pooling for performance
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour, IPoolable
    {
        [Header("Components")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Collider2D col;
        [SerializeField] private TrailRenderer trail;

        [Header("Hit Effect")]
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private LayerMask enemyLayer;

        // Runtime data
        private float damage;
        private float speed;
        private Vector3 direction;
        private float lifetime;
        private GameObject owner;
        private int pierceCount;
        private int currentPierceCount;

        private HashSet<GameObject> hitTargets = new HashSet<GameObject>();
        private float lifetimeTimer;

        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (col == null) col = GetComponent<Collider2D>();

            rb.gravityScale = 0f;
            col.isTrigger = true;
        }

        /// <summary>
        /// Initialize projectile with parameters
        /// </summary>
        public void Initialize(float damage, float speed, Vector3 direction, float lifetime, GameObject owner, int pierceCount)
        {
            this.damage = damage;
            this.speed = speed;
            this.direction = direction.normalized;
            this.lifetime = lifetime;
            this.owner = owner;
            this.pierceCount = pierceCount;
            this.currentPierceCount = 0;

            lifetimeTimer = lifetime;
            hitTargets.Clear();

            // Set velocity
            rb.velocity = this.direction * speed;
        }

        private void Update()
        {
            lifetimeTimer -= Time.deltaTime;

            if (lifetimeTimer <= 0)
            {
                ReturnToPool();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Ignore owner and already hit targets
            if (other.gameObject == owner || hitTargets.Contains(other.gameObject))
                return;

            // Check if it's an enemy
            if (((1 << other.gameObject.layer) & enemyLayer) != 0)
            {
                IDamageable damageable = other.GetComponent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                {
                    // Deal damage
                    damageable.TakeDamage(damage, transform.position, owner);
                    hitTargets.Add(other.gameObject);
                    currentPierceCount++;

                    // Spawn hit effect
                    SpawnHitEffect(transform.position);

                    // Check if projectile should be destroyed
                    if (currentPierceCount > pierceCount)
                    {
                        ReturnToPool();
                    }
                }
            }
            // Hit wall or obstacle
            else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
            {
                SpawnHitEffect(transform.position);
                ReturnToPool();
            }
        }

        private void SpawnHitEffect(Vector3 position)
        {
            if (hitEffectPrefab != null && GameManager.Instance != null)
            {
                GameManager.Instance.PoolManager.SpawnFromPool(
                    hitEffectPrefab.name,
                    position,
                    Quaternion.identity
                );
            }
        }

        private void ReturnToPool()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PoolManager.ReturnToPool(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        #region IPoolable Implementation

        public void OnSpawnFromPool()
        {
            hitTargets.Clear();
            currentPierceCount = 0;

            if (trail != null)
            {
                trail.Clear();
            }
        }

        public void OnReturnToPool()
        {
            rb.velocity = Vector2.zero;
            hitTargets.Clear();

            if (trail != null)
            {
                trail.Clear();
            }
        }

        #endregion
    }
}
