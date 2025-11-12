using UnityEngine;
using UnityEngine.InputSystem;
using VampireSurvivor.Core;

namespace VampireSurvivor.Player
{
    /// <summary>
    /// Main player controller - handles movement, dodging, and aiming
    /// Optimized for mobile touch controls and gamepad
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Health))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float deceleration = 10f;

        [Header("Dodge")]
        [SerializeField] private float dodgeDistance = 3f;
        [SerializeField] private float dodgeDuration = 0.2f;
        [SerializeField] private float dodgeCooldown = 1f;
        [SerializeField] private bool invulnerableWhileDodging = true;

        [Header("Aiming")]
        [SerializeField] private bool autoAim = true;
        [SerializeField] private float autoAimRange = 10f;
        [SerializeField] private LayerMask enemyLayer;

        [Header("Components")]
        [SerializeField] private Transform aimTransform; // Visual indicator for aim direction

        // Components
        private Rigidbody2D rb;
        private Health health;
        private PlayerCombat combat;

        // Movement state
        private Vector2 moveInput;
        private Vector2 currentVelocity;
        private Vector2 aimDirection = Vector2.right;

        // Dodge state
        private bool isDodging = false;
        private float dodgeTimer = 0f;
        private float dodgeCooldownTimer = 0f;
        private Vector2 dodgeDirection;

        // Properties
        public Vector2 AimDirection => aimDirection;
        public bool IsDodging => isDodging;
        public bool CanDodge => dodgeCooldownTimer <= 0 && !isDodging;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
            combat = GetComponent<PlayerCombat>();

            // Rigidbody2D settings for top-down movement
            rb.gravityScale = 0f;
            rb.drag = 0f;
            rb.freezeRotation = true;
        }

        private void OnEnable()
        {
            health.OnDeath += HandlePlayerDeath;
        }

        private void OnDisable()
        {
            health.OnDeath -= HandlePlayerDeath;
        }

        private void Update()
        {
            HandleDodgeTimers();
            UpdateAimDirection();

            if (aimTransform != null)
            {
                float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                aimTransform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        private void FixedUpdate()
        {
            if (isDodging)
            {
                HandleDodgeMovement();
            }
            else
            {
                HandleMovement();
            }
        }

        /// <summary>
        /// Handle normal movement with acceleration/deceleration
        /// </summary>
        private void HandleMovement()
        {
            Vector2 targetVelocity = moveInput.normalized * moveSpeed;

            // Smooth acceleration/deceleration
            float accelRate = (moveInput.magnitude > 0) ? acceleration : deceleration;
            currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, accelRate * Time.fixedDeltaTime);

            rb.velocity = currentVelocity;
        }

        /// <summary>
        /// Handle dodge movement
        /// </summary>
        private void HandleDodgeMovement()
        {
            float t = 1f - (dodgeTimer / dodgeDuration);
            float speed = Mathf.Lerp(dodgeDistance / dodgeDuration, 0f, t);

            rb.velocity = dodgeDirection * speed;
        }

        /// <summary>
        /// Update dodge timers
        /// </summary>
        private void HandleDodgeTimers()
        {
            if (isDodging)
            {
                dodgeTimer -= Time.deltaTime;
                if (dodgeTimer <= 0)
                {
                    EndDodge();
                }
            }

            if (dodgeCooldownTimer > 0)
            {
                dodgeCooldownTimer -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Update aim direction based on auto-aim or manual input
        /// </summary>
        private void UpdateAimDirection()
        {
            if (autoAim)
            {
                // Find nearest enemy
                GameObject nearestEnemy = FindNearestEnemy();
                if (nearestEnemy != null)
                {
                    Vector2 directionToEnemy = (nearestEnemy.transform.position - transform.position).normalized;
                    aimDirection = directionToEnemy;
                    return;
                }
            }

            // Default to movement direction if no enemy or auto-aim off
            if (moveInput.magnitude > 0.1f)
            {
                aimDirection = moveInput.normalized;
            }
        }

        /// <summary>
        /// Find the nearest enemy within auto-aim range
        /// </summary>
        private GameObject FindNearestEnemy()
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, autoAimRange, enemyLayer);

            GameObject nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (Collider2D enemy in enemies)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = enemy.gameObject;
                }
            }

            return nearest;
        }

        /// <summary>
        /// Perform a dodge
        /// </summary>
        public void Dodge()
        {
            if (!CanDodge) return;

            isDodging = true;
            dodgeTimer = dodgeDuration;
            dodgeCooldownTimer = dodgeCooldown;

            // Dodge in movement direction, or forward if not moving
            dodgeDirection = moveInput.magnitude > 0.1f ? moveInput.normalized : aimDirection;

            if (invulnerableWhileDodging)
            {
                health.SetInvulnerable(true);
            }

            Debug.Log("[PlayerController] Dodge executed");
        }

        /// <summary>
        /// End dodge state
        /// </summary>
        private void EndDodge()
        {
            isDodging = false;

            if (invulnerableWhileDodging)
            {
                health.SetInvulnerable(false);
            }
        }

        /// <summary>
        /// Handle player death
        /// </summary>
        private void HandlePlayerDeath(GameObject killer)
        {
            GameEvents.PlayerDeath();
            Debug.Log("[PlayerController] Player died");

            // Disable controls
            enabled = false;
        }

        #region Input System Callbacks

        /// <summary>
        /// Called by Input System for movement input
        /// </summary>
        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        /// <summary>
        /// Called by Input System for dodge input
        /// </summary>
        public void OnDodge(InputValue value)
        {
            if (value.isPressed)
            {
                Dodge();
            }
        }

        /// <summary>
        /// Called by Input System for aim input (manual aiming)
        /// </summary>
        public void OnAim(InputValue value)
        {
            Vector2 aimInput = value.Get<Vector2>();
            if (aimInput.magnitude > 0.1f)
            {
                aimDirection = aimInput.normalized;
                autoAim = false; // Disable auto-aim when manually aiming
            }
            else
            {
                autoAim = true; // Re-enable auto-aim
            }
        }

        #endregion

        private void OnDrawGizmosSelected()
        {
            // Draw auto-aim range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, autoAimRange);

            // Draw aim direction
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)aimDirection * 2f);
        }
    }
}
