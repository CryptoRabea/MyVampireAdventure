using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VampireSurvivor.Core;

namespace VampireSurvivor.UI
{
    /// <summary>
    /// Run end screen - shows victory/defeat and rewards
    /// </summary>
    public class RunEndUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private GameObject defeatPanel;

        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI floorReachedText;
        [SerializeField] private TextMeshProUGUI enemiesKilledText;
        [SerializeField] private TextMeshProUGUI damageDealtText;
        [SerializeField] private TextMeshProUGUI timeText;

        [Header("Rewards")]
        [SerializeField] private TextMeshProUGUI soulsEarnedText;
        [SerializeField] private TextMeshProUGUI essenceEarnedText;

        [Header("Buttons")]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button hubButton;

        private bool isVictory;
        private RunStats currentRunStats;

        private void OnEnable()
        {
            GameEvents.OnRunEnded += ShowRunEnd;
        }

        private void OnDisable()
        {
            GameEvents.OnRunEnded -= ShowRunEnd;
        }

        private void Start()
        {
            SetupButtons();
            gameObject.SetActive(false);
        }

        private void SetupButtons()
        {
            if (continueButton != null)
                continueButton.onClick.AddListener(Continue);

            if (retryButton != null)
                retryButton.onClick.AddListener(Retry);

            if (hubButton != null)
                hubButton.onClick.AddListener(ReturnToHub);
        }

        private void ShowRunEnd(bool victory)
        {
            isVictory = victory;
            currentRunStats = GatherRunStats();

            gameObject.SetActive(true);
            victoryPanel?.SetActive(victory);
            defeatPanel?.SetActive(!victory);

            DisplayStats();
            DisplayRewards();

            Time.timeScale = 0f; // Pause game
        }

        private RunStats GatherRunStats()
        {
            RunStats stats = new RunStats();

            // Gather stats from run
            // This would need to be tracked during the run
            // Placeholder values for now

            return stats;
        }

        private void DisplayStats()
        {
            if (floorReachedText != null)
                floorReachedText.text = $"Floor: {currentRunStats.floorReached}";

            if (enemiesKilledText != null)
                enemiesKilledText.text = $"Enemies: {currentRunStats.enemiesKilled}";

            if (damageDealtText != null)
                damageDealtText.text = $"Damage: {Mathf.RoundToInt(currentRunStats.damageDealt)}";

            if (timeText != null)
            {
                int minutes = Mathf.FloorToInt(currentRunStats.timeElapsed / 60f);
                int seconds = Mathf.FloorToInt(currentRunStats.timeElapsed % 60);
                timeText.text = $"Time: {minutes:00}:{seconds:00}";
            }
        }

        private void DisplayRewards()
        {
            // Calculate rewards based on performance
            int soulsEarned = currentRunStats.enemiesKilled * 2 + currentRunStats.floorReached * 10;
            int essenceEarned = isVictory ? currentRunStats.floorReached * 5 : currentRunStats.floorReached * 2;

            if (soulsEarnedText != null)
                soulsEarnedText.text = $"+{soulsEarned} Souls";

            if (essenceEarnedText != null)
                essenceEarnedText.text = $"+{essenceEarned} Essence";

            // Award currency
            GameEvents.CurrencyGained(ProgressionCurrency.Souls, soulsEarned);
            GameEvents.CurrencyGained(ProgressionCurrency.Essence, essenceEarned);
        }

        private void Continue()
        {
            Time.timeScale = 1f;
            gameObject.SetActive(false);

            if (isVictory)
            {
                // Continue to next floor or hub
                ReturnToHub();
            }
            else
            {
                ReturnToHub();
            }
        }

        private void Retry()
        {
            Time.timeScale = 1f;
            gameObject.SetActive(false);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartRun();
            }
        }

        private void ReturnToHub()
        {
            Time.timeScale = 1f;
            gameObject.SetActive(false);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReturnToHub();
            }
        }
    }

    [System.Serializable]
    public class RunStats
    {
        public int floorReached;
        public int enemiesKilled;
        public float damageDealt;
        public float timeElapsed;
    }
}
