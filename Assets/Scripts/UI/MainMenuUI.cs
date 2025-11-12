using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VampireSurvivor.Core;

namespace VampireSurvivor.UI
{
    /// <summary>
    /// Main menu UI controller
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject codexPanel;

        [Header("Buttons")]
        [SerializeField] private Button startRunButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button codexButton;
        [SerializeField] private Button quitButton;

        [Header("Save Info")]
        [SerializeField] private TextMeshProUGUI saveInfoText;

        private void Start()
        {
            SetupButtons();
            UpdateSaveInfo();
            ShowMainPanel();
        }

        private void SetupButtons()
        {
            if (startRunButton != null)
                startRunButton.onClick.AddListener(StartNewRun);

            if (continueButton != null)
            {
                continueButton.onClick.AddListener(Continue);
                continueButton.interactable = GameManager.Instance?.SaveManager?.SaveExists() ?? false;
            }

            if (settingsButton != null)
                settingsButton.onClick.AddListener(ShowSettings);

            if (codexButton != null)
                codexButton.onClick.AddListener(ShowCodex);

            if (quitButton != null)
                quitButton.onClick.AddListener(QuitGame);
        }

        private void StartNewRun()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartRun();
            }
        }

        private void Continue()
        {
            if (GameManager.Instance?.SaveManager != null)
            {
                GameManager.Instance.SaveManager.LoadGame();
                GameManager.Instance.ReturnToHub();
            }
        }

        private void ShowSettings()
        {
            mainPanel?.SetActive(false);
            settingsPanel?.SetActive(true);
        }

        private void ShowCodex()
        {
            mainPanel?.SetActive(false);
            codexPanel?.SetActive(true);
        }

        private void ShowMainPanel()
        {
            mainPanel?.SetActive(true);
            settingsPanel?.SetActive(false);
            codexPanel?.SetActive(false);
        }

        private void QuitGame()
        {
            GameManager.Instance?.QuitGame();
        }

        private void UpdateSaveInfo()
        {
            if (saveInfoText == null) return;

            if (GameManager.Instance?.SaveManager?.SaveExists() ?? false)
            {
                var saveData = GameManager.Instance.SaveManager.GetCurrentSaveData();
                if (saveData != null)
                {
                    saveInfoText.text = $"Last Save: {saveData.saveDate}";
                }
            }
            else
            {
                saveInfoText.text = "No Save Data";
            }
        }

        public void BackToMain()
        {
            ShowMainPanel();
        }
    }
}
