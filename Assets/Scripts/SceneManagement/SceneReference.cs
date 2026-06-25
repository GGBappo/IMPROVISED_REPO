using UnityEngine;

// im making this class to make our lives so much easier
[System.Serializable]
public class SceneReference
{
    [SerializeField] private Object sceneAsset;
    [SerializeField] private string sceneName = "";

    public string SceneName => sceneName;
}