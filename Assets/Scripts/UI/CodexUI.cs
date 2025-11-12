using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VampireSurvivor.Narrative;

namespace VampireSurvivor.UI
{
    /// <summary>
    /// Codex/Lore browser UI
    /// </summary>
    public class CodexUI : MonoBehaviour
    {
        [Header("Lists")]
        [SerializeField] private Transform entryListContainer;
        [SerializeField] private GameObject entryListItemPrefab;

        [Header("Detail View")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI contentText;
        [SerializeField] private Image entryImage;

        [Header("Categories")]
        [SerializeField] private TMP_Dropdown categoryDropdown;

        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI statsText;

        private NarrativeManager narrativeManager;
        private List<CodexEntryData> currentEntries = new List<CodexEntryData>();

        private void Start()
        {
            narrativeManager = FindObjectOfType<NarrativeManager>();

            if (categoryDropdown != null)
            {
                categoryDropdown.onValueChanged.AddListener(OnCategoryChanged);
            }

            RefreshCodexList();
        }

        private void RefreshCodexList()
        {
            if (narrativeManager == null) return;

            // Clear existing list
            foreach (Transform child in entryListContainer)
            {
                Destroy(child.gameObject);
            }

            // Get entries
            currentEntries = narrativeManager.GetUnlockedEntries();

            // Filter by category if needed
            if (categoryDropdown != null && categoryDropdown.value > 0)
            {
                CodexEntryData.CodexCategory category = (CodexEntryData.CodexCategory)(categoryDropdown.value - 1);
                currentEntries = currentEntries.FindAll(e => e.category == category);
            }

            // Create list items
            foreach (CodexEntryData entry in currentEntries)
            {
                GameObject item = Instantiate(entryListItemPrefab, entryListContainer);
                CodexListItem listItem = item.GetComponent<CodexListItem>();

                if (listItem != null)
                {
                    listItem.Setup(entry, this);
                }
            }

            // Update stats
            if (statsText != null)
            {
                statsText.text = $"Unlocked: {narrativeManager.UnlockedCodexCount} / {narrativeManager.TotalCodexEntries}";
            }
        }

        public void ShowEntry(CodexEntryData entry)
        {
            if (titleText != null)
                titleText.text = entry.entryTitle;

            if (contentText != null)
                contentText.text = entry.entryText;

            if (entryImage != null)
            {
                if (entry.entryImage != null)
                {
                    entryImage.sprite = entry.entryImage;
                    entryImage.enabled = true;
                }
                else
                {
                    entryImage.enabled = false;
                }
            }
        }

        private void OnCategoryChanged(int value)
        {
            RefreshCodexList();
        }
    }

    /// <summary>
    /// Individual codex list item
    /// </summary>
    public class CodexListItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button selectButton;

        private CodexEntryData entry;
        private CodexUI codexUI;

        public void Setup(CodexEntryData entryData, CodexUI ui)
        {
            entry = entryData;
            codexUI = ui;

            if (titleText != null)
                titleText.text = entry.entryTitle;

            if (selectButton != null)
                selectButton.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            codexUI?.ShowEntry(entry);
        }
    }
}
