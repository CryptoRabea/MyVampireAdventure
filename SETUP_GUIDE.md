# Unity 6 Roguelike Survivor Shooter - Complete Setup Guide

## Table of Contents
1. [Project Setup](#project-setup)
2. [Unity Configuration](#unity-configuration)
3. [Input System Setup](#input-system-setup)
4. [Creating ScriptableObjects](#creating-scriptableobjects)
5. [Scene Setup](#scene-setup)
6. [Prefab Creation](#prefab-creation)
7. [Mobile Optimization](#mobile-optimization)
8. [First Vertical Slice](#first-vertical-slice)
9. [Expanding Content](#expanding-content)

---

## Project Setup

### 1. Unity Version
- **Required:** Unity 6 (6000.0.0f1 or later)
- **Platform:** Android/iOS (Mobile)
- **Rendering:** URP (Universal Render Pipeline)

### 2. Required Packages
Install these via Package Manager (Window > Package Manager):

```
- Universal RP (com.unity.render-pipelines.universal) - 17.0+
- Input System (com.unity.inputsystem) - 1.7+
- Cinemachine (com.unity.cinemachine) - 3.0+
- TextMeshPro (com.unity.textmeshpro) - 3.2+
- Addressables (com.unity.addressables) - Optional, for advanced asset management
- Mobile Notifications (com.unity.mobile.notifications) - Optional
- Analytics (com.unity.services.analytics) - Optional for player metrics
```

### 3. Project Settings

#### Graphics Settings
1. Go to Edit > Project Settings > Graphics
2. Set Scriptable Render Pipeline Settings to URP asset
3. Create URP asset: Right-click in Project > Create > Rendering > URP Asset (with 2D Renderer)

#### Quality Settings
1. Edit > Project Settings > Quality
2. Create 3 quality levels: Low, Medium, High
3. Set default for mobile: Medium

#### Player Settings (Mobile)
1. Edit > Project Settings > Player
2. **Android:**
   - Minimum API Level: 24 (Android 7.0)
   - Target API Level: 33 (Android 13)
   - Scripting Backend: IL2CPP
   - Target Architectures: ARM64 (disable ARMv7)
   - Graphics API: Vulkan (primary), OpenGLES3 (fallback)
3. **iOS:**
   - Target SDK: Device SDK
   - Target Minimum iOS Version: 13.0
   - Architecture: ARM64
   - Graphics API: Metal

#### Input System
1. Edit > Project Settings > Player > Active Input Handling
2. Select "Both" (for backward compatibility) or "Input System Package (New)"
3. Restart Unity when prompted

---

## Unity Configuration

### 1. Folder Structure
Your project should have this structure (already created by scripts):

```
Assets/
├── Scenes/
│   ├── MainMenu.unity
│   ├── Hub.unity
│   └── Run.unity
├── Scripts/
│   ├── Core/
│   ├── Player/
│   ├── Enemies/
│   ├── Weapons/
│   ├── Ashmarks/
│   ├── Procedural/
│   ├── Loot/
│   ├── Narrative/
│   ├── Save/
│   ├── UI/
│   ├── Audio/
│   ├── Pooling/
│   └── Utils/
├── Prefabs/
│   ├── Player/
│   ├── Enemies/
│   ├── Weapons/
│   ├── Projectiles/
│   ├── Rooms/
│   ├── UI/
│   ├── VFX/
│   └── Loot/
├── ScriptableObjects/
│   ├── Weapons/
│   ├── Enemies/
│   ├── Ashmarks/
│   ├── Rooms/
│   ├── Encounters/
│   ├── Narrative/
│   └── Progression/
├── Sprites/
│   ├── UI/
│   ├── Player/
│   ├── Enemies/
│   ├── Environment/
│   └── VFX/
├── Materials/
├── Audio/
│   ├── Music/
│   └── SFX/
├── Animations/
└── Settings/
    └── URP/
```

### 2. Layer Setup
Go to Edit > Project Settings > Tags and Layers

**Layers:**
- Default (0)
- Player (6)
- Enemies (7)
- Projectiles (8)
- Obstacles (9)
- Loot (10)
- UI (5)

**Tags:**
- Player
- Enemy
- Projectile
- Loot
- Boss

### 3. Physics 2D Settings
Edit > Project Settings > Physics 2D

**Layer Collision Matrix:**
- Player collides with: Enemies, Obstacles, Loot
- Enemies collide with: Player, Obstacles, Enemies
- Projectiles collide with: Enemies (player projectiles), Player (enemy projectiles), Obstacles
- Loot collides with: Player only

---

## Input System Setup

### 1. Create Input Actions Asset
1. Right-click in Project > Create > Input Actions
2. Name it "PlayerInputActions"
3. Double-click to open Input Actions editor

### 2. Configure Action Maps

#### Player Action Map
```
Action Map: "Player"

Actions:
├── Move
│   ├── Action Type: Value
│   ├── Control Type: Vector2
│   └── Bindings:
│       ├── Gamepad: Left Stick
│       ├── Keyboard: WASD (2D Vector Composite)
│       └── Touch: On-Screen Stick (requires On-Screen Controls)
│
├── Aim
│   ├── Action Type: Value
│   ├── Control Type: Vector2
│   └── Bindings:
│       ├── Gamepad: Right Stick
│       └── Mouse: Delta (optional for PC testing)
│
├── Dodge
│   ├── Action Type: Button
│   └── Bindings:
│       ├── Gamepad: B Button (Xbox) / Circle (PS)
│       ├── Keyboard: Space
│       └── Touch: On-Screen Button
│
├── Ability1-4
│   ├── Action Type: Button
│   └── Bindings:
│       ├── Gamepad: X, Y, LB, RB
│       ├── Keyboard: Q, E, R, F
│       └── Touch: On-Screen Buttons
```

#### UI Action Map
```
Action Map: "UI"

Actions:
├── Navigate (Vector2)
├── Submit (Button)
├── Cancel (Button)
└── Pause (Button)
```

3. Click "Generate C# Class" button
4. Save the generated script to Assets/Scripts/Utils/

### 3. Setup Player Input Component
On Player GameObject:
1. Add "Player Input" component
2. Assign PlayerInputActions asset
3. Default Action Map: "Player"
4. Behavior: "Invoke Unity Events" or "Send Messages"

---

## Creating ScriptableObjects

### 1. Example Weapon: Basic Pistol

Right-click in Project > Create > Vampire Survivor > Weapon Data

```
Name: Weapon_BasicPistol
Settings:
- Weapon Name: "Basic Pistol"
- Description: "A reliable sidearm"
- Weapon Type: Projectile
- Damage: 10
- Cooldown: 0.5
- Projectile Speed: 15
- Range: 20
- Pierce Count: 0
- Projectile Count: 1
- Max Upgrade Level: 5
- Damage Per Level: 3
- Cooldown Reduction Per Level: 0.05
```

### 2. Example Enemy: Zombie

Right-click in Project > Create > Vampire Survivor > Enemy Data

```
Name: Enemy_Zombie
Settings:
- Enemy Name: "Zombie"
- Description: "Slow but relentless undead"
- Enemy Type: Melee
- Max Health: 50
- Move Speed: 2
- Damage: 15
- Attack Range: 1.5
- Attack Cooldown: 1.5
- Detection Range: 10
- Experience Value: 10
- Souls Currency: 2
- Loot Drop Chance: 0.1
- Health Scaling Per Floor: 10
- Damage Scaling Per Floor: 3
```

### 3. Example Ashmark: Vampiric Touch

Right-click in Project > Create > Vampire Survivor > Ashmark Data

```
Name: Ashmark_VampiricTouch
Settings:
- Ashmark Name: "Vampiric Touch"
- Description: "Heal when you kill enemies"
- Type: Triggered
- Rarity: Uncommon
- Trigger Type: OnKill
- Trigger Chance: 1.0 (100%)
- Ability Damage: 10 (heal amount)
- Cooldown: 0.5
```

### 4. Example Room: Combat Room 1

Right-click in Project > Create > Vampire Survivor > Room Data

```
Name: Room_Combat01
Settings:
- Room Name: "Abandoned Hall"
- Room Type: Combat
- Grid Size: 10x10
- World Size: 20x20
- Has Doors: All directions enabled
- Min Enemies: 5
- Max Enemies: 10
- Difficulty Rating: 2
- Minimum Floor: 1
```

### 5. Example Codex Entry

Right-click in Project > Create > Vampire Survivor > Codex Entry

```
Name: Codex_AncientCurse
Settings:
- Entry ID: "codex_curse_001"
- Title: "The Ancient Curse"
- Category: Lore
- Entry Text: "Long ago, a dark ritual transformed the population..."
- Unlock Condition: "Complete Floor 1"
```

---

## Scene Setup

### Scene 1: MainMenu

1. Create new scene: MainMenu
2. Add Canvas (UI > Canvas)
   - Canvas Scaler: Scale with Screen Size
   - Reference Resolution: 1920x1080
   - Match: 0.5 (balance width/height)
3. Add EventSystem
4. Create UI hierarchy:
   ```
   Canvas
   ├── MainMenuUI (add MainMenuUI script)
   │   ├── MainPanel
   │   │   ├── TitleText
   │   │   ├── StartButton
   │   │   ├── ContinueButton
   │   │   ├── CodexButton
   │   │   ├── SettingsButton
   │   │   └── QuitButton
   │   ├── SettingsPanel (initially inactive)
   │   └── CodexPanel (initially inactive)
   ```
5. Create empty GameObject: "GameManager"
   - Add GameManager script
   - Set Scene Names in inspector

### Scene 2: Hub

1. Create new scene: Hub
2. Add 2D Camera
   - Size: 10
   - Background: Solid Color
3. Add Cinemachine Virtual Camera
   - Follow: Player
   - Dead Zone: 0.1, 0.1
   - Soft Zone: 0.8, 0.8
4. Create Hub environment (placeholder sprites)
5. Add NPCs (placeholder GameObjects)
6. Add UI Canvas with HubUI script

### Scene 3: Run

1. Create new scene: Run
2. Add 2D Camera + Cinemachine
3. Create empty GameObject: "RunManager"
   - Add DungeonGenerator script
   - Add LootManager script
4. Create UI Canvas
   - Add GameHUD script
   - Add RunEndUI script (initially inactive)
5. Leave room for procedurally generated content

---

## Prefab Creation

### 1. Player Prefab

Create Player GameObject:
```
Player (Tag: Player, Layer: Player)
├── Sprite (SpriteRenderer)
├── Collider (CircleCollider2D or CapsuleCollider2D)
├── Rigidbody2D (Gravity Scale: 0)
└── Components:
    ├── PlayerController
    ├── PlayerCombat
    ├── PlayerProgression
    ├── Health
    ├── AshmarkManager
    └── PlayerInput
```

Settings:
- Position: (0, 0, 0)
- Collider: Radius 0.5
- Rigidbody2D: Freeze Rotation Z

### 2. Projectile Prefab

Create Projectile GameObject:
```
Projectile (Layer: Projectiles)
├── Sprite (SpriteRenderer)
├── Collider (CircleCollider2D, Trigger)
├── Rigidbody2D (Gravity Scale: 0)
├── TrailRenderer (optional)
└── Components:
    └── Projectile
```

### 3. Enemy Prefab (Zombie)

Create Enemy GameObject:
```
Enemy_Zombie (Tag: Enemy, Layer: Enemies)
├── Sprite (SpriteRenderer)
├── Collider (CircleCollider2D)
├── Rigidbody2D (Gravity Scale: 0)
└── Components:
    ├── MeleeEnemy
    ├── Health
    └── Animator (optional)
```

Assign EnemyData ScriptableObject in inspector

### 4. Room Prefab Template

Create Room GameObject:
```
Room_Template
├── Floor (Sprite)
├── Walls (multiple sprites or tilemap)
├── Doors
│   ├── NorthDoor
│   ├── SouthDoor
│   ├── EastDoor
│   └── WestDoor
└── EnemySpawnPoints (empty GameObjects)
```

### 5. Loot Prefab

Create Loot GameObject:
```
Loot_Currency
├── Sprite (SpriteRenderer)
├── Collider (CircleCollider2D, Trigger)
└── Component:
    └── LootPickup or CurrencyPickup
```

---

## Mobile Optimization

### 1. URP Settings for Mobile

Create URP Renderer Asset:
1. Right-click > Create > Rendering > URP Renderer
2. Settings:
   - Rendering Path: Forward
   - Depth Priming: Auto
   - Opaque Layer Mask: Everything
   - Transparent Layer Mask: Everything

URP Asset settings:
- Quality: Medium
- HDR: Disabled
- MSAA: Disabled (use FXAA instead)
- Render Scale: 0.8 - 1.0
- Shadow Distance: 20
- Cascade Count: 1

### 2. Object Pooling

All frequently spawned objects MUST use pooling:
- Projectiles
- Enemies
- VFX
- Loot pickups

Register pools in ObjectPoolManager:
```csharp
poolManager.RegisterPool("Projectile_Basic", projectilePrefab, 50, 200);
poolManager.RegisterPool("Enemy_Zombie", zombiePrefab, 20, 100);
poolManager.RegisterPool("VFX_Hit", hitEffectPrefab, 30, 100);
```

### 3. Sprite Atlases

1. Install: Window > Package Manager > 2D Sprite
2. Create Sprite Atlas: Right-click > Create > 2D > Sprite Atlas
3. Add sprites by folder:
   - Atlas_Player: All player sprites
   - Atlas_Enemies: All enemy sprites
   - Atlas_UI: All UI sprites
4. Settings:
   - Max Texture Size: 2048
   - Format: ASTC (Android), PVRTC (iOS)
   - Compression: High Quality

### 4. Audio Settings

Audio Clip Import Settings:
- Music: Compressed, Vorbis, Quality 70%
- SFX: Compressed, ADPCM, Force to Mono

### 5. Performance Targets

**Target FPS:**
- High-end mobile: 60 FPS
- Mid-range mobile: 45-60 FPS
- Low-end mobile: 30 FPS

**Memory Budget:**
- Total: < 2GB
- Texture Memory: < 500MB
- Audio Memory: < 100MB

---

## First Vertical Slice

### Goal: Playable 5-Minute Demo

#### Step 1: Create Minimal Assets (1-2 hours)

**Sprites (use placeholder colored squares initially):**
- Player: 32x32 green square
- Enemy: 32x32 red circle
- Projectile: 8x8 yellow circle
- Floor: 512x512 gray tile
- Wall: 64x64 dark gray

**Audio:**
- Use free SFX from Freesound.org or Unity Asset Store
- 1 background music track
- 3 SFX (shoot, hit, death)

#### Step 2: Setup Core Systems (2-3 hours)

1. **Create GameManager GameObject:**
   - Attach GameManager script
   - Attach ObjectPoolManager script
   - Attach SaveManager script
   - Set scene names

2. **Create Player:**
   - Drag sprites onto Player prefab
   - Configure PlayerController settings:
     - Move Speed: 5
     - Dodge Distance: 3
     - Auto Aim: Enabled
   - Create one weapon ScriptableObject
   - Assign to PlayerCombat starting weapons

3. **Create Enemy:**
   - Setup Zombie enemy prefab
   - Create Enemy_Zombie ScriptableObject
   - Place 3-5 enemies in test scene

4. **Setup Pooling:**
   - Create projectile prefab
   - Register in PoolManager
   - Test firing

#### Step 3: Create Test Room (1 hour)

1. Create Room_Test:
   - 20x20 floor
   - Walls on edges
   - Place 5-8 enemies
   - Add player spawn point

2. Test gameplay loop:
   - Movement
   - Shooting
   - Enemy AI
   - Damage/death
   - Object pooling performance

#### Step 4: Add Basic UI (1 hour)

1. Create HUD:
   - Health bar
   - Experience bar
   - Level display
   - Timer

2. Test run:
   - Kill enemies
   - Gain experience
   - Level up
   - Die/win

#### Step 5: Polish (1 hour)

1. Add simple VFX:
   - Muzzle flash (colored sprite)
   - Hit effect (particle system)
   - Death effect

2. Add audio:
   - Background music
   - Weapon fire sound
   - Hit sound
   - Death sound

3. Balance:
   - Enemy health/damage
   - Player health
   - Experience curve
   - Weapon damage

**Result:** 5-10 minute playable loop with core mechanics

---

## Expanding Content

### Adding New Weapon

1. Create weapon ScriptableObject
2. Create projectile prefab (if needed)
3. Register projectile in PoolManager
4. Add weapon icon to UI
5. Test and balance

### Adding New Enemy Type

1. Create EnemyData ScriptableObject
2. Choose enemy script: MeleeEnemy, RangedEnemy, or SpecialEnemy
3. Create enemy prefab
4. Assign to room spawn pools
5. Balance: health, damage, rewards

### Adding New Ashmark

1. Create AshmarkData ScriptableObject
2. Choose type: Passive, Active, or Triggered
3. Create icon
4. Test effect
5. Add to loot pools or shop

### Adding New Room

1. Design room layout (use Tilemap or prefabs)
2. Create RoomData ScriptableObject
3. Configure enemy spawns
4. Add to DungeonGenerator room pools
5. Test generation and enemy placement

### Adding Narrative Content

1. Create CodexEntryData ScriptableObject
2. Write lore text
3. Set unlock conditions
4. Create echo moments (optional)
5. Link to gameplay events

---

## Advanced Features

### Analytics Integration

Track player retention:
```csharp
// Events to track:
- RunStarted (difficulty, loadout)
- RunEnded (result, time, floor reached)
- PlayerDeath (cause, floor, time)
- LevelUp (level, time)
- WeaponEquipped (weapon, time)
- AshmarkEquipped (ashmark, time)
- BossDefeated (boss, time, attempts)
- CodexUnlocked (entry, trigger)
```

### Monetization (Optional)

**Ethical approaches:**
1. **Cosmetics:** Skin packs for player/weapons
2. **Convenience:** Extra save slots, starter pack
3. **Support:** "Buy the devs coffee" donation
4. **NO:** Pay-to-win, loot boxes, predatory mechanics

### Addressables (Advanced)

For larger projects, use Addressables for:
- Weapons (loaded dynamically)
- Enemy prefabs
- Room prefabs
- Audio clips
- Codex entries

Benefits:
- Smaller initial download
- Faster load times
- Easy content updates

---

## Testing Checklist

### Performance Testing
- [ ] Maintain 60 FPS with 20+ enemies on screen
- [ ] No memory leaks after 10+ minute run
- [ ] Smooth projectile pooling (100+ active)
- [ ] UI responsive on all screen sizes

### Gameplay Testing
- [ ] All weapons feel distinct
- [ ] Enemy types require different strategies
- [ ] Progression feels rewarding
- [ ] Death feels fair
- [ ] Runs take 10-20 minutes

### Mobile Testing
- [ ] Touch controls responsive
- [ ] No overheating after 30 minutes
- [ ] Battery drain acceptable
- [ ] Works on low-end devices (2GB RAM)
- [ ] Portrait and landscape support (if needed)

### Save System Testing
- [ ] Save persists after app close
- [ ] Meta progression works
- [ ] Codex unlocks save
- [ ] No save corruption

### Narrative Testing
- [ ] Codex unlocks are clear
- [ ] Echo moments trigger correctly
- [ ] Story motivates completion
- [ ] Lore is interesting and coherent

---

## Troubleshooting

### "Scripts don't compile"
- Check for missing usings
- Verify all namespaces are correct
- Rebuild solution in IDE

### "Object pooling not working"
- Ensure prefabs are registered in PoolManager
- Check pool names match exactly
- Verify IPoolable implementation

### "Input not working"
- Check Player Input component is assigned
- Verify Input Actions asset generated C# class
- Check Action Map is set to "Player"

### "Player can't shoot"
- Verify weapon ScriptableObject is assigned
- Check firePoint transform exists
- Ensure projectile prefab is registered in pool

### "Enemies don't move"
- Check Rigidbody2D gravity scale is 0
- Verify player has "Player" tag
- Check enemy layer collision matrix

### "Save not working"
- Verify Application.persistentDataPath is writable
- Check for JSON serialization errors in console
- Ensure SaveManager is in scene

---

## Next Steps

1. **Art Pass:** Replace placeholder sprites with actual art
2. **Audio Pass:** Add music, ambient sounds, voice lines
3. **Content Expansion:** Add more weapons, enemies, Ashmarks
4. **Meta Progression:** Expand upgrade tree
5. **Narrative:** Write complete story, all codex entries
6. **Polish:** VFX, screen shake, particles, UI animations
7. **Playtesting:** Gather feedback, balance, iterate
8. **Optimization:** Profile, optimize hotspots
9. **Marketing:** Trailer, screenshots, press kit
10. **Launch:** Submit to stores, celebrate! 🎉

---

## Resources

- Unity Input System Documentation: https://docs.unity3d.com/Packages/com.unity.inputsystem@latest
- URP Documentation: https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest
- Cinemachine Documentation: https://docs.unity3d.com/Packages/com.unity.cinemachine@latest
- Mobile Optimization Guide: https://docs.unity.com/Manual/MobileOptimizationPracticalGuide.html

---

## Support

For questions or issues with this framework:
- Review this guide thoroughly
- Check Unity console for errors
- Use Unity forums for Unity-specific questions
- Test on actual mobile devices early and often

Good luck with your roguelike survivor shooter! 🎮
