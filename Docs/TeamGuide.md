# IMPROVISED Bomb Framework — Team Guide

**Branch:** ramework-refactor (merged from Bappo-working-branch @ 7dce724) · **Status:** implemented and verified in Unity 6000.3.8f1 (clean compile, validator green, Bomb V0.1 defused end-to-end)

One document, three parts — read what applies to you:

| Part | For | Contents |
|---|---|---|
| **1. The Plan & Rationale** | everyone, once | What changed and why, architecture review, decisions log, implementation history |
| **2. Designer Guide** | anyone building bombs/items in the Inspector | Zero-code authoring workflow, checklists |
| **3. Developer Manual** | anyone writing new component types (C#) | Full API reference, code templates, wiring, debugging |

The same content also lives as separate files: `FEATURE.md`, `Docs/AuthoringGuide.md`, `Docs/BombCodeManual.md`.

---

## Part 1 — The Plan & Rationale (FEATURE.md)

**Branch:** `Bappo-working-branch` (plan) → implemented on **`framework-refactor`** (branched from `7dce724`)
**Status:** ✅ Implemented (Phases 0–7) — **verified in Unity 6000.3.8f1**: clean compile (zero errors, only pre-existing warnings), `Improv → Validate All Bombs` clean, Bomb V0.1 defused end-to-end on the new pipeline. Developer reference: Part 3 of the team guide (Docs/TeamGuide.md), designer quick-guide: Part 2 of the team guide (Docs/TeamGuide.md).
**Scope:** Code framework only. Settings, bomb content, and component rosters are governed by the v2 design doc and are **out of scope here** — this plan only builds the machinery that content will be dragged into.

#### Implementation log (commits on `framework-refactor`)

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
| 7 | — | Part 2 of the team guide (Docs/TeamGuide.md), this status log; Bomb V0.1 migrated implicitly via the Phase 1 rewiring |

**Deferred:** the play-mode test suite — the project has no `com.unity.test-framework` package. Add it via the Package Manager first, then write the tests from §4.7.

---

### 1. Goals

1. **Inspector-first authoring.** Assembling a bomb, wiring gates, defining items = dragging prefabs, fields, and UnityEvents. Zero code per bomb, per component, per item.
2. **Easier to work with than today.** Fewer confusing overloads, no lying stubs, no typo'd public fields, obvious lifecycle, helpful inspectors, editor-time validation instead of runtime surprises.
3. **Function/skin item model.** Generic items share one *function* (Insulator, Multitool, Heat Source) but exist as different *skins* per setting — a rubber duck here, a tire piece there, identical mechanics. Setting-exclusive items are first-class, never shop items.
4. **Multi-verb items.** One item can fulfill several actions (Multitool cuts, opens, unscrews). The part decides which of its required verbs the item satisfies — the player never picks a verb.
5. **Keep the proven bones.** `BombPart` → `BombFragmentManager` → `BombManager`/`CoreManager` with Inspector-wired UnityEvents stays the gating backbone. We clean and extend it; we do not replace it.

---

### 2. Principles

| # | Principle | Consequence |
|---|---|---|
| P1 | **Inspector-only authoring** | Every tunable is a serialized field with tooltips/header; behavior selection via class choice (which part component you drop), not booleans-as-modes where avoidable |
| P2 | **Function lives in data, skin lives in prefabs** | `Item_SO` is the single source of truth for what an item *does*; prefabs only present it |
| P3 | **Verbs are append-only** | `ItemActionType` serializes as ints in every part prefab — append at the end, never reorder/renumber |
| P4 | **Fail in the editor, not at runtime** | `OnValidate` + validators catch bad wiring (non-generic item in shop, null listener target, orphaned parts) when the scene saves |
| P5 | **UnityEvents remain the wiring fabric** | Gates/chains stay drag-and-drop UnityEvents; we add small helper components that make the common wire-ups one drag instead of a method hunt |
| P6 | **One lifecycle for every part** | `Initialize → (Unlock) → TryUse → Solve`, identical for old and new part types |

---

### 3. Current State — What Stays, What's Awkward

#### Stays (verified in code)
- `BombPart` (`Assets/Scripts/Bomb/Classes/BombPart.cs`): `compatibileItems`, `isSolved`, `dontNeedTool`, `selfLocked`, `onPartSolved`/`onPartUnlocked`/`onPartWrongItem`, guarded `UseBase()` — the core interaction contract.
- `BombFragmentManager` → `BombManager` → `CoreManager` hierarchy and the cascading `Unlock()` chain.
- `PartElement` sub-clickables (`Wire`, `Symbol`, `SymbolIcon`).
- `BombHoveringManager` hover/raycast + bare-click (`dontNeedTool` → `OnItemUsed(Empty)`).
- `InteractableItem` drag-and-drop (`OnEndDrag → hoveredBombPart.OnItemUsed(ActionType)`).
- Shop/budget panels, `SpecialGiver`, `LevelData` SO (already carries `bombPrefab`, `startingBudget`, scene ref).

#### Awkward / to fix
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

### 4. Target Architecture

Five layers. Data flows down; authoring happens top-to-bottom in the Inspector.

```
Verbs        ItemActionType            (mechanical grammar, append-only)
Items        Item_SO (+ skins)         (function + economy; prefabs present it)
Parts        BombPart + subclasses     (interaction + solving + gating)
Assembly     Fragment/Manager/Core     (grouping + win chain)
Authoring    Validators, inspectors,   (editor-time enforcement + QoL)
             wizard, tests
```

#### 4.1 Layer: Verbs (`ItemActionType`)

- Stays a **plain enum** (not `[Flags]` — compatibility is an array intersection, not a bitmask).
- **Decided: existing verb names are frozen** (`Cut`, `Squeak`, `Open`, `Place`, `Disable`, `Cool`, `Special1`). New functions reuse existing verbs via data, or append brand-new ones as the part library grows — no renames.
- Append-only. Immediate change: rename unused `Special2` → `Reveal` (safe: no item maps to it, no prefab lists it). This is the only rename, and it's a name-only change of an unreferenced value.
- Verbs are the *only* mechanical vocabulary. Two items with the same verb are interchangeable mechanically — that's what makes reskins free.

#### 4.2 Layer: Items — function/skin split

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

#### 4.3 Layer: Parts — unified interaction

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

#### 4.4 Layer: Parts — new types (the framework verbs the v2 design needs)

| Class | File | Contract |
|---|---|---|
| `RevealPart : BombPart` | `Assets/Scripts/Bomb/Parts/RevealPart.cs` | Bare-click (forces `interactByClick`). Flips `revealTarget` GameObject visible, optional peel Animator trigger, then `Solve()`. Optional `RevealedCode` child component (`string code; Sprite codeSprite`) so other parts can reference the exposed code. No item consumed, no code entered. |
| `CodeEntryPart : BombPart` | `Assets/Scripts/Bomb/Parts/CodeEntryPart.cs` | Two Inspector modes: **Keypad** (small reusable panel prefab; `requiredCode` typed or dragged from a `RevealedCode`) or **SymbolOrder** (reuses existing `Symbol`/`SymbolIcon` elements). Wrong entry → `onPartWrongItem` strike. Solve chains onward. |
| `AutoSolveListener` | `Assets/Scripts/Bomb/Classes/AutoSolveListener.cs` | Not a part — a tiny component *on* a listener part. Fields: `BombPart gatePart`, `BombPart targetPart`, optional `UnityEvent onGateSolved`. Subscribes to `gatePart.onPartSolved` → `targetPart.Solve()`. Custom inspector banner: **"LISTENER — not a standalone interactable"**; forces `highlightable = false` so hover/raycast ignores it. |

These three are the *entire* new mechanical surface. Everything else in v2 content is reskins and wiring of existing + these parts.

#### 4.5 Layer: Assembly — QoL only

- `BombFragmentManager`: optional `autoCountParts` (default on for new fragments) so `toSolveParts` stops being a hand-maintained int; typo rename `onFragmentUlnocked → onFragmentUnlocked` (rewire in Phase 1).
- `ChainLink` helper component (optional but recommended): the foolproof one-drag wire-up —
  ```csharp
  // On any GameObject: "When <gatePart> solves → <Unlock|Solve|SilentLock> <targetPart>"
  public BombPart gatePart; public BombPart targetPart; public ChainAction action;
  ```
  It literally just subscribes to `onPartSolved` and calls the chosen method, so nobody has to hunt for methods in the UnityEvent dropdown. Raw UnityEvents remain available for anything exotic.

#### 4.6 Layer: Economy — per-level shops (decided)

- **Shops are per level.** `LevelData` holds an **explicit `Shop_SO` reference** (drag-and-drop — no stringly-typed lookup). Every level's shop is a distinct asset, and **no `Item_SO` may appear in more than one level's shop** — the validator errors on duplicates across shops.
- Generic functions (Insulator, Multitool, Heat Source) recur in every level's shop, but each time **as that level's skin** — rubber duck here, tire piece there. The *function* repeats; the *items* never do.
- Setting-exclusive items exist only in their level: dispensed via `SpecialGiver`, never in any shop, never persisted. Each level starts a fresh economy — enforced by data shape (nothing carries items across levels today; the validator flags any path that starts).
- `Shop_SO` gains `OnValidate()`: error on any item with `isGeneric == false`, error on duplicate `genericFunction` within the shop.
- Buying spawns the skin prefab as today (`ItemShopPanel` unchanged apart from multi-verb `Item_SO` reading).

#### 4.7 Layer: Authoring & tooling

| Tool | What it does |
|---|---|
| `BombStructureValidator` (`Assets/Editor/`, menu `Improv/Validate Bomb` + on-save) | Errors: non-generic item in shop, the same `Item_SO` listed in more than one level's shop, missing scripts, null/self `AutoSolveListener` targets, part with zero compatible verbs *and* `interactByClick == false` (unsolvable by construction). Infos: fragment part counts, standalone-component count, external-element count (design-doc targets). |
| Custom inspectors | `BombPart` state chips + play-mode test buttons; `AutoSolveListener` banner; `Item_SO` verb picker with multi-select; `Shop_SO` generic-only enforcement display. |
| `Improv/Bomb` creation wizard | Creates the casing prefab skeleton (`BombManager` + N fragments + core) pre-wired with `baseUnlock`, so a new bomb starts valid instead of assembled from scratch. |
| Solve-graph debug view | Editor window that walks `onPartSolved`/`ChainLink`/fragment wiring and draws the gate graph; flags orphaned parts and unreachable fragments. Directly supports the "components open and lead to new components" rule. |
| Play-mode tests | `Test Runner` cases in `ComponentTestingScene`: each part type solves via each supported verb path, wrong item strikes, `AutoSolveListener` fires, `RevealPart` exposes target, `CodeEntryPart` accepts/ rejects. The regression net so refactors stop being scary. |

---

### 5. Implementation Plan

Each phase compiles, runs the `Unity.unity` bomb end-to-end, and commits separately.

#### Phase 0 — Cleanup & baseline (½ day)
- [x] Delete `Assets/Prefabs/Bomb Prefabs/Components/` (missing-script prefabs), `DisabledUnityDuplicates/`, `emergency-backup.patch`.
- [x] Verify Bomb V0.1 defuses in `Unity.unity`; screenshot/note the run as the baseline.
- **Done when:** zero missing-script warnings; baseline pass recorded.

#### Phase 1 — Rename & signature cleanup (1 day)
- [x] `compatibileItems → compatibleItems`, `dontNeedTool → interactByClick`, `onFragmentUlnocked → onFragmentUnlocked`.
- [x] Rewire the ~8 live prefabs that serialize these fields (manual pass or a one-time `Assets/Editor/Migration/RenameFields.cs` that copies old→new via `SerializedObject` and logs what it touched).
- [x] Collapse `UseBase()` overloads into `UseBase(ref UseContext)`.
- [x] `Special2 → Reveal` enum rename (append-safe).
- **Done when:** Bomb V0.1 still defuses; no serialization warnings; grep finds no old names.

#### Phase 2 — Multi-verb pipeline (1 day)
- [x] `InteractableItem`: `ItemActionType[] Actions` sourced from `itemData`; remove singular `ActionType`.
- [x] `BombPart.OnItemUsed(ItemActionType[])` + intersection compatibility; update `BombHoveringManager` bare-click call site.
- [x] Update `WirePart`, `SimpleItemPart`, `SymbolPuzzlePart`, `TestCore01` to the new signature.
- [x] Author one throwaway multi-verb test item in `ComponentTestingScene` to prove a single item opens a `[Open]` part *and* cuts a `[Cut]` part.
- **Done when:** test item passes; all existing single-verb items behave as before.

#### Phase 3 — Item function/skin model (½ day)
- [x] Extend `Item_SO` (`actions[]`, `isGeneric`, `genericFunction`, `skinSetting`); delete stub methods.
- [x] Slim the seven item classes; prefab actions migrate to their `Item_SO`s (checklist pass — seven assets).
- [x] Create second skin for one function (e.g., Insulator as duck *and* tire piece) to prove the skin swap.
- **Done when:** both skins solve the same part; data is the only difference.

#### Phase 4 — New part types (1–2 days)
- [x] `RevealPart` + `RevealedCode`.
- [x] `CodeEntryPart` (both modes) + reusable keypad panel prefab.
- [x] `AutoSolveListener` + inspector banner + `highlightable = false` forcing.
- [x] `ChainLink` helper.
- [x] All four exercised in `ComponentTestingScene`.
- **Done when:** a gate chain `Reveal → CodeEntry → AutoSolveListener` runs with no custom code.

#### Phase 5 — Economy enforcement (½ day)
- [x] `Shop_SO.OnValidate()`; one shop asset **per level**; `LevelData` gains the explicit `Shop_SO` reference.
- [x] Validator rule: same `Item_SO` in more than one level's shop = error.
- [x] Confirm `SpecialGiver` path works with new item model (it freezes/releases `InteractableItem` — unaffected, but verify).
- **Done when:** exclusive-in-shop and cross-shop-duplicate both produce editor errors; buying spawns the correct skins.

#### Phase 6 — Tooling & tests (1–2 days)
- [x] `BombStructureValidator`, custom inspectors, creation wizard, solve-graph window.
- [ ] Play-mode test suite (deferred: add com.unity.test-framework package first) for every part type and the listener.
- **Done when:** `Improv/Validate Bomb` runs green on Bomb V0.1; tests pass in CI/run locally.

#### Phase 7 — Living proof (1 day)
- [x] Migrate Bomb V0.1 fully onto the new API (it becomes the reference example of inspector-only authoring).
- [x] Short Part 2 of the team guide (Docs/TeamGuide.md): "how to build a bomb without code" with pictures of the inspector.
- **Done when:** a teammate can assemble a working fragment from the guide unaided.

---

### 6. Migration & Risks

1. **Serialized renames (Phase 1) are the riskiest step** — do them first, alone, with the baseline test immediately after. A migration script that logs every touched prefab is worth the hour it takes to write.
2. **Enum append-only**: renames are name-only and safe *only* for values nothing references (`Special2` verified unused). Never reorder.
3. **Moving actions prefab→SO (Phase 3)** changes where truth lives; migrate all seven assets in one commit so there's never a half-split state.
4. **Multi-verb UX**: the part auto-selects its verb from the item's set — no mode-picking UI ever. If a part lists two verbs and an item offers both, the part's own logic decides (as today).
5. **Keypad panel is the only new UI** — one reusable prefab, or scope creeps.
6. **OneDrive workspace**: Unity `Library` + cloud sync is a known corruption risk; consider relocating the project before Phase 4+ content work.

---

### 7. Decisions Log

| # | Question | Decision |
|---|---|---|
| 1 | Rename verbs to match the new function trio (`Disable`→`Insulate`…)? | **No — old verb names stay.** New functions reuse existing verbs via data, or append new ones. No renames. |
| 2 | Shop resolution — explicit reference vs. `skinSetting` string lookup? | **Explicit `Shop_SO` reference on `LevelData`.** Pure drag-and-drop. |
| 3 | Item persistence between levels? | **None.** Shops are per level; no item appears in two levels' shops; exclusive items exist only in their level. Every level starts a fresh economy. |
| 4 | Wizard scope | **Option B — full bomb.** `Create → Improv → Bomb` generates the complete wired skeleton (`BombManager` + fragments + core + `baseUnlock` + validator stub). The fragment-generator inside it doubles as the insert-a-fragment tool (Option A) for free. Scheduled Phase 6, after the API settles. |

### 8. Open Questions

None — all decisions are recorded in the Decisions Log (§7). Implemented and verified; see the status at the top.

---

## Part 2 — Designer Guide (AuthoringGuide.md)

How to build a bomb **without writing code**. Everything below is Inspector drag-and-drop.

> Applies to the `framework-refactor` branch. Verify your work with **Improv → Validate All Bombs** early and often.
> Need to write a new component *type* in C#, or the full API reference? See **Part 3** below.

---

### 1. The five pieces

| Piece | What it is | Where |
|---|---|---|
| **Verbs** (`ItemActionType`) | The mechanical vocabulary: Cut, Squeak, Open, Place, Disable, Cool, Special1, Reveal, Empty | `Assets/Scripts/ItemScripts/ItemActionType.cs` (append-only!) |
| **Items** (`Item_SO`) | Function + economy + skin. `actions` = verbs it fulfills (multi-entry = multi-tool) | `Assets/ScriptableObjects/Items_SO/` |
| **Parts** (`BombPart` subclasses) | The interactable components on the casing | `Assets/Scripts/Bomb/Parts/` |
| **Fragments** (`BombFragmentManager`) | Groups of parts; solving the group fires the next chain | on the bomb prefab |
| **Economy** (`Shop_SO`, `SpecialGiver`) | Generic items per level's shop; exclusive items dispensed in-bomb | `Assets/ScriptableObjects/Items_SO/Shop/` |

### 2. Creating a bomb (the one-click way)

1. **Improv → Bomb Wizard...**
2. Set name, fragment count, part slots per fragment, include core, chain unlocks.
3. **Create Bomb Prefab** — you get a fully wired skeleton at `Assets/Prefabs/Bomb Prefabs/<Name>.prefab`:
   - every fragment's `onFragmentSolved` → `BombManager.OnFragmentSolved`
   - fragment N solved → fragment N+1 `Unlock()`
   - `baseUnlock` = first fragment, `autoCountParts` on everywhere
4. Drag your part prefabs into each fragment's **Parts** array. Delete unused `PartSlot_*` GameObjects.
5. Wire the special chains (see §4). Run the validator. Done.

### 3. Part menu (what to drop on the casing)

| Part | Player interaction | Inspector essentials |
|---|---|---|
| `SimpleItemPart` | right item → solved | `compatibleItems` verbs, optional solve anim / destroy |
| `WirePart` | cut the correct `Wire` elements in order | `wires`, `inOrder`, `wiresToCut` |
| `SymbolPuzzlePart` | click `Symbol`s until colors match `code` | cross-wire `DisableOvercharged` / `EnableElectricity` from other parts' `onPartSolved` |
| **`RevealPart`** | bare click → peels/wipes/pops open | `revealTarget` (hidden sticker/note), optional peel Animator, optional `RevealedCode` |
| **`CodeEntryPart`** | bare click → type code, Enter submits (Esc cancels); or SymbolOrder mode | `requiredCode` **or** drag a `RevealedCode` in as `codeSource`, optional `TMP_Text` display |
| `TestCore01` | final defuse | as before |

**Bare-click parts:** tick `interactByClick` (Reveal/CodeEntry force it on automatically). The part accepts the `Empty` verb on unlock.

### 4. Wiring gates & chains (the fun part)

Three ways, pick per situation:

1. **UnityEvents (anything)** — drag the target component into the part's `onPartSolved` list and pick the method. Unlimited freedom.
2. **`ChainLink` (the common case)** — add to any GameObject: *When [gate part] solves → [Unlock/Solve/SilentLock] [target part]*. One drag, no method hunting.
3. **`AutoSolveListener` (auto-reactors)** — for "the Disco Ball reacts to the Party Lights". Add it next to the listener part, assign `gatePart` + `targetPart`. The inspector banner reminds everyone it's **not a standalone interactable**.

Reveals feeding codes: put a `RevealedCode` on the revealed sticker object, then drag that into the `CodeEntryPart.codeSource`. The password now physically comes from the reveal.

### 5. Items & the economy

- **Generic items** (`isGeneric` on, one of `Insulator/Multitool/HeatSource`): the recurring functions, one skin per setting. Only these go in shops.
  - To reskin: duplicate an item's `Item_SO` + prefab, keep `actions`/`genericFunction`, change visuals + `skinSetting`. Same mechanics, new look.
  - Multi-tool: tick several verbs in the `Item_SO` inspector's flags field — the part picks the right verb automatically.
- **Setting-exclusive items** (`isGeneric` off): delivered by `SpecialGiver`, never in shops, never persist between levels.
- **One shop per level**: `LevelData.levelShop` points at that level's `Shop_SO`. No item may appear in two levels' shops — the validator errors if it happens.

### 6. Editor tools

| Menu item | What it does |
|---|---|
| **Improv → Bomb Wizard...** | §2 |
| **Improv → Validate All Bombs** | Missing scripts, unsolvable parts, unwired listeners/links, exclusive items in shops, cross-shop duplicates, levels missing shops |
| **Improv → Solve Graph** | Select a bomb → text tree of every fragment/part/gate edge; flags orphans |
| Part Inspector (play mode) | **Solve / Unlock / Lock** test buttons + live state chip |

### 7. Checklist before you call a bomb done

- [ ] `Improv → Validate All Bombs` — zero errors
- [ ] Solve Graph shows no accidental orphans (intentional standalones are fine)
- [ ] Exactly one standalone no-item/no-gate component
- [ ] Exactly one external element (or zero, if this is the "everything on the device" bomb)
- [ ] Every exclusive item dispensed by a `SpecialGiver`, not the shop
- [ ] `LevelData` references the level's own shop and bomb prefab

---

## Part 3 — Developer Manual (BombCodeManual.md)

The complete manual for building with the bomb framework: creating components, wiring them, creating items, and assembling bombs. Everything here works on the `framework-refactor` branch.

- **Quick designer workflow** (no code at all): see `Docs/AuthoringGuide.md`
- **This manual**: full reference, including how to write new component *types* in C#
- **Plan & rationale**: see `FEATURE.md` in the repo root

---

### Table of contents

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

### 1. Architecture at a glance

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

### 2. The part lifecycle

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

### 3. BombPart base class reference

`Assets/Scripts/Bomb/Classes/BombPart.cs`

#### Inspector fields

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

#### Properties / methods you use

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

#### `UseContext` — the one guard struct

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

### 4. Creating a new component TYPE (C#)

Create a script under `Assets/Scripts/Bomb/Parts/`, subclass `BombPart`, and implement `OnItemUsed`. Two templates cover almost everything:

#### Template A — item-driven component

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

#### Template B — sub-element component (wires, symbols, dials)

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

#### Conventions (follow the existing parts)

1. **Force your interaction mode in `OnValidate`** — e.g. Reveal/CodeEntry parts do:
   ```csharp
   private void OnValidate() { if (!interactByClick) interactByClick = true; }
   ```
2. `[AddComponentMenu("Improv/Bomb/...")]` so it appears under the Improv menu in Add Component.
3. Never reorder/renumber `ItemActionType` — append only (§9).
4. Wrong-item strikes are automatic via `UseBase` — don't call `timer.RegisterStrike()` yourself for verb mismatches (do call it for *mechanical* mistakes, like cutting wires out of order — see `WirePart`).
5. State that can be wired by designers belongs in `[SerializeField]` fields, never in code.

---

### 5. Placing components on a bomb (no code)

1. **Improv → Bomb Wizard...** creates the bomb skeleton (or use an existing bomb prefab).
2. In the fragment, create a child GameObject (or a `PartSlot_N` from the wizard).
3. **Add Component → Improv → Bomb → <your part>** (or drag an existing part prefab in).
4. Set `compatibleItems` to the verbs it accepts (or tick `interactByClick` for bare-click).
5. Assign `highlight` plane, `lockAnim` if locked, sub-elements if applicable.
6. Add the part to the fragment's **Parts** array.
7. Wire its chains (§6) and run **Improv → Validate All Bombs**.

The part inspector shows a live **SOLVED / LOCKED / OPEN** chip and, in play mode, **Solve / Unlock / Lock** test buttons.

---

### 6. Wiring components together

"The components open and lead to new components." Four mechanisms, pick per situation:

#### 6.1 `onPartSolved` UnityEvents — anything goes

Drag any component into a part's `onPartSolved` list and pick a method:
- another part's `Unlock()` (unlock a `selfLocked` chain)
- any public method (e.g. `SymbolPuzzlePart.DisableOvercharged`)
- `SpecialGiver.Give` (hand out an exclusive item)
- a fragment's `Unlock()`

Most flexible; use for anything exotic.

#### 6.2 `ChainLink` — the common case, one drag

Add **Improv → Bomb → Chain Link** to any GameObject:

```
When [gate part] solves → [Unlock | Solve | SilentLock] [target part]
```

No method hunting in dropdowns. Use this for 90% of part→part gates.

#### 6.3 `AutoSolveListener` — auto-reactors

For "X reacts when Y is solved" (Disco Ball ↔ Party Lights). Add it next to the **listener** part, assign `gatePart` + `targetPart`. The target solves automatically when the gate solves. The inspector banner reminds everyone it's **not player-facing** — don't give it an interaction.

#### 6.4 Fragment chain — progression skeleton

- Every fragment's `onFragmentSolved` → `BombManager.OnFragmentSolved` (progress count).
- Fragment N `onFragmentSolved` → Fragment N+1 `Unlock()` (the wizard wires this linearly; rearrange freely).
- All fragments solved → `core.Open()` → defused.

#### 6.5 The Reveal → Code flow (passwords from stickers)

1. Hidden object on the casing: sticker/note GameObject (starts inactive) + `RevealedCode` component holding the code string.
2. `RevealPart` on the casing with `revealTarget` = that object.
3. `CodeEntryPart` elsewhere with `codeSource` = the `RevealedCode`.
4. Player peels the reveal → reads the note → types the code. No designer needs to duplicate the password anywhere.

---

### 7. Creating new items

#### 7.1 The Item_SO (data)

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

#### 7.2 The item prefab (skin)

1. GameObject with your visuals + a **collider** (for drag/hover raycasts).
2. Add an `InteractableItem` subclass — for plain items an empty subclass is enough:
   ```csharp
   public class TirePiece : InteractableItem { }
   ```
3. Assign the `Item_SO` wherever the game spawns it (shop panel does this automatically).
4. **Exclusive items**: tag the prefab `"Special-Item"` if it should return to special spawn points.

Special behaviours (like the Ticket) can still override `OnUse()` — but verbs/economy always come from the `Item_SO`.

#### 7.3 Reskinning a generic function per setting

The rubber-duck → tire-piece trick:
1. Duplicate the duck's `Item_SO` and prefab.
2. Keep `actions` and `genericFunction` **identical**.
3. Change visuals, name, sprite, `skinSetting`.
4. Put the new skin in the new level's shop instead of the old one.

Mechanically identical, zero code.

#### 7.4 Shops (per level)

- One `Shop_SO` per level, listed via `LevelData.levelShop`.
- **Only generic items** — the inspector errors if you add an exclusive one, or two skins of the same function.
- **No item may appear in two levels' shops** — `Improv → Validate All Bombs` enforces this.

#### 7.5 Setting-exclusive items (in-bomb delivery)

1. Create the `Item_SO` with `isGeneric` **off**.
2. Place the item prefab in the scene held by a `SpecialGiver` (frozen until given).
3. Wire `SpecialGiver.Give()` from a part's `onPartSolved`.
4. It can never be bought, never persists to the next level.

---

### 8. Creating a new bomb

1. **Improv → Bomb Wizard...** → name, fragments, part slots, core → **Create**.
2. Open the generated prefab in `Assets/Prefabs/Bomb Prefabs/`.
3. Drop part prefabs into each fragment's **Parts** array; delete unused `PartSlot_N` objects.
4. Add the casing/miniature visuals as children (the bomb is one device!).
5. Wire gates per §6; put the one **external element** in the scene, wired back to a part.
6. Run **Improv → Validate All Bombs**, then **Improv → Solve Graph** to eyeball the chains.
7. Create a `LevelData` (scene + `bombPrefab` + `levelShop` + timer) and add it to the `LevelDataBase`.

Design guardrails to respect: exactly **one standalone** no-item/no-gate component, at most **one external** element (or zero for an "everything-on-the-device" bomb).

---

### 9. Verbs reference

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

### 10. Editor tools

| Menu item | Use it for |
|---|---|
| **Improv → Bomb Wizard...** | new bomb skeletons, fully wired |
| **Improv → Validate All Bombs** | pre-flight check: missing scripts, unsolvable parts, unwired listeners/links, economy violations |
| **Improv → Solve Graph** | select a bomb → text tree of every fragment/part/gate edge, flags orphans |
| Part inspector (play mode) | Solve / Unlock / Lock buttons + state chip |

---

### 11. Debugging & common pitfalls

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
