using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Textual solve-graph for a selected bomb (prefab or scene instance):
/// fragments -> parts -> persistent onPartSolved edges, listeners and chain links.
/// Flags orphaned parts (no outgoing edge and never referenced by a listener/link)
/// so "everything opens something" stays true by inspection.
/// </summary>
public class SolveGraphWindow : EditorWindow
{
    private Vector2 scroll;

    [MenuItem("Improv/Solve Graph")]
    public static void Open()
    {
        GetWindow<SolveGraphWindow>("Solve Graph");
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox("Select a bomb (prefab asset or scene GameObject with a BombManager) and press Refresh.", MessageType.Info);
        if (GUILayout.Button("Refresh")) { }

        StringBuilder sb = new StringBuilder();
        GameObject selection = Selection.activeGameObject;
        BombManager manager = selection != null ? selection.GetComponentInParent<BombManager>(true) : null;

        if (manager == null)
        {
            sb.AppendLine("No BombManager found on the current selection.");
        }
        else
        {
            DescribeBomb(manager, sb);
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);
        EditorGUILayout.TextArea(sb.ToString(), GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }

    private void DescribeBomb(BombManager manager, StringBuilder sb)
    {
        // Collect listener/link targets first, for orphan detection.
        var referenced = new HashSet<BombPart>();
        foreach (AutoSolveListener listener in manager.GetComponentsInChildren<AutoSolveListener>(true))
        {
            if (listener.TargetPart != null) referenced.Add(listener.TargetPart);
        }
        foreach (ChainLink link in manager.GetComponentsInChildren<ChainLink>(true))
        {
            if (link.TargetPart != null) referenced.Add(link.TargetPart);
        }

        sb.AppendLine($"BOMB '{manager.gameObject.name}'  (totalFragments: {manager.totalFragments})");
        sb.AppendLine();

        foreach (BombFragmentManager fragment in manager.fragments)
        {
            if (fragment == null) continue;

            bool isCore = fragment is CoreManager;
            sb.AppendLine($"  {(isCore ? "CORE" : "FRAGMENT")} '{fragment.gameObject.name}'");

            foreach (BombPart part in fragment.GetComponentsInChildren<BombPart>(true))
            {
                string verbs = part.compatibleItems != null ? string.Join(", ", part.compatibleItems) : "<none>";
                string flags = part.interactByClick ? " +click" : "";
                string state = part.isSolved ? " [SOLVED]" : part.isLocked ? " [locked]" : "";
                sb.AppendLine($"    Part {part.GetType().Name} '{part.name}'{state}  verbs: [{verbs}]{flags}");

                int edges = part.onPartSolved.GetPersistentEventCount();
                for (int i = 0; i < edges; i++)
                {
                    Object target = part.onPartSolved.GetPersistentTarget(i);
                    sb.AppendLine($"      -> {(target != null ? target.GetType().Name : "<null>")}.{part.onPartSolved.GetPersistentMethodName(i)}()");
                }

                if (edges == 0 && !referenced.Contains(part))
                {
                    sb.AppendLine("      (orphan: solves nothing and nothing targets it — intentional standalone?)");
                }
            }

            int fragmentEdges = fragment.onFragmentSolved.GetPersistentEventCount();
            for (int i = 0; i < fragmentEdges; i++)
            {
                Object target = fragment.onFragmentSolved.GetPersistentTarget(i);
                sb.AppendLine($"    [fragment solved] -> {(target != null ? target.GetType().Name : "<null>")}.{fragment.onFragmentSolved.GetPersistentMethodName(i)}()");
            }
            sb.AppendLine();
        }

        foreach (AutoSolveListener listener in manager.GetComponentsInChildren<AutoSolveListener>(true))
        {
            sb.AppendLine($"  LISTENER '{listener.name}': gate '{NameOf(listener.GatePart)}' -> target '{NameOf(listener.TargetPart)}'");
        }
        foreach (ChainLink link in manager.GetComponentsInChildren<ChainLink>(true))
        {
            sb.AppendLine($"  CHAIN '{link.name}': '{NameOf(link.GatePart)}' {link.Action} -> '{NameOf(link.TargetPart)}'");
        }
    }

    private static string NameOf(Object obj)
    {
        return obj != null ? obj.name : "<unassigned>";
    }
}
