using UnityEngine;
using VampireSurvivor.Core;
using System.Collections;

namespace VampireSurvivor.Enemies
{
    /// <summary>
    /// Base enemy class with behavior tree-like AI
    /// All enemy types inherit from this
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Health))]
    public abstract class BaseEnemy : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] protected EnemyData data;
        [SerializeField] protected int currentFloor = 1;

        [Header("Components")]
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected Animator animator;

        protected Rigidbody2D rb;
        protected Health health;

        // AI State
        protected AIState currentState = AIState.Idle;
        protected Transform target;
        protected float attackCooldownTimer = 0f;
        protected float specialAbilityCooldownTimer = 0f;

        // Properties
        public EnemyData Data => data;
        public AIState CurrentState => currentState;
        public bool IsAlive => health != null && health.IsAlive;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();

            rb.gravityScale = 0f;
            rb.drag = 0f;
            rb.freezeRotation = true;

            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (animator == null)
                animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            Initialize(data, currentFloor);
        }

        protected virtual void OnEnable()
        {
            if (health != null)
            {
                health.OnDeath += HandleDeath;
            }
        }

        protected virtual void OnDisable()
        {
            if (health != null)
            {
                health.OnDeath -= HandleDeath;
            }
        }

        /// <summary>
        /// Initialize enemy with data and floor scaling
        /// </summary>
        public virtual void Initialize(EnemyData enemyData, int floor)
        {
            data = enemyData;
            currentFloor = floor;

            if (health != null)
            {
                float scaledHealth = data.GetHealthForFloor(floor);
                health.SetMaxHealth(scaledHealth, healToMax: true);
            }

            if (spriteRenderer != null && data.sprite != null)
            {
                spriteRenderer.sprite = data.sprite;
            }

            if (animator != null && data.animatorController != null)
            {
                animator.runtimeAnimatorController = data.animatorController;
            }

            FindTarget();
            ChangeState(AIState.Idle);
        }

        protected virtual void Update()
        {
            if (!IsAlive) return;

            UpdateCooldowns();
            UpdateAI();
        }

        /// <summary>
        /// Main AI update loop - behavior tree style
        /// </summary>
        protected virtual void UpdateAI()
        {
            // Find or update target
            if (target == null || !IsTargetValid())
            {
                FindTarget();
            }

            // Execute current state
            switch (currentState)
            {
                case AIState.Idle:
                    StateIdle();
                    break;
                case AIState.Chase:
                    StateChase();
                    break;
                case AIState.Attack:
                    StateAttack();
                    break;
                case AIState.Flee:
                    StateFlee();
                    break;
            }
        }

        #region AI States

        protected virtual void StateIdle()
        {
            if (target != null)
            {
                float distanceToTarget = Vector2.Distance(transform.position, target.position);
                if (distanceToTarget <= data.detectionRange)
                {
                    ChangeState(AIState.Chase);
                }
            }
        }

        protected virtual void StateChase()
        {
            if (target == null)
            {
                ChangeState(AIState.Idle);
                return;
            }

            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            // Check if target is out of range
            if (distanceToTarget > data.loseTargetRange)
            {
                ChangeState(AIState.Idle);
                return;
            }

            // Check if in attack range
            if (distanceToTarget <= data.attackRange)
            {
                ChangeState(AIState.Attack);
                return;
            }

            // Move towards target
            MoveTowardsTarget();
        }

        protected virtual void StateAttack()
        {
            if (target == null)
            {
                ChangeState(AIState.Idle);
                return;
            }

            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            // Check if target moved out of attack range
            if (distanceToTarget > data.attackRange)
            {
                ChangeState(AIState.Chase);
                return;
            }

            // Stop movement
            rb.velocity = Vector2.zero;

            // Attack if cooldown is ready
            if (attackCooldownTimer <= 0)
            {
                PerformAttack();
                attackCooldownTimer = data.attackCooldown;
            }
        }

        protected virtual void StateFlee()
        {
            // Move away from target
            if (target != null)
            {
                Vector2 directionAway = (transform.position - target.position).normalized;
                rb.velocity = directionAway * data.moveSpeed;
            }
        }

        #endregion

        #region AI Behaviors

        protected virtual void MoveTowardsTarget()
        {
            if (target == null) return;

            Vector2 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * data.moveSpeed;

            // Flip sprite based on direction
            if (spriteRenderer != null && direction.x != 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }

        protected abstract void PerformAttack();

        protected virtual void FindTarget()
        {
            // Find player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        protected virtual bool IsTargetValid()
        {
            if (target == null) return false;

            Health targetHealth = target.GetComponent<Health>();
            return targetHealth != null && targetHealth.IsAlive;
        }

        #endregion

        #region State Management

        protected virtual void ChangeState(AIState newState)
        {
            if (currentState == newState) return;

            currentState = newState;
            OnStateChanged(newState);
        }

        protected virtual void OnStateChanged(AIState newState)
        {
            // Update animator
            if (animator != null)
            {
                animator.SetInteger("State", (int)newState);
            }
        }

        #endregion

        #region Cooldowns

        protected virtual void UpdateCooldowns()
        {
            if (attackCooldownTimer > 0)
                attackCooldownTimer -= Time.deltaTime;

            if (specialAbilityCooldownTimer > 0)
                specialAbilityCooldownTimer -= Time.deltaTime;
        }

        #endregion

        #region Death & Rewards

        protected virtual void HandleDeath(GameObject killer)
        {
            ChangeState(AIState.Dead);

            // Grant experience
            GameEvents.PlayerExperienceGained(data.experienceValue);

            // Grant currency
            GameEvents.CurrencyGained(ProgressionCurrency.Souls, data.soulsCurrency);

            // Drop loot
            if (Random.value <= data.lootDropChance)
            {
                SpawnLoot();
            }

            // Spawn death effect
            if (data.deathEffectPrefab != null && GameManager.Instance != null)
            {
                GameManager.Instance.PoolManager.SpawnFromPool(
                    data.deathEffectPrefab.name,
                    transform.position,
                    Quaternion.identity
                );
            }

            // Play death sound
            if (data.deathSound != null)
            {
                AudioSource.PlayClipAtPoint(data.deathSound, transform.position);
            }

            // Fire event
            GameEvents.EnemyKilled(gameObject, data.GetDamageForFloor(currentFloor), transform.position);

            // Destroy or return to pool
            Destroy(gameObject, 0.1f);
        }

        protected virtual void SpawnLoot()
        {
            // Loot spawn logic handled by LootManager
            // This is a placeholder for enemy-specific loot
        }

        #endregion
    }
}
