using System.Collections.Generic;
using UnityEngine;
using VampireSurvivor.Core;

namespace VampireSurvivor.Narrative
{
    /// <summary>
    /// Manages narrative elements: codex, echoes, dialogues
    /// Tracks player's story progression and unlocked lore
    /// </summary>
    public class NarrativeManager : MonoBehaviour
    {
        [Header("Codex")]
        [SerializeField] private List<CodexEntryData> allCodexEntries = new List<CodexEntryData>();

        [Header("Echo Moments")]
        [SerializeField] private List<EchoMomentData> echoMoments = new List<EchoMomentData>();

        [Header("Dialogues")]
        [SerializeField] private List<DialogueData> dialogues = new List<DialogueData>();

        private HashSet<string> unlockedCodexEntries = new HashSet<string>();
        private HashSet<string> triggeredEchoMoments = new HashSet<string>();
        private Queue<string> pendingDialogues = new Queue<string>();

        public int TotalCodexEntries => allCodexEntries.Count;
        public int UnlockedCodexCount => unlockedCodexEntries.Count;

        private void OnEnable()
        {
            GameEvents.OnCodexUnlocked += UnlockCodexEntry;
            GameEvents.OnFloorCompleted += CheckForEchoMoments;
        }

        private void OnDisable()
        {
            GameEvents.OnCodexUnlocked -= UnlockCodexEntry;
            GameEvents.OnFloorCompleted -= CheckForEchoMoments;
        }

        /// <summary>
        /// Unlock a codex entry
        /// </summary>
        public void UnlockCodexEntry(string entryId)
        {
            if (unlockedCodexEntries.Contains(entryId))
            {
                Debug.LogWarning($"[NarrativeManager] Codex entry already unlocked: {entryId}");
                return;
            }

            CodexEntryData entry = allCodexEntries.Find(e => e.entryId == entryId);
            if (entry != null)
            {
                entry.isUnlocked = true;
                unlockedCodexEntries.Add(entryId);

                Debug.Log($"[NarrativeManager] Unlocked codex entry: {entry.entryTitle}");

                // Optionally show notification to player
            }
            else
            {
                Debug.LogWarning($"[NarrativeManager] Codex entry not found: {entryId}");
            }
        }

        /// <summary>
        /// Get all unlocked codex entries
        /// </summary>
        public List<CodexEntryData> GetUnlockedEntries()
        {
            return allCodexEntries.FindAll(e => e.isUnlocked);
        }

        /// <summary>
        /// Get codex entries by category
        /// </summary>
        public List<CodexEntryData> GetEntriesByCategory(CodexEntryData.CodexCategory category)
        {
            return allCodexEntries.FindAll(e => e.category == category && e.isUnlocked);
        }

        /// <summary>
        /// Check for echo moments based on floor completion
        /// </summary>
        private void CheckForEchoMoments(int floor)
        {
            foreach (EchoMomentData echo in echoMoments)
            {
                if (echo.triggerFloor == floor && !triggeredEchoMoments.Contains(echo.echoId))
                {
                    TriggerEchoMoment(echo);
                }
            }
        }

        /// <summary>
        /// Trigger an echo moment (in-run story snippet)
        /// </summary>
        public void TriggerEchoMoment(EchoMomentData echo)
        {
            if (triggeredEchoMoments.Contains(echo.echoId)) return;

            triggeredEchoMoments.Add(echo.echoId);
            GameEvents.EchoMomentTriggered(echo.echoId);

            Debug.Log($"[NarrativeManager] Echo moment triggered: {echo.echoTitle}");

            // Display echo to player (handled by UI)
            // Optionally unlock related codex entries
            if (!string.IsNullOrEmpty(echo.unlocksCodexEntry))
            {
                UnlockCodexEntry(echo.unlocksCodexEntry);
            }
        }

        /// <summary>
        /// Queue a dialogue to be shown
        /// </summary>
        public void QueueDialogue(string dialogueId)
        {
            if (!pendingDialogues.Contains(dialogueId))
            {
                pendingDialogues.Enqueue(dialogueId);
            }
        }

        /// <summary>
        /// Start next dialogue in queue
        /// </summary>
        public void PlayNextDialogue()
        {
            if (pendingDialogues.Count == 0) return;

            string dialogueId = pendingDialogues.Dequeue();
            DialogueData dialogue = dialogues.Find(d => d.dialogueId == dialogueId);

            if (dialogue != null)
            {
                GameEvents.DialogueStarted(dialogueId);
                // UI will handle displaying the dialogue
            }
        }

        /// <summary>
        /// Get save data
        /// </summary>
        public NarrativeSaveData GetSaveData()
        {
            return new NarrativeSaveData
            {
                unlockedCodexEntries = new List<string>(unlockedCodexEntries),
                triggeredEchoMoments = new List<string>(triggeredEchoMoments)
            };
        }

        /// <summary>
        /// Load save data
        /// </summary>
        public void LoadSaveData(NarrativeSaveData data)
        {
            if (data == null) return;

            unlockedCodexEntries = new HashSet<string>(data.unlockedCodexEntries);
            triggeredEchoMoments = new HashSet<string>(data.triggeredEchoMoments);

            // Mark entries as unlocked
            foreach (string entryId in unlockedCodexEntries)
            {
                CodexEntryData entry = allCodexEntries.Find(e => e.entryId == entryId);
                if (entry != null) entry.isUnlocked = true;
            }
        }
    }

    [System.Serializable]
    public class NarrativeSaveData
    {
        public List<string> unlockedCodexEntries;
        public List<string> triggeredEchoMoments;
    }

    /// <summary>
    /// Echo moment data - short story snippets during runs
    /// </summary>
    [System.Serializable]
    public class EchoMomentData
    {
        public string echoId;
        public string echoTitle;
        [TextArea(3, 8)]
        public string echoText;
        public int triggerFloor;
        public string unlocksCodexEntry; // Optional codex unlock
        public Sprite echoImage;
    }

    /// <summary>
    /// Dialogue data for conversations with NPCs
    /// </summary>
    [System.Serializable]
    public class DialogueData
    {
        public string dialogueId;
        public string speakerName;
        public List<DialogueLine> lines = new List<DialogueLine>();
    }

    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        public Sprite portrait;
        [TextArea(2, 5)]
        public string text;
        public AudioClip voiceClip;
    }
}
