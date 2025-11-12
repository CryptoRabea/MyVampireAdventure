# Implementation Checklist

Use this checklist to track your progress implementing the framework.

## Phase 1: Initial Setup (2-4 hours)

### Unity Configuration
- [ ] Create new Unity 6 project (URP template)
- [ ] Import required packages (Input System, Cinemachine, TextMeshPro)
- [ ] Configure project settings (Player, Graphics, Quality)
- [ ] Setup layers and tags
- [ ] Configure Physics2D collision matrix
- [ ] Create URP asset and 2D Renderer

### Input System
- [ ] Create Input Actions asset (PlayerInputActions)
- [ ] Configure Player action map (Move, Aim, Dodge, Abilities)
- [ ] Configure UI action map (Navigate, Submit, Cancel, Pause)
- [ ] Generate C# class from Input Actions
- [ ] Test input in empty scene

## Phase 2: Core Systems (4-6 hours)

### Game Manager
- [ ] Create GameManager GameObject (DontDestroyOnLoad)
- [ ] Add GameManager script
- [ ] Add ObjectPoolManager
- [ ] Add SaveManager
- [ ] Configure scene names in inspector

### Player Setup
- [ ] Create player sprite (placeholder 32x32)
- [ ] Create player prefab with all components
  - [ ] PlayerController
  - [ ] PlayerCombat
  - [ ] PlayerProgression
  - [ ] Health
  - [ ] AshmarkManager
  - [ ] PlayerInput
- [ ] Configure PlayerController settings
- [ ] Setup PlayerInput component
- [ ] Test player movement

### Weapon System
- [ ] Create basic weapon ScriptableObject (pistol)
- [ ] Create projectile sprite
- [ ] Create projectile prefab with Projectile script
- [ ] Register projectile in ObjectPoolManager
- [ ] Assign weapon to PlayerCombat
- [ ] Test weapon firing

### Enemy System
- [ ] Create enemy sprite (placeholder 32x32)
- [ ] Create EnemyData ScriptableObject (zombie)
- [ ] Create enemy prefab (MeleeEnemy)
- [ ] Test enemy AI (chase, attack)
- [ ] Test enemy death and experience gain

## Phase 3: Combat & Gameplay (3-4 hours)

### Combat Mechanics
- [ ] Test player damage to enemies
- [ ] Test enemy damage to player
- [ ] Verify health bars update
- [ ] Test experience gain and level-up
- [ ] Verify death states work

### Object Pooling
- [ ] Verify projectiles pool correctly
- [ ] Test with 50+ projectiles on screen
- [ ] Check for memory leaks
- [ ] Verify pool stats in inspector

### Additional Weapons
- [ ] Create 2 more weapon types (different behaviors)
- [ ] Test weapon switching
- [ ] Balance weapon stats

### Additional Enemies
- [ ] Create RangedEnemy prefab and data
- [ ] Create SpecialEnemy prefab and data
- [ ] Test different enemy types together
- [ ] Balance enemy stats

## Phase 4: Procedural Generation (3-4 hours)

### Room System
- [ ] Create room floor/wall sprites
- [ ] Create basic room prefab (20x20)
- [ ] Create RoomData ScriptableObject
- [ ] Configure enemy spawn points in room

### Dungeon Generation
- [ ] Add DungeonGenerator to Run scene
- [ ] Configure room pools (Combat, Loot, Boss)
- [ ] Test dungeon generation
- [ ] Verify room transitions
- [ ] Test enemy spawning in rooms

### Room Types
- [ ] Create Combat room variant
- [ ] Create Loot room variant
- [ ] Create Shop room (basic)
- [ ] Create Boss room
- [ ] Test room type distribution

## Phase 5: Progression & Loot (2-3 hours)

### Loot System
- [ ] Create loot pickup prefabs
- [ ] Configure LootManager
- [ ] Setup loot tables (common, rare, epic)
- [ ] Test loot drops from enemies
- [ ] Test loot magnet/collection

### Meta Progression
- [ ] Configure MetaProgression upgrades
- [ ] Test upgrade purchasing
- [ ] Test currency persistence
- [ ] Verify upgrade effects apply

### Ashmarks (Abilities)
- [ ] Create 3 Ashmark ScriptableObjects (each type)
  - [ ] Passive (stat bonus)
  - [ ] Active (manual ability)
  - [ ] Triggered (OnKill effect)
- [ ] Test Ashmark equipping
- [ ] Test Ashmark activation
- [ ] Test Ashmark UI display

## Phase 6: UI Implementation (4-5 hours)

### Game HUD
- [ ] Create Canvas with GameHUD script
- [ ] Setup health bar
- [ ] Setup experience bar
- [ ] Setup ability slots (x4)
- [ ] Setup currency display
- [ ] Setup timer
- [ ] Test HUD updates during gameplay

### Main Menu
- [ ] Create MainMenu scene
- [ ] Add MainMenuUI script and UI elements
- [ ] Setup buttons (Start, Continue, Codex, Settings, Quit)
- [ ] Test scene transitions
- [ ] Test continue button enable/disable

### Run End Screen
- [ ] Create RunEndUI (inactive by default)
- [ ] Setup victory/defeat panels
- [ ] Display run stats
- [ ] Display rewards
- [ ] Test victory flow
- [ ] Test defeat flow

### Codex UI
- [ ] Create CodexUI panel
- [ ] Setup entry list
- [ ] Setup detail view
- [ ] Test entry display
- [ ] Test category filtering

## Phase 7: Narrative System (2-3 hours)

### Codex Entries
- [ ] Create 5-10 CodexEntryData assets
- [ ] Write lore text for each
- [ ] Set unlock conditions
- [ ] Test codex unlocking during gameplay

### Echo Moments
- [ ] Create 3-5 Echo Moment data entries
- [ ] Configure floor triggers
- [ ] Test echo triggering
- [ ] Link echoes to codex unlocks

### Hub World (Optional)
- [ ] Create Hub scene
- [ ] Add NPCs (placeholder objects)
- [ ] Add shop/upgrade stations
- [ ] Test hub navigation

## Phase 8: Save System (1-2 hours)

### Save Implementation
- [ ] Test save game functionality
- [ ] Test load game functionality
- [ ] Test auto-save
- [ ] Verify meta progression saves
- [ ] Verify codex progress saves
- [ ] Verify settings save
- [ ] Test save file corruption handling

### Data Persistence
- [ ] Test save after app close
- [ ] Test save on device (Android/iOS)
- [ ] Verify save file location
- [ ] Test delete save function

## Phase 9: Mobile Optimization (3-5 hours)

### Performance
- [ ] Setup object pooling for all frequent spawns
- [ ] Create sprite atlases (Player, Enemies, UI)
- [ ] Optimize audio import settings
- [ ] Profile on target device
- [ ] Verify 60 FPS with 20+ enemies
- [ ] Check memory usage < 2GB

### Touch Controls
- [ ] Add virtual joystick (On-Screen Controls package)
- [ ] Add virtual buttons (Dodge, Abilities)
- [ ] Test touch responsiveness
- [ ] Add haptic feedback
- [ ] Test on actual device

### Mobile Build
- [ ] Configure Android build settings
- [ ] Configure iOS build settings
- [ ] Test build and deploy to device
- [ ] Test performance on device
- [ ] Test battery usage

## Phase 10: Polish & Balance (4-6 hours)

### Visual Effects
- [ ] Add muzzle flash VFX
- [ ] Add hit effect VFX
- [ ] Add death effect VFX
- [ ] Add level-up effect
- [ ] Test VFX pooling

### Audio
- [ ] Import background music
- [ ] Import weapon SFX
- [ ] Import enemy SFX
- [ ] Import UI SFX
- [ ] Setup audio mixer
- [ ] Test volume controls

### Game Balance
- [ ] Balance player health
- [ ] Balance weapon damage
- [ ] Balance enemy health/damage
- [ ] Balance experience curve
- [ ] Balance meta upgrade costs
- [ ] Playtest for 30+ minutes
- [ ] Adjust difficulty curve

### UI Polish
- [ ] Add button hover effects
- [ ] Add transition animations
- [ ] Add screen fade transitions
- [ ] Polish HUD layout
- [ ] Test on different screen sizes

## Phase 11: Content Creation (Variable time)

### Weapons
- [ ] Create 10+ unique weapons
- [ ] Create weapon icons
- [ ] Write weapon descriptions
- [ ] Balance each weapon
- [ ] Create weapon VFX

### Enemies
- [ ] Create 15+ enemy types
- [ ] Create enemy sprites/animations
- [ ] Write enemy lore
- [ ] Balance enemy stats
- [ ] Create enemy VFX

### Ashmarks
- [ ] Create 20+ Ashmarks
- [ ] Create Ashmark icons
- [ ] Write Ashmark descriptions
- [ ] Balance Ashmark effects
- [ ] Test Ashmark combinations

### Rooms
- [ ] Create 10+ combat room variants
- [ ] Create 5+ loot room variants
- [ ] Create 3+ boss room variants
- [ ] Create room art/decorations
- [ ] Test room variety

### Narrative
- [ ] Write complete codex (30+ entries)
- [ ] Write echo moments for all floors
- [ ] Write NPC dialogues
- [ ] Test narrative pacing
- [ ] Verify story coherence

## Phase 12: Testing & QA (2-3 hours)

### Functional Testing
- [ ] Test all weapons
- [ ] Test all enemies
- [ ] Test all Ashmarks
- [ ] Test all rooms
- [ ] Test procedural generation
- [ ] Test save/load
- [ ] Test all UI screens
- [ ] Test all menus

### Performance Testing
- [ ] Profile CPU usage
- [ ] Profile GPU usage
- [ ] Profile memory usage
- [ ] Test on low-end device
- [ ] Test on high-end device
- [ ] Fix any performance issues

### Gameplay Testing
- [ ] Full playthrough (30+ minutes)
- [ ] Test death scenarios
- [ ] Test victory scenarios
- [ ] Test edge cases
- [ ] Balance check
- [ ] Get external playtester feedback

### Mobile Testing
- [ ] Test on Android
- [ ] Test on iOS
- [ ] Test on tablet
- [ ] Test touch controls thoroughly
- [ ] Test app backgrounding
- [ ] Test memory warnings
- [ ] Test different screen sizes

## Phase 13: Release Preparation

### Final Polish
- [ ] Fix all known bugs
- [ ] Implement critical feedback
- [ ] Final balance pass
- [ ] Optimize final build size
- [ ] Add analytics (optional)
- [ ] Add monetization (optional)

### Marketing Assets
- [ ] Create app icon
- [ ] Create screenshots (5+)
- [ ] Create trailer video
- [ ] Write store description
- [ ] Prepare press kit

### Store Submission
- [ ] Setup Google Play Console
- [ ] Setup Apple App Store Connect
- [ ] Upload builds
- [ ] Fill store listings
- [ ] Submit for review

---

## Estimated Total Time: 40-60 hours

This assumes solo development with placeholder art. Professional art and additional content will extend the timeline significantly.

## Priority Levels

**P0 (Must-Have for MVP):**
- Player movement and combat
- Basic enemy AI (1 type minimum)
- 1 weapon
- Basic procedural generation (5 rooms)
- HUD
- Save system

**P1 (Important for 1.0):**
- 3 enemy types
- 5 weapons
- 5 Ashmarks
- Meta progression
- Full UI suite
- 10 codex entries

**P2 (Nice-to-Have):**
- Additional content (15+ weapons, 20+ enemies)
- Full narrative implementation
- Hub world
- Advanced VFX
- Analytics

**P3 (Post-Launch):**
- New game modes
- Additional floors/biomes
- Multiplayer/co-op
- Leaderboards
- Seasonal content

---

**Track your progress and adjust timeline as needed. Good luck!** 🚀
