using System;
using UnityEngine;

namespace VampireSurvivor.Core
{
    /// <summary>
    /// Central event system for game-wide communication
    /// Prevents tight coupling between systems
    /// </summary>
    public static class GameEvents
    {
        // Player Events
        public static event Action<float, float> OnPlayerHealthChanged; // current, max
        public static event Action OnPlayerDeath;
        public static event Action<int> OnPlayerLevelUp; // new level
        public static event Action<float> OnPlayerExperienceGained; // experience amount

        // Combat Events
        public static event Action<GameObject, float, Vector3> OnEnemyKilled; // enemy, damage, position
        public static event Action<GameObject> OnEnemySpawned;
        public static event Action<DamageType, float> OnDamageDealt; // type, amount

        // Weapon Events
        public static event Action<string> OnWeaponEquipped; // weapon name
        public static event Action<string> OnWeaponFired; // weapon name

        // Ashmark Events
        public static event Action<string> OnAshmarkEquipped; // ashmark name
        public static event Action<string> OnAshmarkActivated; // ashmark name

        // Loot Events
        public static event Action<LootType, string> OnLootCollected; // type, item name
        public static event Action<ProgressionCurrency, int> OnCurrencyGained; // currency type, amount

        // Progression Events
        public static event Action<string> OnUpgradePurchased; // upgrade name
        public static event Action<string> OnCodexUnlocked; // codex entry ID

        // Level Events
        public static event Action<RoomType> OnRoomEntered; // room type
        public static event Action OnRoomCleared;
        public static event Action<int> OnFloorCompleted; // floor number

        // Narrative Events
        public static event Action<string> OnEchoMomentTriggered; // echo ID
        public static event Action<string> OnDialogueStarted; // dialogue ID
        public static event Action OnDialogueEnded;

        // Game State Events
        public static event Action<GameState, GameState> OnGameStateChanged; // oldState, newState
        public static event Action OnRunStarted;
        public static event Action<bool> OnRunEnded; // victory or defeat
        public static event Action OnReturnToHub;

        // Invocation Methods
        public static void PlayerHealthChanged(float current, float max) => OnPlayerHealthChanged?.Invoke(current, max);
        public static void PlayerDeath() => OnPlayerDeath?.Invoke();
        public static void PlayerLevelUp(int level) => OnPlayerLevelUp?.Invoke(level);
        public static void PlayerExperienceGained(float exp) => OnPlayerExperienceGained?.Invoke(exp);

        public static void EnemyKilled(GameObject enemy, float damage, Vector3 position) => OnEnemyKilled?.Invoke(enemy, damage, position);
        public static void EnemySpawned(GameObject enemy) => OnEnemySpawned?.Invoke(enemy);
        public static void DamageDealt(DamageType type, float amount) => OnDamageDealt?.Invoke(type, amount);

        public static void WeaponEquipped(string weaponName) => OnWeaponEquipped?.Invoke(weaponName);
        public static void WeaponFired(string weaponName) => OnWeaponFired?.Invoke(weaponName);

        public static void AshmarkEquipped(string ashmarkName) => OnAshmarkEquipped?.Invoke(ashmarkName);
        public static void AshmarkActivated(string ashmarkName) => OnAshmarkActivated?.Invoke(ashmarkName);

        public static void LootCollected(LootType type, string itemName) => OnLootCollected?.Invoke(type, itemName);
        public static void CurrencyGained(ProgressionCurrency type, int amount) => OnCurrencyGained?.Invoke(type, amount);

        public static void UpgradePurchased(string upgradeName) => OnUpgradePurchased?.Invoke(upgradeName);
        public static void CodexUnlocked(string codexId) => OnCodexUnlocked?.Invoke(codexId);

        public static void RoomEntered(RoomType type) => OnRoomEntered?.Invoke(type);
        public static void RoomCleared() => OnRoomCleared?.Invoke();
        public static void FloorCompleted(int floorNumber) => OnFloorCompleted?.Invoke(floorNumber);

        public static void EchoMomentTriggered(string echoId) => OnEchoMomentTriggered?.Invoke(echoId);
        public static void DialogueStarted(string dialogueId) => OnDialogueStarted?.Invoke(dialogueId);
        public static void DialogueEnded() => OnDialogueEnded?.Invoke();

        public static void GameStateChanged(GameState oldState, GameState newState) => OnGameStateChanged?.Invoke(oldState, newState);
        public static void RunStarted() => OnRunStarted?.Invoke();
        public static void RunEnded(bool victory) => OnRunEnded?.Invoke(victory);
        public static void ReturnToHub() => OnReturnToHub?.Invoke();
    }
}
