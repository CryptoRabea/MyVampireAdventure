using UnityEngine;

namespace VampireSurvivor.Narrative
{
    /// <summary>
    /// ScriptableObject for codex/lore entries
    /// Create via: Assets > Create > Vampire Survivor > Codex Entry
    /// </summary>
    [CreateAssetMenu(fileName = "New Codex Entry", menuName = "Vampire Survivor/Codex Entry", order = 6)]
    public class CodexEntryData : ScriptableObject
    {
        [Header("Entry Info")]
        public string entryId = "codex_001";
        public string entryTitle = "Ancient Artifact";
        public CodexCategory category = CodexCategory.Lore;

        [Header("Content")]
        [TextArea(5, 15)]
        public string entryText = "Long ago, in a forgotten age...";

        [Header("Visual")]
        public Sprite entryImage;

        [Header("Unlock")]
        public bool isUnlocked = false;
        public string unlockCondition = "Complete Floor 1";

        public enum CodexCategory
        {
            Lore,
            Enemies,
            Weapons,
            Ashmarks,
            Characters,
            Locations
        }
    }
}
