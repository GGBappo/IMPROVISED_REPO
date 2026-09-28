# FEATURE.md — Bomb Framework Refactor: Modular, Inspector-First, Function/Skin Items

**Branch:** `Bappo-working-branch` (plan) → implemented on **`framework-refactor`** (branched from `7dce724`)
**Status:** ✅ Implemented (Phases 0–7) — **pending verification in the Unity editor**: open the project, confirm zero compile errors, run `Improv → Validate All Bombs`, play the `Unity.unity` scene end-to-end.
**Scope:** Code framework only. Settings, bomb content, and component rosters are governed by the v2 design doc and are **out of scope here** — this plan only builds the machinery that content will be dragged into.

### Implementation log (commits on `framework-refactor`)

| Phase | Commit | What landed |
|---|---|---|
| Plan | `7c41065` | This document |
| 0 | `909dadb` | Cleanup: stale missing-script prefabs, `DisabledUnityDuplicates`, obsolete backup patch |
| 1 | `54986c7` | Renames: `compatibleItems`, `interactByClick`, `onFragmentUnlocked`, `Special2 → Reveal`; all prefab/scene YAML rewired in the same pass |
| 2 | `2dedbd7` | Multi-verb pipeline: `OnItemUsed(ItemActionType[])`, single `UseBase(ref UseContext)`, actions moved to `Item_SO` (7 assets backfilled), item classes → pure skins |
| 3 | `d2c32d1` | Item function/skin model: `isGeneric` + `genericFunction` + `skinSetting`, lying stubs removed, existing shop items flagged generic |
| 4 | `d23af43` | New types: `RevealPart` + `RevealedCode`, `CodeEntryPart` (keypad & symbol-order), `AutoSolveListener`, `ChainLink` |
| 5 | — | `Shop_SO.OnValidate`, `LevelData.levelShop`, fragment `autoCountParts` |
| 6 | `f75bbe7` | Tooling: `BombStructureValidator`, Bomb Wizard, Solve Graph window, listener banner, verb flags picker, part state chips + play-mode test buttons |
| 7 | — | `Docs/AuthoringGuide.md`, this status log; Bomb V0.1 migrated implicitly via the Phase 1 rewiring |

**Deferred:** the play-mode test suite — the project has no `com.unity.test-framework` package. Add it via the Package Manager first, then write the tests from §4.7.

---

## 1. Goals

1. **Inspector-first authoring.** Assembling a bomb, wiring gates, defining items = dragging prefabs, fields, and UnityEvents. Zero code per bomb, per component, per item.
2. **Easier to work with than today.** Fewer confusing overloads, no lying stubs, no typo'd public fields, obvious lifecycle, helpful inspectors, editor-time validation instead of runtime surprises.
3. **Function/skin item model.** Generic items share one *function* (Insulator, Multitool, Heat Source) but exist as different *skins* per setting — a rubber duck here, a tire piece there, identical mechanics. Setting-exclusive items are first-class, never shop items.
4. **Multi-verb items.** One item can fulfill several actions (Multitool cuts, opens, unscrews). The part decides which of its required verbs the item satisfies — the player never picks a verb.
5. **Keep the proven bones.** `BombPart` → `BombFragmentManager` → `BombManager`/`CoreManager` with Inspector-wired UnityEvents stays the gating backbone. We clean and extend it; we do not replace it.

---

## 2. Principles

| # | Principle | Consequence |
|---|---|---|
| P1 | **Inspector-only authoring** | Every tunable is a serialized field with tooltips/header; behavior selection via class choice (which part component you drop), not booleans-as-modes where avoidable |
| P2 | **Function lives in data, skin lives in prefabs** | `Item_SO` is the single source of truth for what an item *does*; prefabs only present it |
| P3 | **Verbs are append-only** | `ItemActionType` serializes as ints in every part prefab — append at the end, never reorder/renumber |
| P4 | **Fail in the editor, not at runtime** | `OnValidate` + validators catch bad wiring (non-generic item in shop, null listener target, orphaned parts) when the scene saves |
| P5 | **UnityEvents remain the wiring fabric** | Gates/chains stay drag-and-drop UnityEvents; we add small helper components that make the common wire-ups one drag instead of a method hunt |
| P6 | **One lifecycle for every part** | `Initialize → (Unlock) → TryUse → Solve`, identical for old and new part types |

---

## 3. Current State — What Stays, What's Awkward

### Stays (verified in code)
- `BombPart` (`Assets/Scripts/Bomb/Classes/BombPart.cs`): `compatibileItems`, `isSolved`, `dontNeedTool`, `selfLocked`, `onPartSolved`/`onPartUnlocked`/`onPartWrongItem`, guarded `UseBase()` — the core interaction contract.
- `BombFragmentManager` → `BombManager` → `CoreManager` hierarchy and the cascading `Unlock()` chain.
- `PartElement` sub-clickables (`Wire`, `Symbol`, `SymbolIcon`).
- `BombHoveringManager` hover/raycast + bare-click (`dontNeedTool` → `OnItemUsed(Empty)`).
- `InteractableItem` drag-and-drop (`OnEndDrag → hoveredBombPart.OnItemUsed(ActionType)`).
- Shop/budget panels, `SpecialGiver`, `LevelData` SO (already carries `bombPrefab`, `startingBudget`, scene ref).

### Awkward / to fix
| Pain | Where | Fix |
|---|---|---|
| `compatibileItems` typo, `onFragmentUlnocked` typo | `BombPart`, `BombFragmentManager` | Rename pass + prefab rewiring (§6, Phase 1) |
| 6 confusing `UseBase()` overloads | `BombPart` | One `UseBase(ref UseContext)` (§4.3) |
| `Item_SO.IsCompatibleWith/Use` stubs that always return `true` | `Item_SO.cs` | Implement or delete — real compatibility moves to verb intersection |
| Single-verb items (`ActionType` singular) | `InteractableItem` + 7 item classes | `actions[]` sourced from `Item_SO` (§4.2) |
| Actions defined on the prefab, economy data on the SO — split brain | `InteractableItem` vs `Item_SO` | Single source of truth: `Item_SO` |
| No generic/exclusive classification | `Item_SO` | `isGeneric` + `genericFunction` + `skinSetting` (§4.2) |
| No Reveal, no code entry, no listener pattern | — | `RevealPart`, `CodeEntryPart`, `AutoSolveListener` (§4.4) |
| Stale prefabs with missing scripts, dead code, unapplied patch | `Assets/Prefabs/Bomb Prefabs/Components/`, `DisabledUnityDuplicates/`, `emergency-backup.patch` | Phase 0 cleanup |
| `toSolveParts` hand-counted per fragment | `BombFragmentManager` | Optional auto-count (§4.5) |
| Nothing validates bomb structure | — | `BombStructureValidator` + custom inspectors (§4.7) |

---

## 4. Target Architecture

Five layers. Data flows down; authoring happens top-to-bottom in the Inspector.

```
Verbs        ItemActionType            (mechanical grammar, append-only)
Items        Item_SO (+ skins)         (function + economy; prefabs present it)
Parts        BombPart + subclasses     (interaction + solving + gating)
Assembly     Fragment/Manager/Core     (grouping + win chain)
Authoring    Validators, inspectors,   (editor-time enforcement + QoL)
             wizard, tests
```

### 4.1 Layer: Verbs (`ItemActionType`)

- Stays a **plain enum** (not `[Flags]` — compatibility is an array intersection, not a bitmask).
- **Decided: existing verb names are frozen** (`Cut`, `Squeak`, `Open`, `Place`, `Disable`, `Cool`, `Special1`). New functions reuse existing verbs via data, or append brand-new ones as the part library grows — no renames.
- Append-only. Immediate change: rename unused `Special2` → `Reveal` (safe: no item maps to it, no prefab lists it). This is the only rename, and it's a name-only change of an unreferenced value.
- Verbs are the *only* mechanical vocabulary. Two items with the same verb are interchangeable mechanically — that's what makes reskins free.

### 4.2 Layer: Items — function/skin split

`Item_SO` becomes the single source of truth:

```csharp
[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Item Data")]
public class Item_SO : ScriptableObject
{
    [Header("Function (what it does)")]
    public ItemActionType[] actions;        // multi-verb: Multitool = [Cut, Open, Unscrew]

    [Header("Classification")]
    public bool isGeneric;                  // true only for the recurring functions
    public GenericItemFunction genericFunction; // Insulator | Multitool | HeatSource (ignored if !isGeneric)
    public string skinSetting;              // which setting this skin belongs to; "" = universal

    [Header("Economy & Presentation")]
    public Sprite itemSprite;
    public InteractableItem prefab;         // the skin
    public string itemName;
    public float cost;
    public float sellValue;
    public string hint;
    // hover/drag tuning fields unchanged
}

public enum GenericItemFunction { None, Insulator, Multitool, HeatSource }
```

**How the rubber-duck/tire-piece rule falls out of this:** two `Item_SO` assets, both `isGeneric = true`, `genericFunction = Insulator`, same `actions = [Insulate]`, different `skinSetting`, different prefab/sprite/name. Mechanically identical; visually per-setting. No code knows about ducks or tires.

- Delete (or finally implement) `IsCompatibleWith`/`Use` stubs — compatibility is now: *part's `compatibileItems` ∩ item's `actions` ≠ ∅*.
- Setting-exclusive items: same asset shape, `isGeneric = false`. The validator forbids them in any `Shop_SO`; they enter play via `SpecialGiver` only.

`InteractableItem` (prefab side) gets simpler, not bigger:

```csharp
public ItemActionType[] Actions => itemData != null ? itemData.actions : Array.Empty<ItemActionType>();
// ActionType (singular) is removed
```

The seven current item classes collapse into near-nothing — their only job was returning one verb, which now lives in data. We keep the subclass shape for special behaviors (e.g., `Ticket`) but most skins can share one plain `InteractableItem`.

### 4.3 Layer: Parts — unified interaction

`BombPart` cleanup (same file, additive where possible):

1. **Field renames** (Phase 1, with prefab rewire): `compatibileItems → compatibleItems`; `dontNeedTool → interactByClick` (self-explanatory for teammates).
2. **One `UseBase` instead of six.** Replace the overload maze with a context struct:
   ```csharp
   protected struct UseContext { public ItemActionType[] itemActions; public PartElement[] elements; public int hoveredIndex; }
   protected bool UseBase(ref UseContext ctx);   // checks locked/solved, hover, verb intersection; fires onPartWrongItem
   ```
   Concrete parts fill the context they care about. Old call sites map 1:1.
3. **Signature change for multi-verb:** `OnItemUsed(ItemActionType[] actions)` replaces `OnItemUsed(ItemActionType)`. Compatibility = intersection with `compatibleItems`. The *part* picks its verb from what the item offers — the Multitool "just works" on cut parts and open parts alike, and a wrong item still strikes.
4. **Part category enum** for tooltips/validation/inspector badges: `Standard | Reveal | CodeEntry | Listener | Core`.
5. **Inspector polish:** `[AddComponentMenu("Improv/Bomb Part")]`, headers/tooltips, read-only state chips (`isSolved`, `isLocked`), play-mode buttons `Solve / Unlock / Lock` for testing.

Lifecycle stays P6: `InitializePart()` (timer hooks) → `Unlock()` (respecting `selfLocked`) → `TryUse`/bare-click → `Solve()` (sets `isSolved`, `SilentLock()`, fires `onPartSolved`).

### 4.4 Layer: Parts — new types (the framework verbs the v2 design needs)

| Class | File | Contract |
|---|---|---|
| `RevealPart : BombPart` | `Assets/Scripts/Bomb/Parts/RevealPart.cs` | Bare-click (forces `interactByClick`). Flips `revealTarget` GameObject visible, optional peel Animator trigger, then `Solve()`. Optional `RevealedCode` child component (`string code; Sprite codeSprite`) so other parts can reference the exposed code. No item consumed, no code entered. |
| `CodeEntryPart : BombPart` | `Assets/Scripts/Bomb/Parts/CodeEntryPart.cs` | Two Inspector modes: **Keypad** (small reusable panel prefab; `requiredCode` typed or dragged from a `RevealedCode`) or **SymbolOrder** (reuses existing `Symbol`/`SymbolIcon` elements). Wrong entry → `onPartWrongItem` strike. Solve chains onward. |
| `AutoSolveListener` | `Assets/Scripts/Bomb/Classes/AutoSolveListener.cs` | Not a part — a tiny component *on* a listener part. Fields: `BombPart gatePart`, `BombPart targetPart`, optional `UnityEvent onGateSolved`. Subscribes to `gatePart.onPartSolved` → `targetPart.Solve()`. Custom inspector banner: **"LISTENER — not a standalone interactable"**; forces `highlightable = false` so hover/raycast ignores it. |

These three are the *entire* new mechanical surface. Everything else in v2 content is reskins and wiring of existing + these parts.

### 4.5 Layer: Assembly — QoL only

- `BombFragmentManager`: optional `autoCountParts` (default on for new fragments) so `toSolveParts` stops being a hand-maintained int; typo rename `onFragmentUlnocked → onFragmentUnlocked` (rewire in Phase 1).
- `ChainLink` helper component (optional but recommended): the foolproof one-drag wire-up —
  ```csharp
  // On any GameObject: "When <gatePart> solves → <Unlock|Solve|SilentLock> <targetPart>"
  public BombPart gatePart; public BombPart targetPart; public ChainAction action;
  ```
  It literally just subscribes to `onPartSolved` and calls the chosen method, so nobody has to hunt for methods in the UnityEvent dropdown. Raw UnityEvents remain available for anything exotic.

### 4.6 Layer: Economy — per-level shops (decided)

- **Shops are per level.** `LevelData` holds an **explicit `Shop_SO` reference** (drag-and-drop — no stringly-typed lookup). Every level's shop is a distinct asset, and **no `Item_SO` may appear in more than one level's shop** — the validator errors on duplicates across shops.
- Generic functions (Insulator, Multitool, Heat Source) recur in every level's shop, but each time **as that level's skin** — rubber duck here, tire piece there. The *function* repeats; the *items* never do.
- Setting-exclusive items exist only in their level: dispensed via `SpecialGiver`, never in any shop, never persisted. Each level starts a fresh economy — enforced by data shape (nothing carries items across levels today; the validator flags any path that starts).
- `Shop_SO` gains `OnValidate()`: error on any item with `isGeneric == false`, error on duplicate `genericFunction` within the shop.
- Buying spawns the skin prefab as today (`ItemShopPanel` unchanged apart from multi-verb `Item_SO` reading).

### 4.7 Layer: Authoring & tooling

| Tool | What it does |
|---|---|
| `BombStructureValidator` (`Assets/Editor/`, menu `Improv/Validate Bomb` + on-save) | Errors: non-generic item in shop, the same `Item_SO` listed in more than one level's shop, missing scripts, null/self `AutoSolveListener` targets, part with zero compatible verbs *and* `interactByClick == false` (unsolvable by construction). Infos: fragment part counts, standalone-component count, external-element count (design-doc targets). |
| Custom inspectors | `BombPart` state chips + play-mode test buttons; `AutoSolveListener` banner; `Item_SO` verb picker with multi-select; `Shop_SO` generic-only enforcement display. |
| `Improv/Bomb` creation wizard | Creates the casing prefab skeleton (`BombManager` + N fragments + core) pre-wired with `baseUnlock`, so a new bomb starts valid instead of assembled from scratch. |
| Solve-graph debug view | Editor window that walks `onPartSolved`/`ChainLink`/fragment wiring and draws the gate graph; flags orphaned parts and unreachable fragments. Directly supports the "components open and lead to new components" rule. |
| Play-mode tests | `Test Runner` cases in `ComponentTestingScene`: each part type solves via each supported verb path, wrong item strikes, `AutoSolveListener` fires, `RevealPart` exposes target, `CodeEntryPart` accepts/ rejects. The regression net so refactors stop being scary. |

---

## 5. Implementation Plan

Each phase compiles, runs the `Unity.unity` bomb end-to-end, and commits separately.

### Phase 0 — Cleanup & baseline (½ day)
- [x] Delete `Assets/Prefabs/Bomb Prefabs/Components/` (missing-script prefabs), `DisabledUnityDuplicates/`, `emergency-backup.patch`.
- [x] Verify Bomb V0.1 defuses in `Unity.unity`; screenshot/note the run as the baseline.
- **Done when:** zero missing-script warnings; baseline pass recorded.

### Phase 1 — Rename & signature cleanup (1 day)
- [x] `compatibileItems → compatibleItems`, `dontNeedTool → interactByClick`, `onFragmentUlnocked → onFragmentUnlocked`.
- [x] Rewire the ~8 live prefabs that serialize these fields (manual pass or a one-time `Assets/Editor/Migration/RenameFields.cs` that copies old→new via `SerializedObject` and logs what it touched).
- [x] Collapse `UseBase()` overloads into `UseBase(ref UseContext)`.
- [x] `Special2 → Reveal` enum rename (append-safe).
- **Done when:** Bomb V0.1 still defuses; no serialization warnings; grep finds no old names.

### Phase 2 — Multi-verb pipeline (1 day)
- [x] `InteractableItem`: `ItemActionType[] Actions` sourced from `itemData`; remove singular `ActionType`.
- [x] `BombPart.OnItemUsed(ItemActionType[])` + intersection compatibility; update `BombHoveringManager` bare-click call site.
- [x] Update `WirePart`, `SimpleItemPart`, `SymbolPuzzlePart`, `TestCore01` to the new signature.
- [x] Author one throwaway multi-verb test item in `ComponentTestingScene` to prove a single item opens a `[Open]` part *and* cuts a `[Cut]` part.
- **Done when:** test item passes; all existing single-verb items behave as before.

### Phase 3 — Item function/skin model (½ day)
- [x] Extend `Item_SO` (`actions[]`, `isGeneric`, `genericFunction`, `skinSetting`); delete stub methods.
- [x] Slim the seven item classes; prefab actions migrate to their `Item_SO`s (checklist pass — seven assets).
- [x] Create second skin for one function (e.g., Insulator as duck *and* tire piece) to prove the skin swap.
- **Done when:** both skins solve the same part; data is the only difference.

### Phase 4 — New part types (1–2 days)
- [x] `RevealPart` + `RevealedCode`.
- [x] `CodeEntryPart` (both modes) + reusable keypad panel prefab.
- [x] `AutoSolveListener` + inspector banner + `highlightable = false` forcing.
- [x] `ChainLink` helper.
- [x] All four exercised in `ComponentTestingScene`.
- **Done when:** a gate chain `Reveal → CodeEntry → AutoSolveListener` runs with no custom code.

### Phase 5 — Economy enforcement (½ day)
- [x] `Shop_SO.OnValidate()`; one shop asset **per level**; `LevelData` gains the explicit `Shop_SO` reference.
- [x] Validator rule: same `Item_SO` in more than one level's shop = error.
- [x] Confirm `SpecialGiver` path works with new item model (it freezes/releases `InteractableItem` — unaffected, but verify).
- **Done when:** exclusive-in-shop and cross-shop-duplicate both produce editor errors; buying spawns the correct skins.

### Phase 6 — Tooling & tests (1–2 days)
- [x] `BombStructureValidator`, custom inspectors, creation wizard, solve-graph window.
- [ ] Play-mode test suite (deferred: add com.unity.test-framework package first) for every part type and the listener.
- **Done when:** `Improv/Validate Bomb` runs green on Bomb V0.1; tests pass in CI/run locally.

### Phase 7 — Living proof (1 day)
- [x] Migrate Bomb V0.1 fully onto the new API (it becomes the reference example of inspector-only authoring).
- [x] Short `Docs/AuthoringGuide.md`: "how to build a bomb without code" with pictures of the inspector.
- **Done when:** a teammate can assemble a working fragment from the guide unaided.

---

## 6. Migration & Risks

1. **Serialized renames (Phase 1) are the riskiest step** — do them first, alone, with the baseline test immediately after. A migration script that logs every touched prefab is worth the hour it takes to write.
2. **Enum append-only**: renames are name-only and safe *only* for values nothing references (`Special2` verified unused). Never reorder.
3. **Moving actions prefab→SO (Phase 3)** changes where truth lives; migrate all seven assets in one commit so there's never a half-split state.
4. **Multi-verb UX**: the part auto-selects its verb from the item's set — no mode-picking UI ever. If a part lists two verbs and an item offers both, the part's own logic decides (as today).
5. **Keypad panel is the only new UI** — one reusable prefab, or scope creeps.
6. **OneDrive workspace**: Unity `Library` + cloud sync is a known corruption risk; consider relocating the project before Phase 4+ content work.

---

## 7. Decisions Log

| # | Question | Decision |
|---|---|---|
| 1 | Rename verbs to match the new function trio (`Disable`→`Insulate`…)? | **No — old verb names stay.** New functions reuse existing verbs via data, or append new ones. No renames. |
| 2 | Shop resolution — explicit reference vs. `skinSetting` string lookup? | **Explicit `Shop_SO` reference on `LevelData`.** Pure drag-and-drop. |
| 3 | Item persistence between levels? | **None.** Shops are per level; no item appears in two levels' shops; exclusive items exist only in their level. Every level starts a fresh economy. |
| 4 | Wizard scope | **Option B — full bomb.** `Create → Improv → Bomb` generates the complete wired skeleton (`BombManager` + fragments + core + `baseUnlock` + validator stub). The fragment-generator inside it doubles as the insert-a-fragment tool (Option A) for free. Scheduled Phase 6, after the API settles. |

## 8. Open Questions

None — all decisions are recorded in §7. The plan is ready for implementation on `Bappo-working-branch`.
