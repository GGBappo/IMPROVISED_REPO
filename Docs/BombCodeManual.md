# IMPROVISED — Bomb Code Manual

The complete manual for building with the bomb framework: creating components, wiring them, creating items, and assembling bombs. Everything here works on the `framework-refactor` branch.

- **Quick designer workflow** (no code at all): see `Docs/AuthoringGuide.md`
- **This manual**: full reference, including how to write new component *types* in C#
- **Plan & rationale**: see `FEATURE.md` in the repo root

---

## Table of contents

1. [Architecture at a glance](#1-architecture-at-a-glance)
2. [The part lifecycle](#2-the-part-lifecycle)
3. [BombPart base class reference](#3-bombpart-base-class-reference)
4. [Creating a new component TYPE (C#)](#4-creating-a-new-component-type-c)
5. [Placing components on a bomb (no code)](#5-placing-components-on-a-bomb-no-code)
6. [Wiring components together](#6-wiring-components-together)
7. [Creating new items](#7-creating-new-items)
8. [Creating a new bomb](#8-creating-a-new-bomb)
9. [Verbs reference](#9-verbs-reference)
10. [Editor tools](#10-editor-tools)
11. [Debugging & common pitfalls](#11-debugging--common-pitfalls)

---

## 1. Architecture at a glance

Five layers. Authoring flows top to bottom; data flows down.

```
VERBS        ItemActionType enum        the mechanical vocabulary (Cut, Open, ...)
ITEMS        Item_SO + prefab skin      what the player holds; verbs + economy live here
PARTS        BombPart subclasses        the interactable components on the casing
ASSEMBLY     Fragment / BombManager     grouping + the solve chain to the core
TOOLING      Improv menu, validators    editor-time guardrails
```

**Core idea:** parts declare which *verbs* they accept (`compatibleItems`). Items declare which verbs they *offer* (`Item_SO.actions`). An interaction succeeds when those sets intersect — the **part** decides which verb applies. That single rule gives you:

- **Reskins for free** — swap an item's prefab, keep its `actions`; every part treats it identically
- **Multi-tools for free** — an item with `[Cut, Open]` works on both cut-parts and open-parts; no mode-picking UI
- **Wrong-item strikes for free** — no intersection → `onPartWrongItem` fires → timer strike

## 2. The part lifecycle

Every part, old or new, follows the same path:

```
InitializePart()          BombManager start: hooks the timer (strikes)
     ↓
Unlock()                  fragment unlock reaches the part (unless selfLocked)
     ↓                          interactByClick parts also gain the Empty verb here
OnItemUsed(actions)       player interaction (item drop or bare click)
     ↓
Solve()                   sets isSolved, SilentLock(), fires onPartSolved
     ↓
onPartSolved  ──────►     gates other parts (ChainLink / UnityEvents / listeners),
                           hands out items (SpecialGiver), advances the fragment
```

Key rules:
- A part can only be used while **unlocked and unsolved** (`UseBase` enforces this).
- `selfLocked` parts are skipped by fragment `Unlock()` — they wait for an explicit `Unlock()` call, wired from another part's `onPartSolved`.
- Solving always **locks** the part (`SilentLock`) — solved parts can't be re-triggered.

## 3. BombPart base class reference

`Assets/Scripts/Bomb/Classes/BombPart.cs`

### Inspector fields

| Field | Type | Meaning |
|---|---|---|
| `fragment` | `BombFragmentManager` | parent fragment (organizational) |
| `compatibleItems` | `ItemActionType[]` | verbs this part accepts |
| `interactByClick` | `bool` | player can interact with a bare mouse click (adds the `Empty` verb on unlock) |
| `selfLocked` | `bool` | not auto-unlocked with the fragment; needs an explicit `Unlock()` |
| `lockAnim` | `Animator` | lock animation, bool `"IsLocked"` |
| `highlightable` / `highlight` | bool / GameObject | hover highlight plane |
| `onPartSolved` | `UnityEvent` | **the wiring point** — fires once when solved |
| `onPartUnlocked` | `UnityEvent` | fires when unlocked |
| `onPartWrongItem` | `UnityEvent` | fires on incompatible item use (pre-wired to timer strike) |
| `sendStrikeOnWrongItem` | `bool` | auto-register strike on wrong item (default on) |

### Properties / methods you use

| Member | Access | Purpose |
|---|---|---|
| `isSolved`, `isLocked` | public get | state checks |
| `OnItemUsed(ItemActionType[])` | public abstract | **implement this** in your component |
| `Unlock()` / `SilentLock()` | public virtual | lock control (wire `Unlock()` from other parts' events) |
| `ForceSolve()` | public | solve from code/editor/listeners (idempotent) |
| `InitializePart()` | protected virtual | override for setup, **call base** (timer hook) |
| `Solve()` | protected virtual | override to add solve VFX, **always call `base.Solve()`** |
| `UseBase(ref UseContext)` | protected | the guard — see below |
| `IsCompatible(ItemActionType[])` | protected | verb intersection check |

### `UseContext` — the one guard struct

```csharp
protected struct UseContext
{
    public ItemActionType[] itemActions;  // verbs offered (null = skip compatibility check)
    public PartElement[] elements;        // sub-elements to hover-check (null = skip)
    public int hoveredIndex;              // OUT: index of the hovered element
}
```

`UseBase(ref ctx)` returns `false` (and fires a strike on wrong item) when the part is locked/solved, no element is hovered, or the item shares no verb. Fill in only what your part needs.

---

## 4. Creating a new component TYPE (C#)

Create a script under `Assets/Scripts/Bomb/Parts/`, subclass `BombPart`, and implement `OnItemUsed`. Two templates cover almost everything:

### Template A — item-driven component

```csharp
using UnityEngine;

[AddComponentMenu("Improv/Bomb/My Gadget Part")]
public class MyGadgetPart : BombPart
{
    [SerializeField] Animator gadgetAnim;   // your own inspector fields

    public override bool OnItemUsed(ItemActionType[] itemActions)
    {
        // 1. Guard: locked? solved? right verb?
        var ctx = new UseContext { itemActions = itemActions };
        if (!UseBase(ref ctx)) return false;

        // 2. Your mechanic
        gadgetAnim.SetTrigger("Activate");

        // 3. Solve
        Solve();
        return true;
    }

    protected override void Solve()
    {
        base.Solve();                        // ALWAYS call base — it fires onPartSolved
        // extra solve VFX/sounds here
    }
}
```

### Template B — sub-element component (wires, symbols, dials)

For parts with multiple clickable sub-objects, make each sub-object a `PartElement` subclass (like `Wire` or `Symbol`) and pass them into the context:

```csharp
public class DialSequencePart : BombPart
{
    [SerializeField] Dial[] dials;          // Dial : PartElement

    public override bool OnItemUsed(ItemActionType[] itemActions)
    {
        var ctx = new UseContext { itemActions = itemActions, elements = dials };
        if (!UseBase(ref ctx)) return false;

        int clicked = ctx.hoveredIndex;     // which dial the player hovered
        dials[clicked].Turn();

        if (AllDialsCorrect()) Solve();
        return true;
    }
}
```

Notes for sub-elements:
- Put them on the **`partElementMask`** layer so `BombHoveringManager` finds them.
- `PartElement` gives you `mouseHover`, `disabled`, highlight handling.
- Parts using elements should NOT be `interactByClick` — the elements carry the clicks.

### Conventions (follow the existing parts)

1. **Force your interaction mode in `OnValidate`** — e.g. Reveal/CodeEntry parts do:
   ```csharp
   private void OnValidate() { if (!interactByClick) interactByClick = true; }
   ```
2. `[AddComponentMenu("Improv/Bomb/...")]` so it appears under the Improv menu in Add Component.
3. Never reorder/renumber `ItemActionType` — append only (§9).
4. Wrong-item strikes are automatic via `UseBase` — don't call `timer.RegisterStrike()` yourself for verb mismatches (do call it for *mechanical* mistakes, like cutting wires out of order — see `WirePart`).
5. State that can be wired by designers belongs in `[SerializeField]` fields, never in code.

---

## 5. Placing components on a bomb (no code)

1. **Improv → Bomb Wizard...** creates the bomb skeleton (or use an existing bomb prefab).
2. In the fragment, create a child GameObject (or a `PartSlot_N` from the wizard).
3. **Add Component → Improv → Bomb → <your part>** (or drag an existing part prefab in).
4. Set `compatibleItems` to the verbs it accepts (or tick `interactByClick` for bare-click).
5. Assign `highlight` plane, `lockAnim` if locked, sub-elements if applicable.
6. Add the part to the fragment's **Parts** array.
7. Wire its chains (§6) and run **Improv → Validate All Bombs**.

The part inspector shows a live **SOLVED / LOCKED / OPEN** chip and, in play mode, **Solve / Unlock / Lock** test buttons.

---

## 6. Wiring components together

"The components open and lead to new components." Four mechanisms, pick per situation:

### 6.1 `onPartSolved` UnityEvents — anything goes

Drag any component into a part's `onPartSolved` list and pick a method:
- another part's `Unlock()` (unlock a `selfLocked` chain)
- any public method (e.g. `SymbolPuzzlePart.DisableOvercharged`)
- `SpecialGiver.Give` (hand out an exclusive item)
- a fragment's `Unlock()`

Most flexible; use for anything exotic.

### 6.2 `ChainLink` — the common case, one drag

Add **Improv → Bomb → Chain Link** to any GameObject:

```
When [gate part] solves → [Unlock | Solve | SilentLock] [target part]
```

No method hunting in dropdowns. Use this for 90% of part→part gates.

### 6.3 `AutoSolveListener` — auto-reactors

For "X reacts when Y is solved" (Disco Ball ↔ Party Lights). Add it next to the **listener** part, assign `gatePart` + `targetPart`. The target solves automatically when the gate solves. The inspector banner reminds everyone it's **not player-facing** — don't give it an interaction.

### 6.4 Fragment chain — progression skeleton

- Every fragment's `onFragmentSolved` → `BombManager.OnFragmentSolved` (progress count).
- Fragment N `onFragmentSolved` → Fragment N+1 `Unlock()` (the wizard wires this linearly; rearrange freely).
- All fragments solved → `core.Open()` → defused.

### 6.5 The Reveal → Code flow (passwords from stickers)

1. Hidden object on the casing: sticker/note GameObject (starts inactive) + `RevealedCode` component holding the code string.
2. `RevealPart` on the casing with `revealTarget` = that object.
3. `CodeEntryPart` elsewhere with `codeSource` = the `RevealedCode`.
4. Player peels the reveal → reads the note → types the code. No designer needs to duplicate the password anywhere.

---

## 7. Creating new items

### 7.1 The Item_SO (data)

**Create → Item → Item Data** (or duplicate an existing one). Fields:

| Field | What to set |
|---|---|
| `actions` | Verbs the item fulfills — tick them in the flags picker. Multiple = multi-tool |
| `isGeneric` | **True only** for the 3 recurring functions (Insulator / Multitool / HeatSource skins) |
| `genericFunction` | Which function (when `isGeneric`) |
| `skinSetting` | Which setting this skin belongs to (e.g. `"Bakery"`) |
| `prefab` | The skin prefab (see below) |
| `cost` / `sellValue` / `hint` | Economy + tooltip |
| hover/drag tuning | Presentation-only |

### 7.2 The item prefab (skin)

1. GameObject with your visuals + a **collider** (for drag/hover raycasts).
2. Add an `InteractableItem` subclass — for plain items an empty subclass is enough:
   ```csharp
   public class TirePiece : InteractableItem { }
   ```
3. Assign the `Item_SO` wherever the game spawns it (shop panel does this automatically).
4. **Exclusive items**: tag the prefab `"Special-Item"` if it should return to special spawn points.

Special behaviours (like the Ticket) can still override `OnUse()` — but verbs/economy always come from the `Item_SO`.

### 7.3 Reskinning a generic function per setting

The rubber-duck → tire-piece trick:
1. Duplicate the duck's `Item_SO` and prefab.
2. Keep `actions` and `genericFunction` **identical**.
3. Change visuals, name, sprite, `skinSetting`.
4. Put the new skin in the new level's shop instead of the old one.

Mechanically identical, zero code.

### 7.4 Shops (per level)

- One `Shop_SO` per level, listed via `LevelData.levelShop`.
- **Only generic items** — the inspector errors if you add an exclusive one, or two skins of the same function.
- **No item may appear in two levels' shops** — `Improv → Validate All Bombs` enforces this.

### 7.5 Setting-exclusive items (in-bomb delivery)

1. Create the `Item_SO` with `isGeneric` **off**.
2. Place the item prefab in the scene held by a `SpecialGiver` (frozen until given).
3. Wire `SpecialGiver.Give()` from a part's `onPartSolved`.
4. It can never be bought, never persists to the next level.

---

## 8. Creating a new bomb

1. **Improv → Bomb Wizard...** → name, fragments, part slots, core → **Create**.
2. Open the generated prefab in `Assets/Prefabs/Bomb Prefabs/`.
3. Drop part prefabs into each fragment's **Parts** array; delete unused `PartSlot_N` objects.
4. Add the casing/miniature visuals as children (the bomb is one device!).
5. Wire gates per §6; put the one **external element** in the scene, wired back to a part.
6. Run **Improv → Validate All Bombs**, then **Improv → Solve Graph** to eyeball the chains.
7. Create a `LevelData` (scene + `bombPrefab` + `levelShop` + timer) and add it to the `LevelDataBase`.

Design guardrails to respect: exactly **one standalone** no-item/no-gate component, at most **one external** element (or zero for an "everything-on-the-device" bomb).

---

## 9. Verbs reference

`Assets/Scripts/ItemScripts/ItemActionType.cs` — **append-only**:

| Value | Name | Legacy user |
|---|---|---|
| 0 | `Cut` | Nail Clipper |
| 1 | `Squeak` | Rubber Duck |
| 2 | `Open` | Can Opener |
| 3 | `Place` | Potato |
| 4 | `Disable` | Shoe |
| 5 | `Cool` | Ice Cube |
| 6 | `Special1` | Ticket |
| 7 | `Reveal` | — (reserved for reveal-flavoured interactions) |
| 8 | `Empty` | bare mouse click (no item) |

**Adding a verb:** append at the end, never insert/reorder/renumber — Unity serializes these as plain ints inside every part prefab. If you add one, also add its bit to the `ItemActionTypeFlags` mirror in `Assets/Editor/Improv/Item_SOEditor.cs` (same order).

---

## 10. Editor tools

| Menu item | Use it for |
|---|---|
| **Improv → Bomb Wizard...** | new bomb skeletons, fully wired |
| **Improv → Validate All Bombs** | pre-flight check: missing scripts, unsolvable parts, unwired listeners/links, economy violations |
| **Improv → Solve Graph** | select a bomb → text tree of every fragment/part/gate edge, flags orphans |
| Part inspector (play mode) | Solve / Unlock / Lock buttons + state chip |

---

## 11. Debugging & common pitfalls

| Symptom | Cause / fix |
|---|---|
| Part won't react to clicks | Not unlocked yet, or `interactByClick` off and it has no matching verb |
| Every item strikes on a bare-click part | `interactByClick` was ticked *after* play started — `Unlock()` adds the `Empty` verb only if it's on before unlock |
| Component can never be solved | `compatibleItems` empty **and** `interactByClick` off — the validator flags exactly this |
| `selfLocked` part never unlocks | Nothing calls `Unlock()` — wire a `ChainLink` or an `onPartSolved` event to it |
| Listener fires in edit mode | It can't — listeners subscribe in `OnEnable`; enter play mode |
| Multi-tool strikes on a part it should work on | The part's `compatibleItems` doesn't include any verb the item offers — check both sides |
| Two skins of a function in one shop | Editor error by design — remove one |
| Wrong item count needed to solve fragment | Toggle `autoCountParts` on the fragment (wizard sets it) |
| Prefab lost its wiring after rename | Field renames must be paired with prefab rewiring — never rename serialized fields by hand without a migration pass |

**The golden debug loop:** Solve Graph (see the chains) → validator (catch structural breaks) → play-mode buttons (poke parts individually) → console strikes.
