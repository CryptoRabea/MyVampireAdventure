using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Weapons
{
    /// <summary>
    /// AOE (Area of Effect) weapon implementation
    /// Creates damage zones at target location or around player
    /// </summary>
    public class AOEWeapon : BaseWeapon
    {
        public override void Fire(Vector3 origin, Vector3 direction, GameObject owner)
        {
            base.Fire(origin, direction, owner);

            // Spawn AOE effect at range distance in aim direction
            Vector3 spawnPosition = origin + direction * data.range;

            // Create temporary AOE damage zone
            GameObject aoeZone = new GameObject($"{WeaponName}_AOE");
            aoeZone.transform.position = spawnPosition;

            AOEDamageZone damageZone = aoeZone.AddComponent<AOEDamageZone>();
            damageZone.Initialize(
                GetCurrentDamage(),
                data.aoeRadius,
                data.aoeDuration,
                owner,
                data.weaponType == WeaponType.AOE
            );

            // Spawn visual effect if available
            if (data.projectilePrefab != null && GameManager.Instance != null)
            {
                GameManager.Instance.PoolManager.SpawnFromPool(
                    data.projectilePrefab.name,
                    spawnPosition,
                    Quaternion.identity
                );
            }
        }
    }

    /// <summary>
    /// AOE damage zone component
    /// Deals damage to enemies within radius over time
    /// </summary>
    public class AOEDamageZone : MonoBehaviour
    {
        private float damage;
        private float radius;
        private float duration;
        private GameObject owner;
        private bool continuousDamage;
        private float damageTickRate = 0.2f;
        private float damageTickTimer = 0f;

        private HashSet<GameObject> enemiesInZone = new HashSet<GameObject>();
        private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();

        public void Initialize(float damage, float radius, float duration, GameObject owner, bool continuousDamage)
        {
            this.damage = damage;
            this.radius = radius;
            this.duration = duration;
            this.owner = owner;
            this.continuousDamage = continuousDamage;

            // Setup collider
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = radius;
            collider.isTrigger = true;

            Destroy(gameObject, duration);
        }

        private void Update()
        {
            if (continuousDamage)
            {
                damageTickTimer -= Time.deltaTime;
                if (damageTickTimer <= 0)
                {
                    DamageEnemiesInZone();
                    damageTickTimer = damageTickRate;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive && other.gameObject != owner)
            {
                enemiesInZone.Add(other.gameObject);

                if (!continuousDamage && !damagedEnemies.Contains(other.gameObject))
                {
                    damageable.TakeDamage(damage, transform.position, owner);
                    damagedEnemies.Add(other.gameObject);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            enemiesInZone.Remove(other.gameObject);
        }

        private void DamageEnemiesInZone()
        {
            foreach (GameObject enemy in enemiesInZone)
            {
                if (enemy == null) continue;

                IDamageable damageable = enemy.GetComponent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                {
                    damageable.TakeDamage(damage * damageTickRate, transform.position, owner);
                }
            }
        }
    }
}
