using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

[CustomEditor(typeof(CoreManager))]
public class CoreManagerEditor : Editor
{
    #region Properites
    SerializedProperty bomb;
    SerializedProperty parts;
    SerializedProperty toSolveParts;
    SerializedProperty onFragmentSolved;
    SerializedProperty onFragmentUlnocked;
    SerializedProperty lockAnim;

    bool events = false;
    bool locks = false;
    bool core = false;

    AnimBool eventsAB;
    AnimBool locksAB;
    AnimBool coreAB;

    private void OnEnable()
    {
        locksAB = new AnimBool(false);
        eventsAB = new AnimBool(false);
        coreAB = new AnimBool(false);

        locksAB.valueChanged.AddListener(Repaint);
        eventsAB.valueChanged.AddListener(Repaint);
        coreAB.valueChanged.AddListener(Repaint);

        bomb = serializedObject.FindProperty("bomb");
        parts = serializedObject.FindProperty("parts");
        toSolveParts = serializedObject.FindProperty("toSolveParts");
        onFragmentSolved = serializedObject.FindProperty("onFragmentSolved");
        onFragmentUlnocked = serializedObject.FindProperty("onFragmentUlnocked");
        lockAnim = serializedObject.FindProperty("lockAnim");
    }
    #endregion

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(bomb);
        EditorGUILayout.PropertyField(parts);
        EditorGUILayout.PropertyField(toSolveParts);

        events = EditorGUILayout.BeginFoldoutHeaderGroup(events, "Events");
        eventsAB.target = events;
        if (EditorGUILayout.BeginFadeGroup(eventsAB.faded))
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(onFragmentSolved);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(onFragmentUlnocked);
            EditorGUILayout.Space(5);
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndFoldoutHeaderGroup();
        locks = EditorGUILayout.BeginFoldoutHeaderGroup(locks, "Locks");
        locksAB.target = locks;
        if (EditorGUILayout.BeginFadeGroup(locksAB.faded))
        {
            EditorGUILayout.PropertyField(lockAnim);
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(7.5F);
        //CORE
        core = EditorGUILayout.BeginFoldoutHeaderGroup(core, "Core");
        coreAB.target = core;
        if (EditorGUILayout.BeginFadeGroup(coreAB.faded))
        {
            string[] toExclude = new string[7];
            toExclude[0] = "m_Script";
            toExclude[1] = "bomb";
            toExclude[2] = "parts";
            toExclude[3] = "toSolveParts";
            toExclude[4] = "onFragmentSolved";
            toExclude[5] = "onFragmentUlnocked";
            toExclude[6] = "lockAnim";

            DrawPropertiesExcluding(serializedObject, toExclude);
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
