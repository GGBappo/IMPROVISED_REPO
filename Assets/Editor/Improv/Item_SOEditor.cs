using UnityEditor;
using UnityEngine;

/// <summary>
/// Mirror of ItemActionType for the flags picker. KEEP IN SYNC with ItemActionType —
/// append-only, same order. (The validator does not check this, so review it when
/// adding verbs.)
/// </summary>
[System.Flags]
public enum ItemActionTypeFlags
{
    None = 0,
    Cut = 1 << 0,
    Squeak = 1 << 1,
    Open = 1 << 2,
    Place = 1 << 3,
    Disable = 1 << 4,
    Cool = 1 << 5,
    Special1 = 1 << 6,
    Reveal = 1 << 7,
    Empty = 1 << 8,
}

/// <summary>
/// Item_SO inspector with a verb flags picker: tick the verbs this item fulfills
/// (multi-verb = multi-tool) instead of managing the raw list by hand.
/// </summary>
[CustomEditor(typeof(Item_SO))]
public class Item_SOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty actions = serializedObject.FindProperty("actions");

        // Flags <-> list conversion.
        int mask = 0;
        for (int i = 0; i < actions.arraySize; i++)
        {
            int v = actions.GetArrayElementAtIndex(i).intValue;
            if (v >= 0 && v < 32) mask |= 1 << v;
        }

        EditorGUI.BeginChangeCheck();
        mask = (int)(ItemActionTypeFlags)(int)EditorGUILayout.EnumFlagsField("Actions (function)", (ItemActionTypeFlags)mask);
        if (EditorGUI.EndChangeCheck())
        {
            actions.arraySize = 0;
            for (int v = 0; v < 32; v++)
            {
                if ((mask & (1 << v)) != 0)
                {
                    actions.InsertArrayElementAtIndex(actions.arraySize);
                    actions.GetArrayElementAtIndex(actions.arraySize - 1).intValue = v;
                }
            }
        }

        EditorGUILayout.PropertyField(actions, new GUIContent("Actions (raw list)"), true);

        // Everything else, as usual.
        DrawPropertiesExcluding(serializedObject, "actions");

        serializedObject.ApplyModifiedProperties();
    }
}
