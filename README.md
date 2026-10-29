# Wheel of Fortune — Vertigo Games Case Study

**Unity 6000.3.23f1** · Android (IL2CPP, ARM64)

A wheel-of-fortune gambling game inspired by Critical Strike's "Card Game" mode. Players spin a reward wheel across escalating zones, choosing when to cash out or risk losing everything to a bomb.

## Gameplay

- Spin the wheel to collect rewards that scale with zone progression.
- Every **5th zone** is a **Safe Zone** — risk-free silver spin, no bomb.
- Every **30th zone** is a **Super Zone** — risk-free golden spin with special rewards.
- All other zones contain a **bomb slice** — hitting it loses all collected rewards.
- Players can **exit and collect** rewards during safe/super zones (while idle).
- **Revive system** (bonus): spend in-game currency to keep rewards after a bomb hit, with escalating cost per revive.

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

## Folder Structure

```
Assets/_Project/
├── Art/Sprites/          UI sprite assets
├── Data/                 ScriptableObject instances (wheels, zones, economy)
├── Prefabs/UI/           Generated UI prefabs (wheel, popups, panels)
├── Scenes/               Game scene
├── Scripts/
│   ├── Core/             Domain logic (no Unity dependency)
│   ├── Data/             SO definitions, config types
│   ├── UI/               Views, presenter, animations, pooling
│   ├── Infrastructure/   Composition root, persistence
│   └── Editor/           Prefab builders, hierarchy validator, sandbox tools
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
3. Open `Assets/_Project/Scenes/Game.unity`
4. **VertigoCase > Build All Prefabs** (regenerates all UI prefabs)
5. **VertigoCase > Validate UI Hierarchy** (should report 0 errors)
6. Play in editor or **Build And Run** for APK

## Third-Party

- [DOTween](http://dotween.demigiant.com/) — tween engine for all UI animations
