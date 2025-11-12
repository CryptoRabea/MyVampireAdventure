using System;
using System.IO;
using UnityEngine;
using VampireSurvivor.Loot;
using VampireSurvivor.Narrative;

namespace VampireSurvivor.Save
{
    /// <summary>
    /// Manages game save/load functionality
    /// Handles persistent data (meta progression, codex, settings, etc.)
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string saveFileName = "gamesave.json";
        [SerializeField] private bool autoSave = true;
        [SerializeField] private float autoSaveInterval = 60f; // Seconds

        private string SaveFilePath => Path.Combine(Application.persistentDataPath, saveFileName);
        private float autoSaveTimer = 0f;

        private GameSaveData currentSaveData;

        private void Update()
        {
            if (autoSave)
            {
                autoSaveTimer += Time.deltaTime;
                if (autoSaveTimer >= autoSaveInterval)
                {
                    SaveGame();
                    autoSaveTimer = 0f;
                }
            }
        }

        /// <summary>
        /// Save the game
        /// </summary>
        public void SaveGame()
        {
            try
            {
                currentSaveData = GatherSaveData();
                string json = JsonUtility.ToJson(currentSaveData, true);

                File.WriteAllText(SaveFilePath, json);
                Debug.Log($"[SaveManager] Game saved to {SaveFilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Failed to save game: {e.Message}");
            }
        }

        /// <summary>
        /// Load the game
        /// </summary>
        public void LoadGame()
        {
            try
            {
                if (!File.Exists(SaveFilePath))
                {
                    Debug.Log("[SaveManager] No save file found, creating new save");
                    currentSaveData = new GameSaveData();
                    return;
                }

                string json = File.ReadAllText(SaveFilePath);
                currentSaveData = JsonUtility.FromJson<GameSaveData>(json);

                ApplySaveData(currentSaveData);
                Debug.Log($"[SaveManager] Game loaded from {SaveFilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Failed to load game: {e.Message}");
                currentSaveData = new GameSaveData();
            }
        }

        /// <summary>
        /// Delete save file
        /// </summary>
        public void DeleteSave()
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    File.Delete(SaveFilePath);
                    Debug.Log("[SaveManager] Save file deleted");
                }

                currentSaveData = new GameSaveData();
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Failed to delete save: {e.Message}");
            }
        }

        /// <summary>
        /// Check if save file exists
        /// </summary>
        public bool SaveExists()
        {
            return File.Exists(SaveFilePath);
        }

        /// <summary>
        /// Gather all save data from various systems
        /// </summary>
        private GameSaveData GatherSaveData()
        {
            GameSaveData data = new GameSaveData
            {
                saveDate = DateTime.Now.ToString(),
                playTime = Time.realtimeSinceStartup
            };

            // Gather meta progression data
            MetaProgression metaProgression = FindObjectOfType<MetaProgression>();
            if (metaProgression != null)
            {
                data.metaProgressionData = metaProgression.GetSaveData();
            }

            // Gather narrative data
            NarrativeManager narrativeManager = FindObjectOfType<NarrativeManager>();
            if (narrativeManager != null)
            {
                data.narrativeData = narrativeManager.GetSaveData();
            }

            // Gather run statistics
            RunStatistics runStats = FindObjectOfType<RunStatistics>();
            if (runStats != null)
            {
                data.runStatistics = runStats.GetSaveData();
            }

            // Gather settings
            data.settings = SettingsSaveData.Current;

            return data;
        }

        /// <summary>
        /// Apply loaded save data to various systems
        /// </summary>
        private void ApplySaveData(GameSaveData data)
        {
            if (data == null) return;

            // Apply meta progression
            MetaProgression metaProgression = FindObjectOfType<MetaProgression>();
            if (metaProgression != null && data.metaProgressionData != null)
            {
                metaProgression.LoadSaveData(data.metaProgressionData);
            }

            // Apply narrative data
            NarrativeManager narrativeManager = FindObjectOfType<NarrativeManager>();
            if (narrativeManager != null && data.narrativeData != null)
            {
                narrativeManager.LoadSaveData(data.narrativeData);
            }

            // Apply run statistics
            RunStatistics runStats = FindObjectOfType<RunStatistics>();
            if (runStats != null && data.runStatistics != null)
            {
                runStats.LoadSaveData(data.runStatistics);
            }

            // Apply settings
            if (data.settings != null)
            {
                SettingsSaveData.Current = data.settings;
                SettingsSaveData.ApplySettings();
            }
        }

        /// <summary>
        /// Get current save data (for display purposes)
        /// </summary>
        public GameSaveData GetCurrentSaveData()
        {
            return currentSaveData;
        }
    }

    /// <summary>
    /// Complete save data structure
    /// </summary>
    [Serializable]
    public class GameSaveData
    {
        public string saveDate;
        public float playTime;

        public MetaProgressionSaveData metaProgressionData;
        public NarrativeSaveData narrativeData;
        public RunStatisticsSaveData runStatistics;
        public SettingsSaveData settings;
    }

    /// <summary>
    /// Run statistics save data
    /// </summary>
    [Serializable]
    public class RunStatisticsSaveData
    {
        public int totalRuns;
        public int successfulRuns;
        public int highestFloorReached;
        public float longestRunTime;
        public int totalEnemiesKilled;
        public float totalDamageDealt;
    }

    /// <summary>
    /// Settings save data
    /// </summary>
    [Serializable]
    public class SettingsSaveData
    {
        public float masterVolume = 1f;
        public float musicVolume = 0.8f;
        public float sfxVolume = 1f;
        public bool vibrationEnabled = true;
        public int graphicsQuality = 2; // 0=Low, 1=Medium, 2=High

        public static SettingsSaveData Current = new SettingsSaveData();

        public static void ApplySettings()
        {
            AudioListener.volume = Current.masterVolume;
            QualitySettings.SetQualityLevel(Current.graphicsQuality);
            // Apply other settings as needed
        }
    }

    /// <summary>
    /// Run statistics tracker
    /// </summary>
    public class RunStatistics : MonoBehaviour
    {
        private RunStatisticsSaveData stats = new RunStatisticsSaveData();

        private void OnEnable()
        {
            Core.GameEvents.OnRunEnded += HandleRunEnded;
            Core.GameEvents.OnEnemyKilled += HandleEnemyKilled;
            Core.GameEvents.OnFloorCompleted += HandleFloorCompleted;
        }

        private void OnDisable()
        {
            Core.GameEvents.OnRunEnded -= HandleRunEnded;
            Core.GameEvents.OnEnemyKilled -= HandleEnemyKilled;
            Core.GameEvents.OnFloorCompleted -= HandleFloorCompleted;
        }

        private void HandleRunEnded(bool victory)
        {
            stats.totalRuns++;
            if (victory) stats.successfulRuns++;
        }

        private void HandleEnemyKilled(GameObject enemy, float damage, Vector3 position)
        {
            stats.totalEnemiesKilled++;
            stats.totalDamageDealt += damage;
        }

        private void HandleFloorCompleted(int floor)
        {
            if (floor > stats.highestFloorReached)
            {
                stats.highestFloorReached = floor;
            }
        }

        public RunStatisticsSaveData GetSaveData()
        {
            return stats;
        }

        public void LoadSaveData(RunStatisticsSaveData data)
        {
            stats = data ?? new RunStatisticsSaveData();
        }
    }
}
