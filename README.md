# Vampire Survivor Framework - Unity 6

A complete, production-ready framework for a top-down roguelike survivor shooter in Unity 6, optimized for mobile (Android/iOS).

## Overview

This framework provides a fully modular, extensible system for creating a roguelike survivor game inspired by *Vampire Survivors* and *Hero Adventure: Survivor RPG*, with an emphasis on **story/lore progression** that motivates players to complete runs.

### Key Features

✅ **Complete Core Systems**
- Player movement, aiming, shooting, dodging (mobile-optimized)
- Enemy AI with behavior tree-like structure (Melee, Ranged, Special)
- Modular weapon system (Projectile, AOE, Beam)
- Ashmark system (active/passive abilities with unique effects)
- Procedural level generation with room-based dungeons
- Object pooling for optimal mobile performance
- Full save/load system with persistent progression

✅ **Progression Systems**
- In-run progression (levels, experience, temporary upgrades)
- Meta-progression (permanent upgrades, unlocks)
- Multi-currency economy (Souls, Essence, Fragments)
- Loot and reward systems

✅ **Narrative Systems**
- Codex/lore system (unlockable entries)
- Echo moments (in-run story snippets)
- Dialogue system for NPCs
- Story progression tracking

✅ **UI Systems**
- In-game HUD (health, experience, abilities, timer)
- Main menu, hub, and settings
- Codex browser
- Run end screen with stats and rewards
- Mobile-friendly layouts

✅ **Production-Ready**
- ScriptableObject-driven content (easy to expand)
- Unity Input System integration (gamepad, touch, keyboard)
- Event-driven architecture (low coupling)
- Clean, commented code with interfaces
- Mobile optimization (pooling, atlasing, performance targets)

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/                    # Core interfaces, enums, events, GameManager
│   ├── Player/                  # PlayerController, Combat, Progression
│   ├── Enemies/                 # Enemy AI, enemy types (Melee, Ranged, Special)
│   ├── Weapons/                 # Weapon system, projectiles, weapon types
│   ├── Ashmarks/                # Ability system (passive, active, triggered)
│   ├── Procedural/              # Dungeon generation, room management
│   ├── Loot/                    # Loot drops, pickups, meta progression
│   ├── Narrative/               # Codex, echoes, dialogues
│   ├── Save/                    # Save system, persistence
│   ├── UI/                      # HUD, menus, codex UI
│   ├── Pooling/                 # Object pool manager
│   └── Utils/                   # Helpers, constants
├── Prefabs/
│   ├── Player/                  # Player prefab
│   ├── Enemies/                 # Enemy prefabs
│   ├── Weapons/                 # Weapon effects
│   ├── Projectiles/             # Projectile prefabs
│   ├── Rooms/                   # Room prefabs for proc gen
│   ├── UI/                      # UI prefabs
│   ├── VFX/                     # Visual effects
│   └── Loot/                    # Loot pickups
├── ScriptableObjects/
│   ├── Weapons/                 # Weapon data assets
│   ├── Enemies/                 # Enemy data assets
│   ├── Ashmarks/                # Ashmark data assets
│   ├── Rooms/                   # Room data assets
│   ├── Narrative/               # Codex entries
│   └── Progression/             # Meta upgrade data
├── Scenes/
│   ├── MainMenu.unity
│   ├── Hub.unity
│   └── Run.unity
└── [Other asset folders]
```

## Quick Start

### Prerequisites
- Unity 6.0+ (6000.0.0f1 or later)
- Basic understanding of Unity and C#

### Setup Steps

1. **Open the project in Unity 6**

2. **Install required packages:**
   - Universal RP
   - Input System
   - Cinemachine
   - TextMeshPro

3. **Configure Input System:**
   - Create Input Actions asset
   - Set up Player and UI action maps
   - See `SETUP_GUIDE.md` for details

4. **Create example content:**
   - Create weapon ScriptableObjects (Right-click > Create > Vampire Survivor > Weapon Data)
   - Create enemy ScriptableObjects (Right-click > Create > Vampire Survivor > Enemy Data)
   - Create Ashmark ScriptableObjects (Right-click > Create > Vampire Survivor > Ashmark Data)

5. **Setup scenes:**
   - Follow the scene setup guide in `SETUP_GUIDE.md`
   - Create player prefab with required components
   - Create enemy prefabs
   - Setup GameManager with pooling

6. **Test the vertical slice:**
   - Create a test room with enemies
   - Configure player with one weapon
   - Play and iterate

### First Playable (5-10 hours)

Follow the **"First Vertical Slice"** section in `SETUP_GUIDE.md` to create a playable demo in one work session.

## Architecture Overview

### Event-Driven System

The framework uses a central event system (`GameEvents`) to decouple systems:

```csharp
// Example: Enemy death triggers multiple systems
GameEvents.EnemyKilled(enemy, damage, position);

// Systems listening:
// - PlayerProgression: awards experience
// - LootManager: spawns loot
// - RunStatistics: tracks kills
// - NarrativeManager: checks for triggers
// - UI: updates kill counter
```

### ScriptableObject-Driven Content

All game content is defined via ScriptableObjects for easy expansion:

```csharp
WeaponData      // Defines weapon stats, behavior
EnemyData       // Defines enemy stats, AI parameters
AshmarkData     // Defines ability effects, cooldowns
RoomData        // Defines room layout, enemy spawns
CodexEntryData  // Defines lore entries
```

### Interface-Based Design

Core functionality uses interfaces for flexibility:

```csharp
IHealth       // Any entity with health
IDamageable   // Simplified damage interface
IWeapon       // Weapon functionality
IAbility      // Ability/Ashmark functionality
IPoolable     // Objects that can be pooled
```

## Core Systems Documentation

### Player System

**PlayerController** - Movement, dodging, aiming
- Touch/gamepad optimized controls
- Auto-aim for mobile
- Dodge with i-frames

**PlayerCombat** - Weapon management
- Auto-fire or manual modes
- Multi-weapon support (up to 6)
- Weapon cooldown management

**PlayerProgression** - Leveling and stats
- Experience and level-up system
- Stat scaling per level
- Level-up reward triggers

### Enemy AI System

**BaseEnemy** - Core AI with behavior tree pattern
- States: Idle, Chase, Attack, Flee
- Configurable via EnemyData ScriptableObjects
- Floor-based scaling

**Enemy Types:**
- **MeleeEnemy**: Charges and melee attacks
- **RangedEnemy**: Maintains distance, shoots projectiles
- **SpecialEnemy**: Unique abilities (teleport, summon, AOE, shield, heal)

### Weapon System

**BaseWeapon** - Core weapon functionality
- Cooldown management
- Upgrade system
- Level scaling

**Weapon Types:**
- **ProjectileWeapon**: Fires projectiles with pierce/spread
- **AOEWeapon**: Creates damage zones
- **BeamWeapon**: Instant line damage

### Ashmark System (Abilities)

**BaseAshmark** - Core ability functionality
- Active, Passive, and Triggered types
- Cooldown and stat modifier systems

**Ashmark Types:**
- **PassiveAshmark**: Permanent stat bonuses
- **ActiveAshmark**: Manually triggered abilities
- **TriggeredAshmark**: Auto-trigger on events (OnKill, OnHit, OnDodge, etc.)

### Procedural Generation

**DungeonGenerator** - Room-based dungeon generation
- Connects rooms using simple branching algorithm
- Room type distribution (Combat, Loot, Shop, Boss)
- Floor difficulty scaling

**Room** - Individual room controller
- Enemy spawning
- Room clear tracking
- Event triggers

### Loot & Progression

**LootManager** - Drop spawning and collection
- Weighted loot tables
- Auto-magnet system
- Despawn timers

**MetaProgression** - Persistent upgrades
- Multi-currency economy
- Stat upgrades (health, damage, speed, etc.)
- Cost scaling per upgrade level

### Narrative System

**NarrativeManager** - Story progression tracking
- Codex entry unlocking
- Echo moment triggers (in-run story beats)
- Dialogue queueing

**CodexEntryData** - Individual lore entries
- Categorized (Lore, Enemies, Weapons, etc.)
- Unlock conditions
- Rich text support

### Save System

**SaveManager** - JSON-based save/load
- Auto-save support
- Modular save data structure
- Persistent path storage

**Saved Data:**
- Meta progression (upgrades, currency)
- Narrative progress (unlocked codex, triggered echoes)
- Run statistics
- Settings (volume, graphics, etc.)

### Object Pooling

**ObjectPoolManager** - Performance-critical pooling system
- Pre-warming
- Expandable pools
- Max pool sizes
- IPoolable interface for lifecycle hooks

**Pooled Objects:**
- Projectiles
- Enemies (optional)
- VFX
- Loot pickups

### UI System

**GameHUD** - In-game interface
- Health/experience bars
- Ability cooldowns
- Currency display
- Timer
- FPS counter (debug)

**MainMenuUI** - Main menu navigation
**CodexUI** - Lore browser
**RunEndUI** - Victory/defeat screen with stats and rewards

## Mobile Optimization

### Performance Targets
- **60 FPS** on high-end devices
- **45-60 FPS** on mid-range devices
- **30 FPS** minimum on low-end devices
- **<2GB** total memory usage
- **No stuttering** during gameplay

### Optimization Techniques Used
- Object pooling for frequently spawned objects
- Sprite atlasing for reduced draw calls
- URP optimized rendering
- Compressed audio (Vorbis/ADPCM)
- Efficient collision matrix
- Lazy initialization where appropriate
- Event-driven updates (not polling)

### Mobile Controls
- Touch-based virtual joystick/buttons
- Auto-aim for easier targeting
- Larger UI elements for finger input
- Haptic feedback support
- Portrait or landscape orientations

## Expanding the Framework

### Adding New Content

**New Weapon:**
1. Create WeaponData ScriptableObject
2. (Optional) Create new weapon type class if behavior is unique
3. Create projectile prefab if needed
4. Register in object pool
5. Add to loot tables

**New Enemy:**
1. Create EnemyData ScriptableObject
2. Choose appropriate enemy script (Melee/Ranged/Special)
3. Create prefab with sprites/animations
4. Assign to room spawn lists
5. Balance stats

**New Ashmark:**
1. Create AshmarkData ScriptableObject
2. Choose type (Passive/Active/Triggered)
3. (Optional) Extend Ashmark classes for complex behaviors
4. Add to loot/shop
5. Test and balance

**New Room:**
1. Build room layout (Tilemap or prefabs)
2. Create RoomData ScriptableObject
3. Configure spawn points and enemy counts
4. Add to DungeonGenerator room pools
5. Test generation

**New Codex Entry:**
1. Create CodexEntryData ScriptableObject
2. Write lore text
3. Set unlock conditions
4. Link to gameplay events via GameEvents

### Extending Systems

The framework is designed for easy extension:

- **New Weapon Types**: Inherit from `BaseWeapon`
- **New Enemy Behaviors**: Inherit from `BaseEnemy`, override AI states
- **New Abilities**: Inherit from `BaseAshmark`
- **New UI Panels**: Follow existing UI pattern with event subscriptions
- **New Damage Types**: Add to `DamageType` enum, implement resistance system
- **New Currencies**: Add to `ProgressionCurrency` enum, update MetaProgression

## Analytics & Monetization

### Recommended Analytics Events

Track these for player retention insights:
- Run started (loadout, difficulty)
- Run ended (victory, time, floor, cause of death)
- Level up (level, time in run)
- Weapon/Ashmark equipped
- Boss defeated
- Codex unlocked
- Currency earned
- Upgrades purchased

### Ethical Monetization Options

If monetizing:
- **Cosmetic packs**: Character/weapon skins
- **Starter packs**: Optional convenience
- **Support packs**: "Buy the dev coffee" donations
- **Full unlock**: One-time purchase to unlock all content early

**Avoid**: Pay-to-win, loot boxes, energy systems, intrusive ads

## Known Limitations & Future Improvements

### Current Limitations
- Basic enemy AI (no advanced pathfinding)
- Simple room generation (no complex layouts)
- Limited animation system
- Basic VFX (needs particle systems)
- No multiplayer/co-op
- No replay system
- No cloud saves

### Recommended Improvements
1. **Advanced AI**: Add pathfinding (A*), formations, tactics
2. **Richer Procedural Generation**: Graph-based dungeon layouts, handcrafted segments
3. **Animation System**: Sprite animations, character customization
4. **VFX Polish**: Particle systems, screen shake, post-processing
5. **Audio System**: Adaptive music, spatial audio
6. **Achievements**: Integration with platform achievements
7. **Leaderboards**: Daily/weekly challenges
8. **Meta Story**: Hub world evolution based on progress
9. **Run Modifiers**: Challenge modes, mutations
10. **Character Classes**: Multiple playable characters with unique abilities

## Contributing

This framework is designed for solo developers but can be extended:

1. Fork the project
2. Create feature branches
3. Keep systems modular
4. Document new systems in code comments
5. Update this README with major changes
6. Test on mobile devices

## License

This framework is provided as a complete starting point for developers. Customize and extend as needed for your project.

## Credits

**Framework Design**: Unity 6 Roguelike Survivor Framework
**Architecture**: Event-driven, ScriptableObject-based
**Inspiration**: *Vampire Survivors*, *Hero Adventure: Survivor RPG*

## Additional Resources

- [Complete Setup Guide](SETUP_GUIDE.md) - Comprehensive step-by-step instructions
- Unity Input System: https://docs.unity3d.com/Packages/com.unity.inputsystem@latest
- URP Documentation: https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest
- Mobile Optimization: https://docs.unity.com/Manual/MobileOptimizationPracticalGuide.html

---

**Ready to build your roguelike survivor? Start with [SETUP_GUIDE.md](SETUP_GUIDE.md)!** 🎮
