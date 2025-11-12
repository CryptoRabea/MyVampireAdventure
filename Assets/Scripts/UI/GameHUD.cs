using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VampireSurvivor.Core;
using VampireSurvivor.Player;

namespace VampireSurvivor.UI
{
    /// <summary>
    /// Main in-game HUD
    /// Displays health, experience, abilities, etc.
    /// </summary>
    public class GameHUD : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("Experience")]
        [SerializeField] private Slider experienceBar;
        [SerializeField] private TextMeshProUGUI levelText;

        [Header("Abilities")]
        [SerializeField] private AbilitySlotUI[] abilitySlots = new AbilitySlotUI[4];

        [Header("Currency")]
        [SerializeField] private TextMeshProUGUI soulsText;

        [Header("Timer")]
        [SerializeField] private TextMeshProUGUI timerText;

        [Header("FPS (Debug)")]
        [SerializeField] private TextMeshProUGUI fpsText;
        [SerializeField] private bool showFPS = false;

        private PlayerController player;
        private PlayerProgression progression;
        private Health playerHealth;
        private Ashmarks.AshmarkManager ashmarkManager;

        private float runTimer = 0f;
        private float fpsTimer = 0f;
        private int frameCount = 0;

        private void OnEnable()
        {
            GameEvents.OnPlayerHealthChanged += UpdateHealthBar;
            GameEvents.OnPlayerLevelUp += UpdateLevel;
            GameEvents.OnPlayerExperienceGained += UpdateExperience;
            GameEvents.OnCurrencyGained += UpdateCurrency;
            GameEvents.OnRunStarted += HandleRunStarted;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerHealthChanged -= UpdateHealthBar;
            GameEvents.OnPlayerLevelUp -= UpdateLevel;
            GameEvents.OnPlayerExperienceGained -= UpdateExperience;
            GameEvents.OnCurrencyGained -= UpdateCurrency;
            GameEvents.OnRunStarted -= HandleRunStarted;
        }

        private void Start()
        {
            FindPlayerReferences();
            InitializeUI();
        }

        private void Update()
        {
            UpdateTimer();
            UpdateAbilitySlots();

            if (showFPS)
            {
                UpdateFPS();
            }
        }

        private void FindPlayerReferences()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.GetComponent<PlayerController>();
                progression = playerObj.GetComponent<PlayerProgression>();
                playerHealth = playerObj.GetComponent<Health>();
                ashmarkManager = playerObj.GetComponent<Ashmarks.AshmarkManager>();
            }
        }

        private void InitializeUI()
        {
            if (playerHealth != null)
            {
                UpdateHealthBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            }

            if (progression != null)
            {
                UpdateLevel(progression.CurrentLevel);
                UpdateExperience(0);
            }

            UpdateCurrency(ProgressionCurrency.Souls, 0);
        }

        private void HandleRunStarted()
        {
            runTimer = 0f;
            FindPlayerReferences();
            InitializeUI();
        }

        private void UpdateHealthBar(float current, float max)
        {
            if (healthBar != null)
            {
                healthBar.maxValue = max;
                healthBar.value = current;
            }

            if (healthText != null)
            {
                healthText.text = $"{Mathf.Ceil(current)} / {max}";
            }
        }

        private void UpdateLevel(int level)
        {
            if (levelText != null)
            {
                levelText.text = $"LV {level}";
            }
        }

        private void UpdateExperience(float exp)
        {
            if (progression == null || experienceBar == null) return;

            experienceBar.maxValue = progression.ExperienceRequiredForNextLevel;
            experienceBar.value = progression.CurrentExperience;
        }

        private void UpdateCurrency(ProgressionCurrency type, int amount)
        {
            if (type == ProgressionCurrency.Souls && soulsText != null)
            {
                // Get total souls from meta progression
                Loot.MetaProgression metaProg = FindObjectOfType<Loot.MetaProgression>();
                if (metaProg != null)
                {
                    soulsText.text = metaProg.GetCurrency(ProgressionCurrency.Souls).ToString();
                }
            }
        }

        private void UpdateTimer()
        {
            if (GameManager.Instance?.CurrentState == GameState.InRun)
            {
                runTimer += Time.deltaTime;

                if (timerText != null)
                {
                    int minutes = Mathf.FloorToInt(runTimer / 60f);
                    int seconds = Mathf.FloorToInt(runTimer % 60);
                    timerText.text = $"{minutes:00}:{seconds:00}";
                }
            }
        }

        private void UpdateAbilitySlots()
        {
            if (ashmarkManager == null) return;

            var equippedAshmarks = ashmarkManager.EquippedAshmarks;

            for (int i = 0; i < abilitySlots.Length; i++)
            {
                if (abilitySlots[i] != null)
                {
                    if (i < equippedAshmarks.Count && !equippedAshmarks[i].IsPassive)
                    {
                        abilitySlots[i].UpdateSlot(equippedAshmarks[i]);
                    }
                    else
                    {
                        abilitySlots[i].ClearSlot();
                    }
                }
            }
        }

        private void UpdateFPS()
        {
            if (fpsText == null) return;

            frameCount++;
            fpsTimer += Time.deltaTime;

            if (fpsTimer >= 1f)
            {
                int fps = Mathf.RoundToInt(frameCount / fpsTimer);
                fpsText.text = $"FPS: {fps}";

                frameCount = 0;
                fpsTimer = 0f;
            }
        }
    }

    /// <summary>
    /// Individual ability slot UI component
    /// </summary>
    [System.Serializable]
    public class AbilitySlotUI
    {
        public Image iconImage;
        public Image cooldownOverlay;
        public TextMeshProUGUI cooldownText;

        public void UpdateSlot(Ashmarks.BaseAshmark ashmark)
        {
            if (iconImage != null && ashmark.Data.icon != null)
            {
                iconImage.sprite = ashmark.Data.icon;
                iconImage.enabled = true;
            }

            // Update cooldown
            if (cooldownOverlay != null)
            {
                float cooldownPercent = ashmark.CurrentCooldown / ashmark.Data.cooldown;
                cooldownOverlay.fillAmount = cooldownPercent;
            }

            if (cooldownText != null)
            {
                if (ashmark.CurrentCooldown > 0)
                {
                    cooldownText.text = Mathf.Ceil(ashmark.CurrentCooldown).ToString();
                    cooldownText.enabled = true;
                }
                else
                {
                    cooldownText.enabled = false;
                }
            }
        }

        public void ClearSlot()
        {
            if (iconImage != null) iconImage.enabled = false;
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0;
            if (cooldownText != null) cooldownText.enabled = false;
        }
    }
}
