# Improv Bomb Authoring Guide

How to build a bomb **without writing code**. Everything below is Inspector drag-and-drop.

> Applies to the `framework-refactor` branch. Verify your work with **Improv → Validate All Bombs** early and often.

---

## 1. The five pieces

| Piece | What it is | Where |
|---|---|---|
| **Verbs** (`ItemActionType`) | The mechanical vocabulary: Cut, Squeak, Open, Place, Disable, Cool, Special1, Reveal, Empty | `Assets/Scripts/ItemScripts/ItemActionType.cs` (append-only!) |
| **Items** (`Item_SO`) | Function + economy + skin. `actions` = verbs it fulfills (multi-entry = multi-tool) | `Assets/ScriptableObjects/Items_SO/` |
| **Parts** (`BombPart` subclasses) | The interactable components on the casing | `Assets/Scripts/Bomb/Parts/` |
| **Fragments** (`BombFragmentManager`) | Groups of parts; solving the group fires the next chain | on the bomb prefab |
| **Economy** (`Shop_SO`, `SpecialGiver`) | Generic items per level's shop; exclusive items dispensed in-bomb | `Assets/ScriptableObjects/Items_SO/Shop/` |

## 2. Creating a bomb (the one-click way)

1. **Improv → Bomb Wizard...**
2. Set name, fragment count, part slots per fragment, include core, chain unlocks.
3. **Create Bomb Prefab** — you get a fully wired skeleton at `Assets/Prefabs/Bomb Prefabs/<Name>.prefab`:
   - every fragment's `onFragmentSolved` → `BombManager.OnFragmentSolved`
   - fragment N solved → fragment N+1 `Unlock()`
   - `baseUnlock` = first fragment, `autoCountParts` on everywhere
4. Drag your part prefabs into each fragment's **Parts** array. Delete unused `PartSlot_*` GameObjects.
5. Wire the special chains (see §4). Run the validator. Done.

## 3. Part menu (what to drop on the casing)

| Part | Player interaction | Inspector essentials |
|---|---|---|
| `SimpleItemPart` | right item → solved | `compatibleItems` verbs, optional solve anim / destroy |
| `WirePart` | cut the correct `Wire` elements in order | `wires`, `inOrder`, `wiresToCut` |
| `SymbolPuzzlePart` | click `Symbol`s until colors match `code` | cross-wire `DisableOvercharged` / `EnableElectricity` from other parts' `onPartSolved` |
| **`RevealPart`** | bare click → peels/wipes/pops open | `revealTarget` (hidden sticker/note), optional peel Animator, optional `RevealedCode` |
| **`CodeEntryPart`** | bare click → type code, Enter submits (Esc cancels); or SymbolOrder mode | `requiredCode` **or** drag a `RevealedCode` in as `codeSource`, optional `TMP_Text` display |
| `TestCore01` | final defuse | as before |

**Bare-click parts:** tick `interactByClick` (Reveal/CodeEntry force it on automatically). The part accepts the `Empty` verb on unlock.

## 4. Wiring gates & chains (the fun part)

Three ways, pick per situation:

1. **UnityEvents (anything)** — drag the target component into the part's `onPartSolved` list and pick the method. Unlimited freedom.
2. **`ChainLink` (the common case)** — add to any GameObject: *When [gate part] solves → [Unlock/Solve/SilentLock] [target part]*. One drag, no method hunting.
3. **`AutoSolveListener` (auto-reactors)** — for "the Disco Ball reacts to the Party Lights". Add it next to the listener part, assign `gatePart` + `targetPart`. The inspector banner reminds everyone it's **not a standalone interactable**.

Reveals feeding codes: put a `RevealedCode` on the revealed sticker object, then drag that into the `CodeEntryPart.codeSource`. The password now physically comes from the reveal.

## 5. Items & the economy

- **Generic items** (`isGeneric` on, one of `Insulator/Multitool/HeatSource`): the recurring functions, one skin per setting. Only these go in shops.
  - To reskin: duplicate an item's `Item_SO` + prefab, keep `actions`/`genericFunction`, change visuals + `skinSetting`. Same mechanics, new look.
  - Multi-tool: tick several verbs in the `Item_SO` inspector's flags field — the part picks the right verb automatically.
- **Setting-exclusive items** (`isGeneric` off): delivered by `SpecialGiver`, never in shops, never persist between levels.
- **One shop per level**: `LevelData.levelShop` points at that level's `Shop_SO`. No item may appear in two levels' shops — the validator errors if it happens.

## 6. Editor tools

| Menu item | What it does |
|---|---|
| **Improv → Bomb Wizard...** | §2 |
| **Improv → Validate All Bombs** | Missing scripts, unsolvable parts, unwired listeners/links, exclusive items in shops, cross-shop duplicates, levels missing shops |
| **Improv → Solve Graph** | Select a bomb → text tree of every fragment/part/gate edge; flags orphans |
| Part Inspector (play mode) | **Solve / Unlock / Lock** test buttons + live state chip |

## 7. Checklist before you call a bomb done

- [ ] `Improv → Validate All Bombs` — zero errors
- [ ] Solve Graph shows no accidental orphans (intentional standalones are fine)
- [ ] Exactly one standalone no-item/no-gate component
- [ ] Exactly one external element (or zero, if this is the "everything on the device" bomb)
- [ ] Every exclusive item dispensed by a `SpecialGiver`, not the shop
- [ ] `LevelData` references the level's own shop and bomb prefab
