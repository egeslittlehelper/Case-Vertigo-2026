# Wheel of Fortune — Vertigo Games Case Study

**Unity 6000.3.23f1** · Android (IL2CPP, ARM64)

A wheel-of-fortune gambling game inspired by Critical Strike's "Card Game" mode. Players spin a reward wheel across escalating zones, choosing when to cash out or risk losing everything to a bomb.

> **[Download APK](https://github.com/egeslittlehelper/Case-Vertigo-2026/releases/tag/Executable)**

## How to Play

1. Tap **SPIN** to spin the wheel. Each slice is either a reward or a bomb.
2. Rewards are added to your collection. The wheel advances to the next zone.
3. Every **5th zone** is a **Safe Zone** (silver wheel, no bomb). Every **30th zone** is a **Super Zone** (golden wheel, special rewards, no bomb).
4. During safe/super zones you can tap **EXIT** to cash out and keep all collected rewards.
5. Hit the bomb and you lose everything — or spend currency to **Revive** and keep going.
6. Rewards scale with zone progression: the deeper you go, the better they get.

## Screenshots

| 20:9 | 16:9 | 4:3 |
|:---:|:---:|:---:|
| ![20:9](Screenshots/20_9.png) | ![16:9](Screenshots/16_9.png) | ![4:3](Screenshots/4_3.png) |

## Architecture

Four assembly definitions enforce a strict dependency direction:

```
Core  (pure C#, no UnityEngine)
 └─ Data  (ScriptableObjects, config)
     └─ UI  (Views, Presenter, animations)
         └─ Infrastructure  (Composition Root, wallet persistence)
```

| Layer | Responsibility |
|---|---|
| **Core** | Domain types, game session state machine, spin resolver, zone rules, reward inventory |
| **Data** | ScriptableObjects (wheel configs, zone progression, economy settings, spin animation tuning) |
| **UI** | MVP views, DOTween animations, prefab builders, object pooling, hierarchy validator |
| **Infrastructure** | `GameInstaller` composition root, `PlayerPrefsWallet` |

## Design Patterns

| Pattern | Where | Why |
|---|---|---|
| **MVP** | GameSession (model), Views, GamePresenter | Separate domain logic from visuals |
| **Observer** | C# events on GameSession, views | Decoupled notifications |
| **Strategy** | IRandomProvider, IRewardScaling, IWheelProvider, IReviveCostPolicy | Swappable, testable behavior |
| **State Machine** | GameStateMachine (enum transitions) | Prevents illegal actions (double-tap, exit while spinning) |
| **Object Pool** | ObjectPool\<T\>, SliceViewFactory, zone cell recycling | Zero Instantiate/Destroy during play |
| **Factory** | SliceViewFactory, SO factory methods | Creators don't know concrete types |
| **Template Method** | PopupViewBase | Shared show/hide animation, popup-specific overrides |
| **Composition Root** | GameInstaller | All wiring in one place, no DI framework |
| **Data-Driven** | WheelConfigSO, ZoneProgressionSO, EconomySettingsSO | Designers change content without code |

## Key Decisions

- **Two-phase spin**: `BeginSpin()` resolves the result immediately (deterministic, testable); the animation is pure presentation.
- **Domain guards**: `GameStateMachine` prevents illegal state transitions regardless of UI state — the buttons are a convenience, the domain is the safeguard.
- **Deferred layout**: Zone bar uses `DOVirtual.DelayedCall(0f, ...)` to read dimensions after `AspectRatioFitter` resolves.
- **Manual DI**: Small project scope — a DI framework would be over-engineering. All dependencies are visible in one `Awake()`.
- **Escalating revive cost**: Strategy pattern (`IReviveCostPolicy`) — swap pricing logic without editing session code.

## Tests

128 EditMode unit tests covering core domain logic and pooling:

- **GameSession** — spin, zone advancement, bomb, cash out, revive, illegal state transitions
- **SpinResolver** — weighted random selection, deterministic seeding
- **ZoneRules** — safe/super zone detection, zone type classification
- **RewardInventory** — add, merge, clear, stack consolidation
- **ReviveCost** — escalating cost calculation, policy behavior
- **ObjectPool** — get, release, grow, reuse, parent assignment
- **WheelAngleCalculator, NumberFormatter, CurveRewardScaling** — math and formatting utilities

Run via Unity Test Runner (EditMode) or command line:
```
Unity.exe -batchmode -nographics -projectPath . -runTests -testPlatform EditMode -testResults TestResults/editmode.xml
```

## Folder Structure

```
Assets/_Project/
├── Art/Sprites/          UI sprite assets
│   └── Atlases/          Sprite Atlas (ui_atlas_main)
├── Data/                 ScriptableObject instances (wheels, zones, economy)
├── Prefabs/UI/           UI prefabs (wheel, popups, panels)
├── Scenes/               Game scene
├── Scripts/
│   ├── Core/             Domain logic (no Unity dependency)
│   ├── Data/             SO definitions, config types
│   ├── UI/               Views, presenter, animations, pooling
│   ├── Infrastructure/   Composition root, persistence
│   └── Editor/           Prefab builders, hierarchy validator, debug tools
└── Tests/EditMode/       Unit tests (Core + pool)
```

## UI Rules (per brief)

- Canvas Scaler: **Scale With Screen Size**, Match Mode **Expand**
- All text uses **TextMeshPro**
- Dynamic UI element names end with `_value`
- Naming convention: `ui_type_context_detail` (e.g. `ui_image_wheel_base_value`)
- **No** unnecessary `raycastTarget` or `maskable`
- Animators on dedicated `ui_anim_*` child transforms, never on root
- Button references auto-assigned via `OnValidate` — no editor OnClick wiring
- Sliced sprites on all button/frame Image components
- No stretched images — `preserveAspect` where needed
- Supports **20:9**, **16:9**, and **4:3** aspect ratios

## Build & Run

1. Open in **Unity 6000.3.23f1**
2. Switch platform to **Android** (IL2CPP, ARM64)
3. Open `Assets/_Project/Scenes/New Scene.unity`
4. **VertigoCase > Validate UI Hierarchy** (should report 0 errors)
5. Play in editor or **Build And Run** for APK

> Prefab builder scripts are under `VertigoCase > Build Prefabs` — they are development tools for regenerating prefabs. You do not need to run them to open or play the project.

## Known Limitations

- Wallet balance persists via `PlayerPrefs` — a production game would use server-side storage.
- No audio — the brief focuses on visual polish and game mechanics.
- The revive system is a bonus feature; pricing is tunable via `EconomySettingsSO` but not balanced for a live economy.

## Third-Party

- [DOTween](http://dotween.demigiant.com/) — tween engine for all UI animations
