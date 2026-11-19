using UnityEditor;
using UnityEditor.Events;
using UnityEngine;

/// <summary>
/// One click = a fully wired, runnable-but-empty bomb prefab:
/// BombManager + N fragments (auto-counted parts) + optional Core, with the
/// onFragmentSolved -> BombManager and fragment -> fragment unlock chains already
/// persistent-wired. You only drag part prefabs into the empty slots afterwards.
/// The fragment generator is also usable by hand: create a GameObject with a
/// BombFragmentManager and let autoCountParts handle the count.
/// </summary>
public class BombWizard : EditorWindow
{
    private string bombName = "NewBomb";
    private int fragmentCount = 3;
    private int partsPerFragment = 3;
    private bool includeCore = true;
    private bool chainFragments = true;
    private bool runValidator = true;

    [MenuItem("Improv/Bomb Wizard...")]
    public static void Open()
    {
        GetWindow<BombWizard>("Bomb Wizard");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Improv Bomb Wizard", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Creates a prefab skeleton: BombManager + fragments + core, all event chains pre-wired. You then drop part prefabs into the fragments' parts arrays.", MessageType.None);

        bombName = EditorGUILayout.TextField("Bomb name", bombName);
        fragmentCount = Mathf.Max(1, EditorGUILayout.IntField("Fragments", fragmentCount));
        partsPerFragment = Mathf.Max(0, EditorGUILayout.IntField("Part slots per fragment", partsPerFragment));
        includeCore = EditorGUILayout.Toggle("Include core", includeCore);
        chainFragments = EditorGUILayout.Toggle("Chain fragment unlocks", chainFragments);
        runValidator = EditorGUILayout.Toggle("Run validator after creation", runValidator);

        EditorGUILayout.Space(8);

        if (GUILayout.Button("Create Bomb Prefab", GUILayout.Height(28)))
        {
            Create();
        }
    }

    private void Create()
    {
        if (string.IsNullOrWhiteSpace(bombName))
        {
            ShowNotification(new GUIContent("Name the bomb first"));
            return;
        }

        // --- Root ---
        GameObject root = new GameObject(bombName);
        BombManager manager = root.AddComponent<BombManager>();

        GameObject container = new GameObject("Fragments");
        container.transform.SetParent(root.transform, false);

        int total = fragmentCount + (includeCore ? 1 : 0);
        BombFragmentManager[] fragments = new BombFragmentManager[total];

        // --- Fragments ---
        for (int i = 0; i < fragmentCount; i++)
        {
            GameObject fragGO = new GameObject($"Fragment_{i}");
            fragGO.transform.SetParent(container.transform, false);
            BombFragmentManager fragment = fragGO.AddComponent<BombFragmentManager>();
            fragments[i] = fragment;

            ConfigureFragment(fragment, manager);
            CreatePartSlots(fragGO, partsPerFragment);

            // fragment solved -> bomb progress
            UnityEventTools.AddPersistentListener(fragment.onFragmentSolved, manager.OnFragmentSolved);

            // previous fragment solved -> this fragment unlocks (linear chain, like Bomb V0.1)
            if (chainFragments && i > 0)
            {
                UnityEventTools.AddPersistentListener(fragments[i - 1].onFragmentSolved, fragment.Unlock);
            }
        }

        // --- Core ---
        if (includeCore)
        {
            GameObject coreGO = new GameObject("Core");
            coreGO.transform.SetParent(root.transform, false);
            CoreManager core = coreGO.AddComponent<CoreManager>();
            fragments[total - 1] = core;

            ConfigureFragment(core, manager);
            UnityEventTools.AddPersistentListener(core.onFragmentSolved, manager.OnFragmentSolved);

            manager.core = core;
        }

        // --- Manager wiring ---
        manager.fragments = fragments;
        manager.baseUnlock = new BombFragmentManager[] { fragments[0] };
        manager.totalFragments = fragmentCount; // core excluded, matches Bomb V0.1 convention

        // --- Save ---
        string folder = "Assets/Prefabs/Bomb Prefabs";
        if (!AssetDatabase.IsValidFolder(folder)) folder = "Assets";
        string path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{bombName}.prefab");

        GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);

        Selection.activeObject = saved;
        EditorGUIUtility.PingObject(saved);
        Debug.Log($"[BombWizard] Created wired bomb skeleton at {path}. Drop part prefabs into each fragment's parts array, then run Improv/Validate All Bombs.");

        if (runValidator) BombStructureValidator.ValidateAll();
    }

    private void ConfigureFragment(BombFragmentManager fragment, BombManager manager)
    {
        // private serialized fields -> SerializedObject
        SerializedObject so = new SerializedObject(fragment);
        so.FindProperty("bomb").objectReferenceValue = manager;
        so.FindProperty("autoCountParts").boolValue = true; // never hand-count parts again
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private void CreatePartSlots(GameObject fragmentGO, int count)
    {
        for (int p = 0; p < count; p++)
        {
            GameObject slot = new GameObject($"PartSlot_{p}");
            slot.transform.SetParent(fragmentGO.transform, false);
        }
    }
}
