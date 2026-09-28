using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor-time guardrails for the bomb framework. Catches the classic silent-bomb bugs
/// before play: missing scripts, unsolvable parts, unwired listeners/links, and
/// item-economy violations (exclusive items in shops, items shared across levels' shops).
/// </summary>
public static class BombStructureValidator
{
    [MenuItem("Improv/Validate All Bombs")]
    public static void ValidateAll()
    {
        int errors = 0;
        int infos = 0;

        // --- Bomb prefabs ---
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (root == null) continue;
            if (root.GetComponentInChildren<BombManager>(true) == null) continue;

            errors += ValidateBomb(root, path);
        }

        // --- Shops: generic-only rule + no item in two levels' shops ---
        var itemOwners = new Dictionary<Item_SO, List<string>>();
        foreach (string guid in AssetDatabase.FindAssets("t:Shop_SO"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Shop_SO shop = AssetDatabase.LoadAssetAtPath<Shop_SO>(path);
            if (shop == null || shop.shopItems == null) continue;

            var seenFunctions = new HashSet<GenericItemFunction>();
            foreach (Item_SO item in shop.shopItems)
            {
                if (item == null) continue;

                if (!item.isGeneric)
                {
                    Debug.LogError($"[Validate] {path}: setting-exclusive item '{SafeName(item)}' is listed in a shop. Exclusive items must be delivered in-bomb via SpecialGiver.");
                    errors++;
                }
                else if (!seenFunctions.Add(item.genericFunction))
                {
                    Debug.LogError($"[Validate] {path}: more than one skin of generic function '{item.genericFunction}' — a level's shop carries exactly one skin per function.");
                    errors++;
                }

                if (!itemOwners.TryGetValue(item, out List<string> owners))
                {
                    owners = new List<string>();
                    itemOwners[item] = owners;
                }
                owners.Add(System.IO.Path.GetFileNameWithoutExtension(path));
            }
        }
        foreach (KeyValuePair<Item_SO, List<string>> kv in itemOwners)
        {
            if (kv.Value.Count > 1)
            {
                Debug.LogError($"[Validate] Item '{SafeName(kv.Key)}' appears in {kv.Value.Count} shops ({string.Join(", ", kv.Value)}). Shops are per level — no item may appear in two levels' shops.");
                errors++;
            }
        }

        // --- Levels: shop reference info ---
        foreach (string guid in AssetDatabase.FindAssets("t:LevelData"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            LevelData level = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            if (level == null) continue;
            if (!level.isDud && level.levelShop == null)
            {
                Debug.Log($"[Validate] INFO {path}: no levelShop assigned yet.");
                infos++;
            }
        }

        Debug.Log($"[Validate] Done — {errors} error(s), {infos} info note(s).");
        if (errors == 0) Debug.Log("[Validate] All clear. Nice bomb hygiene.");
    }

    private static int ValidateBomb(GameObject root, string path)
    {
        int errors = 0;

        // Missing scripts anywhere under the bomb.
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject) > 0)
            {
                Debug.LogError($"[Validate] {path}: '{t.name}' has missing script(s).");
                errors++;
            }
        }

        // Parts must be solvable by construction.
        foreach (BombPart part in root.GetComponentsInChildren<BombPart>(true))
        {
            bool acceptsAnyVerb = part.compatibleItems != null && part.compatibleItems.Length > 0;
            if (!acceptsAnyVerb && !part.interactByClick)
            {
                Debug.LogError($"[Validate] {path}: part '{part.name}' ({part.GetType().Name}) accepts no verb and isn't interactByClick — the player can never solve it.");
                errors++;
            }
        }

        // Listeners and links must be wired.
        foreach (AutoSolveListener listener in root.GetComponentsInChildren<AutoSolveListener>(true))
        {
            if (listener.GatePart == null)
            {
                Debug.LogError($"[Validate] {path}: AutoSolveListener '{listener.name}' has no gate part — it will never fire.");
                errors++;
            }
            else if (listener.GatePart == listener.GetComponent<BombPart>())
            {
                Debug.LogError($"[Validate] {path}: AutoSolveListener '{listener.name}' gates its own part.");
                errors++;
            }
        }
        foreach (ChainLink link in root.GetComponentsInChildren<ChainLink>(true))
        {
            if (link.GatePart == null || link.TargetPart == null)
            {
                Debug.LogError($"[Validate] {path}: ChainLink '{link.name}' is missing gate or target.");
                errors++;
            }
        }

        return errors;
    }

    private static string SafeName(Item_SO item)
    {
        return item != null ? (string.IsNullOrEmpty(item.itemName) ? item.name : item.itemName) : "<null>";
    }
}
