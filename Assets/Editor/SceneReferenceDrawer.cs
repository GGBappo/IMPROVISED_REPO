using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SceneReference))]
public class SceneReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        SerializedProperty sceneAssetProp = property.FindPropertyRelative("sceneAsset");
        SerializedProperty sceneNameProp = property.FindPropertyRelative("sceneName");

        // this draws the box
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        sceneAssetProp.objectReferenceValue = EditorGUI.ObjectField(position, sceneAssetProp.objectReferenceValue, typeof(SceneAsset), false);

        // then i just steal the name
        if (sceneAssetProp.objectReferenceValue != null)
        {
            sceneNameProp.stringValue = sceneAssetProp.objectReferenceValue.name;
        }
        else
        {
            sceneNameProp.stringValue = "";
        }

        EditorGUI.EndProperty();
    }
}