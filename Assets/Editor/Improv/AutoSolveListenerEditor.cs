using UnityEditor;
using UnityEngine;

/// <summary>
/// Makes it impossible to mistake an AutoSolveListener for a standalone interactable component.
/// </summary>
[CustomEditor(typeof(AutoSolveListener))]
public class AutoSolveListenerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox(
            "LISTENER — not a standalone interactable.\n" +
            "This component auto-solves its target part the moment the gate part solves (e.g. Disco Ball reacts to Party Lights).\n" +
            "Do NOT wire it for the player and do NOT give its part an interaction.",
            MessageType.Info);

        DrawDefaultInspector();
    }
}
