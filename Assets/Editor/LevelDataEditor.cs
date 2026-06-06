using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))] 
public class LevelDataEditor : Editor
{
    private PreviewRenderUtility previewRenderUtility;
    private GameObject previewInstance;
    private Vector2 dragRotation;

    private void OnEnable()
    {
        if (previewRenderUtility == null)
        {
            previewRenderUtility = new PreviewRenderUtility();
            
            // adds basic camera for the preview window
            previewRenderUtility.camera.transform.position = new Vector3(0, 0, -5);
            previewRenderUtility.camera.transform.rotation = Quaternion.identity;
            previewRenderUtility.camera.fieldOfView = 30f;
            previewRenderUtility.camera.nearClipPlane = 0.01f;
            previewRenderUtility.camera.farClipPlane = 100f;
            
            // lighting so it isnt so dark
            previewRenderUtility.lights[0].intensity = 1.5f;
            previewRenderUtility.lights[0].transform.rotation = Quaternion.Euler(30f, 30f, 0f);
        }
    }

    private void OnDisable()
    {
        // memory clean up!!!!!!
        if (previewRenderUtility != null)
        {
            previewRenderUtility.Cleanup();
            previewRenderUtility = null;
        }

        if (previewInstance != null)
        {
            DestroyImmediate(previewInstance);
        }
    }

    public override bool HasPreviewGUI()
    {
        LevelData data = (LevelData)target;
        // only shows the preview if a bomb prefab is actually assigned
        return data.bombPrefab != null; 
    }

    // handles the actual drawing and interacting of the preview
    public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
    {
        LevelData data = (LevelData)target;
        if (data.bombPrefab == null) return;

        // handle mouse drag for rotation
        dragRotation = Drag2D(dragRotation, r);

        if (Event.current.type == EventType.Repaint)
        {
            previewRenderUtility.BeginPreview(r, background);

            // instantiate a temporary, invisible clone of the prefab for the preview
            if (previewInstance == null)
            {
                previewInstance = Instantiate(data.bombPrefab, Vector3.zero, Quaternion.identity);
                // this prevents the clone from showing up in the scene hierarchy
                previewInstance.hideFlags = HideFlags.HideAndDontSave;
                previewRenderUtility.AddSingleGO(previewInstance); 
            }

            // apply our dragged rotation to the instance
            previewInstance.transform.rotation = Quaternion.Euler(dragRotation.y, dragRotation.x, 0);

            // render the camera view and draw it to the inspector rectangle
            previewRenderUtility.camera.Render();
            previewRenderUtility.EndAndDrawPreview(r);
        }
    }

    // helper method to handle mouse dragging math
    private Vector2 Drag2D(Vector2 scrollPosition, Rect position)
    {
        int controlID = GUIUtility.GetControlID("Slider".GetHashCode(), FocusType.Passive);
        Event current = Event.current;
        
        switch (current.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                if (position.Contains(current.mousePosition) && position.width > 50f)
                {
                    GUIUtility.hotControl = controlID;
                    current.Use();
                    EditorGUIUtility.SetWantsMouseJumping(1);
                }
                break;
            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlID)
                {
                    GUIUtility.hotControl = 0;
                }
                EditorGUIUtility.SetWantsMouseJumping(0);
                break;
            case EventType.MouseDrag:
                if (GUIUtility.hotControl == controlID)
                {
                    scrollPosition -= current.delta * (current.shift ? 3f : 1f);
                    scrollPosition.y = Mathf.Clamp(scrollPosition.y, -90f, 90f);
                    current.Use();
                    GUI.changed = true;
                }
                break;
        }
        return scrollPosition;
    }

    public override GUIContent GetPreviewTitle()
    {
        LevelData data = (LevelData)target;
        
        if (data.bombPrefab != null)
        {
            return new GUIContent(data.bombPrefab.name + " Preview");
        }
        
        return new GUIContent("Bomb Preview");
    }
}