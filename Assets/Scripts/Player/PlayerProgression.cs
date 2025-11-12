using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Player
{
    /// <summary>
    /// Handles player progression during a run - experience and leveling
    /// </summary>
    public class PlayerProgression : MonoBehaviour
    {
        [Header("Progression Settings")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private float currentExperience = 0f;
        [SerializeField] private float baseExperienceRequired = 100f;
        [SerializeField] private float experienceScaling = 1.5f;

        [Header("Stats Per Level")]
        [SerializeField] private float healthPerLevel = 10f;
        [SerializeField] private float damageMultiplierPerLevel = 0.05f;

        private float experienceRequiredForNextLevel;
        private float totalDamageMultiplier = 1f;

        public int CurrentLevel => currentLevel;
        public float CurrentExperience => currentExperience;
        public float ExperienceRequiredForNextLevel => experienceRequiredForNextLevel;
        public float TotalDamageMultiplier => totalDamageMultiplier;

        private Health health;

        private void Awake()
        {
            health = GetComponent<Health>();
            CalculateExperienceRequired();
        }

        private void OnEnable()
        {
            GameEvents.OnEnemyKilled += HandleEnemyKilled;
            GameEvents.OnPlayerExperienceGained += AddExperience;
        }

        private void OnDisable()
        {
            GameEvents.OnEnemyKilled -= HandleEnemyKilled;
            GameEvents.OnPlayerExperienceGained -= AddExperience;
        }

        /// <summary>
        /// Add experience to the player
        /// </summary>
        public void AddExperience(float amount)
        {
            currentExperience += amount;

            while (currentExperience >= experienceRequiredForNextLevel)
            {
                LevelUp();
            }
        }

        /// <summary>
        /// Level up the player
        /// </summary>
        private void LevelUp()
        {
            currentExperience -= experienceRequiredForNextLevel;
            currentLevel++;

            // Apply stat increases
            if (health != null)
            {
                health.SetMaxHealth(health.MaxHealth + healthPerLevel, healToMax: true);
            }

            totalDamageMultiplier += damageMultiplierPerLevel;

            CalculateExperienceRequired();

            GameEvents.PlayerLevelUp(currentLevel);
            Debug.Log($"[PlayerProgression] Level up! New level: {currentLevel}");

            // Trigger level-up reward selection (handled by UI)
        }

        /// <summary>
        /// Calculate experience required for next level
        /// </summary>
        private void CalculateExperienceRequired()
        {
            experienceRequiredForNextLevel = baseExperienceRequired * Mathf.Pow(experienceScaling, currentLevel - 1);
        }

        /// <summary>
        /// Handle enemy killed event
        /// </summary>
        private void HandleEnemyKilled(GameObject enemy, float damage, Vector3 position)
        {
            // Enemy experience is handled by enemy script
            // This is here for any additional logic needed
        }

        /// <summary>
        /// Get progress to next level (0-1)
        /// </summary>
        public float GetLevelProgress()
        {
            return currentExperience / experienceRequiredForNextLevel;
        }

        /// <summary>
        /// Reset progression for new run
        /// </summary>
        public void ResetProgression()
        {
            currentLevel = 1;
            currentExperience = 0f;
            totalDamageMultiplier = 1f;
            CalculateExperienceRequired();
        }
    }
}
