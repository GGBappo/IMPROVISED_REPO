using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

[CustomEditor(typeof(BombPart), true)]
public class BombPartEditor : Editor
{
    #region Properites
    SerializedProperty fragment;
    SerializedProperty compatibileItems;
    SerializedProperty dontNeedTool;
    SerializedProperty selfLocked;
    SerializedProperty lockAnim;
    SerializedProperty highlightable;
    SerializedProperty highlight;
    SerializedProperty onPartSolved;
    SerializedProperty onPartUnlocked;
    SerializedProperty onPartWrongItem;
    SerializedProperty sendStrikeOnWrongItem;

    bool bases = false;
    AnimBool basesAB;
    bool locks = false;
    AnimBool locksAB;
    bool highs = false;
    AnimBool highsAB;
    bool events = false;
    AnimBool eventsAB;

    bool parts = false;
    string button = "Show";
    AnimBool partAB;

    AnimBool selfLockAB;
    AnimBool highlightableAB;
    AnimBool dntAB;



    private void OnEnable()
    {
        partAB = new AnimBool(false);
        locksAB = new AnimBool(false);
        basesAB = new AnimBool(false);
        highsAB = new AnimBool(false);
        eventsAB = new AnimBool(false);
        selfLockAB = new AnimBool(false);
        highlightableAB = new AnimBool(false);
        dntAB = new AnimBool(false);

        partAB.valueChanged.AddListener(Repaint);
        locksAB.valueChanged.AddListener(Repaint);
        basesAB.valueChanged.AddListener(Repaint);
        highsAB.valueChanged.AddListener(Repaint);
        eventsAB.valueChanged.AddListener(Repaint);
        selfLockAB.valueChanged.AddListener(Repaint);
        highlightableAB.valueChanged.AddListener(Repaint);
        dntAB.valueChanged.AddListener(Repaint);

        fragment = serializedObject.FindProperty("fragment");
        compatibileItems = serializedObject.FindProperty("compatibileItems");
        dontNeedTool = serializedObject.FindProperty("dontNeedTool");
        selfLocked = serializedObject.FindProperty("selfLocked");
        lockAnim = serializedObject.FindProperty("lockAnim");
        highlightable = serializedObject.FindProperty("highlightable");
        highlight = serializedObject.FindProperty("highlight");
        onPartSolved = serializedObject.FindProperty("onPartSolved");
        onPartUnlocked = serializedObject.FindProperty("onPartUnlocked");
        onPartWrongItem = serializedObject.FindProperty("onPartWrongItem");
        sendStrikeOnWrongItem = serializedObject.FindProperty("sendStrikeOnWrongItem");
    }
    #endregion

    public override void OnInspectorGUI()
    {
        BombPart part = (BombPart)target;
        serializedObject.Update();

        //BASE
        bases = EditorGUILayout.BeginFoldoutHeaderGroup(bases, "Base");
        basesAB.target = bases;
        if (EditorGUILayout.BeginFadeGroup(basesAB.faded))
        {
            EditorGUILayout.PropertyField(fragment);
            EditorGUILayout.PropertyField(dontNeedTool);
            dntAB.target = !dontNeedTool.boolValue;
            if (EditorGUILayout.BeginFadeGroup(dntAB.faded))
            {
                EditorGUILayout.PropertyField(compatibileItems);
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space(5);
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndFoldoutHeaderGroup();

        //LOKCS
        locks = EditorGUILayout.BeginFoldoutHeaderGroup(locks, "Lokcs");
        locksAB.target = locks;
        if (EditorGUILayout.BeginFadeGroup(locksAB.faded))
        {
            EditorGUILayout.PropertyField(selfLocked);
            selfLockAB.target = selfLocked.boolValue;
            if (EditorGUILayout.BeginFadeGroup(selfLockAB.faded))
            {
                EditorGUILayout.PropertyField(lockAnim);
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space(5);
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndFoldoutHeaderGroup();

        //HIGHLIGHT
        highs = EditorGUILayout.BeginFoldoutHeaderGroup(highs, "Highlight");
        highsAB.target = highs;
        if (EditorGUILayout.BeginFadeGroup(highsAB.faded))
        {
            EditorGUILayout.PropertyField(highlightable);
            highlightableAB.target = highlightable.boolValue;
            if (EditorGUILayout.BeginFadeGroup(highlightableAB.faded))
            {
                EditorGUILayout.PropertyField(highlight);
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.Space(5);
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndFoldoutHeaderGroup();

        //EVENTS
        events = EditorGUILayout.BeginFoldoutHeaderGroup(events, "Events");
        eventsAB.target = events;
        if (EditorGUILayout.BeginFadeGroup(eventsAB.faded))
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(onPartSolved);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(onPartUnlocked);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(sendStrikeOnWrongItem);
            EditorGUILayout.PropertyField(onPartWrongItem);
            EditorGUILayout.Space(5);
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(15F);
        //PART
        if (GUILayout.Button(button + " Part Attributes"))
        {
            parts = !parts;
            partAB.target = parts;
        }
        EditorGUILayout.Space(2.5F);

        if (EditorGUILayout.BeginFadeGroup(partAB.faded))
        {
            string[] toExclude = new string[12];
            toExclude[0] = "m_Script";
            toExclude[1] = "fragment";
            toExclude[2] = "compatibileItems";
            toExclude[3] = "dontNeedTool";
            toExclude[4] = "lockAnim";
            toExclude[5] = "highlightable";
            toExclude[6] = "highlight";
            toExclude[7] = "onPartSolved";
            toExclude[8] = "onPartUnlocked";
            toExclude[9] = "onPartWrongItem";
            toExclude[10] = "sendStrikeOnWrongItem";
            toExclude[11] = "selfLocked";

            DrawPropertiesExcluding(serializedObject, toExclude);

            EditorGUILayout.Space(5F);

            GUIStyle style = GUIStyle.none;
            style.fontSize = 10;
            style.fontStyle = FontStyle.Italic;
            style.normal.textColor = new Color(0.65F, 0.65F, 0.65F);
            style.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField("If nothing shows up here, that means that there is no custom attributes in the BombPart.", style);
        }
        EditorGUILayout.EndFadeGroup();

        if (parts)
        {
            button = "Hide";
        }
        else
        {
            button = "Show";
        }


        serializedObject.ApplyModifiedProperties();
    }
}
