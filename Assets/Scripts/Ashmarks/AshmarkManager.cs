using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VampireSurvivor.Core;

namespace VampireSurvivor.Ashmarks
{
    /// <summary>
    /// Manages player's equipped Ashmarks (abilities/equipment)
    /// Handles activation, cooldowns, and stat modifications
    /// </summary>
    public class AshmarkManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int maxActiveAshmarks = 4;
        [SerializeField] private int maxPassiveAshmarks = 8;

        [Header("Starting Ashmarks")]
        [SerializeField] private List<AshmarkData> startingAshmarks = new List<AshmarkData>();

        private List<BaseAshmark> equippedAshmarks = new List<BaseAshmark>();
        private Dictionary<AshmarkData.TriggerType, List<BaseAshmark>> triggeredAshmarks = new Dictionary<AshmarkData.TriggerType, List<BaseAshmark>>();

        public List<BaseAshmark> EquippedAshmarks => equippedAshmarks;
        public int ActiveAshmarkCount => equippedAshmarks.FindAll(a => !a.IsPassive).Count;
        public int PassiveAshmarkCount => equippedAshmarks.FindAll(a => a.IsPassive).Count;

        private void Start()
        {
            // Equip starting Ashmarks
            foreach (AshmarkData ashmarkData in startingAshmarks)
            {
                if (ashmarkData != null)
                {
                    EquipAshmark(ashmarkData);
                }
            }

            SubscribeToTriggerEvents();
        }

        private void Update()
        {
            UpdateAshmarkCooldowns();
        }

        private void OnDestroy()
        {
            UnsubscribeFromTriggerEvents();
        }

        /// <summary>
        /// Update all Ashmark cooldowns
        /// </summary>
        private void UpdateAshmarkCooldowns()
        {
            foreach (BaseAshmark ashmark in equippedAshmarks)
            {
                ashmark.UpdateCooldown(Time.deltaTime);
            }
        }

        /// <summary>
        /// Equip a new Ashmark
        /// </summary>
        public bool EquipAshmark(AshmarkData ashmarkData)
        {
            if (ashmarkData == null)
            {
                Debug.LogWarning("[AshmarkManager] Cannot equip null Ashmark");
                return false;
            }

            // Check slot limits
            bool isPassive = ashmarkData.type == AshmarkType.Passive;
            int currentCount = isPassive ? PassiveAshmarkCount : ActiveAshmarkCount;
            int maxCount = isPassive ? maxPassiveAshmarks : maxActiveAshmarks;

            if (currentCount >= maxCount)
            {
                Debug.LogWarning($"[AshmarkManager] Max {(isPassive ? "passive" : "active")} Ashmarks reached ({maxCount})");
                return false;
            }

            // Create Ashmark instance
            BaseAshmark ashmark = CreateAshmarkInstance(ashmarkData);
            if (ashmark == null)
            {
                Debug.LogError($"[AshmarkManager] Failed to create Ashmark instance for {ashmarkData.ashmarkName}");
                return false;
            }

            ashmark.Initialize(ashmarkData, gameObject);
            equippedAshmarks.Add(ashmark);

            // Register triggered Ashmarks
            if (ashmarkData.type == AshmarkType.Triggered)
            {
                if (!triggeredAshmarks.ContainsKey(ashmarkData.triggerType))
                {
                    triggeredAshmarks[ashmarkData.triggerType] = new List<BaseAshmark>();
                }
                triggeredAshmarks[ashmarkData.triggerType].Add(ashmark);
            }

            GameEvents.AshmarkEquipped(ashmarkData.ashmarkName);
            Debug.Log($"[AshmarkManager] Equipped Ashmark: {ashmarkData.ashmarkName}");

            return true;
        }

        /// <summary>
        /// Unequip an Ashmark by index
        /// </summary>
        public bool UnequipAshmark(int index)
        {
            if (index < 0 || index >= equippedAshmarks.Count) return false;

            BaseAshmark ashmark = equippedAshmarks[index];
            ashmark.Deactivate(gameObject);
            ashmark.RemoveStatModifiers(gameObject);

            equippedAshmarks.RemoveAt(index);

            Debug.Log($"[AshmarkManager] Unequipped Ashmark: {ashmark.AbilityName}");
            return true;
        }

        /// <summary>
        /// Manually activate an Ashmark by index
        /// </summary>
        public void ActivateAshmark(int index)
        {
            if (index < 0 || index >= equippedAshmarks.Count) return;

            BaseAshmark ashmark = equippedAshmarks[index];
            if (ashmark.CanActivate)
            {
                ashmark.Activate(gameObject);
            }
        }

        /// <summary>
        /// Create Ashmark instance based on type
        /// </summary>
        private BaseAshmark CreateAshmarkInstance(AshmarkData data)
        {
            BaseAshmark ashmark = null;

            switch (data.type)
            {
                case AshmarkType.Passive:
                    ashmark = new PassiveAshmark();
                    break;
                case AshmarkType.Active:
                    ashmark = new ActiveAshmark();
                    break;
                case AshmarkType.Triggered:
                    ashmark = new TriggeredAshmark();
                    break;
            }

            return ashmark;
        }

        /// <summary>
        /// Handle triggered Ashmarks
        /// </summary>
        private void TriggerAshmarks(AshmarkData.TriggerType triggerType)
        {
            if (!triggeredAshmarks.ContainsKey(triggerType)) return;

            foreach (BaseAshmark ashmark in triggeredAshmarks[triggerType])
            {
                if (ashmark.CanActivate && Random.value <= ashmark.Data.triggerChance)
                {
                    ashmark.Activate(gameObject);
                }
            }
        }

        #region Event Subscriptions

        private void SubscribeToTriggerEvents()
        {
            GameEvents.OnEnemyKilled += HandleEnemyKilled;
            GameEvents.OnDamageDealt += HandleDamageDealt;
            // Add more event subscriptions as needed
        }

        private void UnsubscribeFromTriggerEvents()
        {
            GameEvents.OnEnemyKilled -= HandleEnemyKilled;
            GameEvents.OnDamageDealt -= HandleDamageDealt;
        }

        private void HandleEnemyKilled(GameObject enemy, float damage, Vector3 position)
        {
            TriggerAshmarks(AshmarkData.TriggerType.OnKill);
        }

        private void HandleDamageDealt(DamageType type, float amount)
        {
            TriggerAshmarks(AshmarkData.TriggerType.OnHit);
        }

        #endregion

        #region Input System Callbacks

        /// <summary>
        /// Called by Input System for ability 1
        /// </summary>
        public void OnAbility1(InputValue value)
        {
            if (value.isPressed) ActivateAshmark(0);
        }

        /// <summary>
        /// Called by Input System for ability 2
        /// </summary>
        public void OnAbility2(InputValue value)
        {
            if (value.isPressed) ActivateAshmark(1);
        }

        /// <summary>
        /// Called by Input System for ability 3
        /// </summary>
        public void OnAbility3(InputValue value)
        {
            if (value.isPressed) ActivateAshmark(2);
        }

        /// <summary>
        /// Called by Input System for ability 4
        /// </summary>
        public void OnAbility4(InputValue value)
        {
            if (value.isPressed) ActivateAshmark(3);
        }

        #endregion
    }
}
