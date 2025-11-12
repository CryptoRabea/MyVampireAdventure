namespace VampireSurvivor.Core
{
    /// <summary>
    /// Core enumerations used throughout the game
    /// </summary>

    public enum EnemyType
    {
        Melee,
        Ranged,
        Special,
        Elite,
        Boss
    }

    public enum WeaponType
    {
        Projectile,
        Beam,
        AOE,
        Melee,
        Summon
    }

    public enum AshmarkType
    {
        Active,
        Passive,
        Triggered // Activates on specific conditions
    }

    public enum AshmarkRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public enum RoomType
    {
        Combat,
        Loot,
        Shop,
        Boss,
        Event,
        Safe
    }

    public enum DamageType
    {
        Physical,
        Fire,
        Ice,
        Lightning,
        Poison,
        Holy,
        Dark
    }

    public enum LootType
    {
        Weapon,
        Ashmark,
        Consumable,
        Currency,
        Codex
    }

    public enum ProgressionCurrency
    {
        Souls,      // Primary run currency
        Essence,    // Meta upgrade currency
        Fragments   // Special unlock currency
    }

    public enum GameState
    {
        MainMenu,
        Hub,
        InRun,
        Paused,
        GameOver,
        Victory,
        Cinematic
    }

    public enum AIState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Flee,
        Dead
    }
}
