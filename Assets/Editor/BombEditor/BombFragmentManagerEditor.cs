using UnityEditor;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(BombFragmentManager))]
public class BombFragmentManagerEditor : Editor
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

    AnimBool eventsAB;
    AnimBool locksAB;

    private void OnEnable()
    {
        locksAB = new AnimBool(false);
        eventsAB = new AnimBool(false);

        locksAB.valueChanged.AddListener(Repaint);
        eventsAB.valueChanged.AddListener(Repaint);

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

        serializedObject.ApplyModifiedProperties();
    }
}
