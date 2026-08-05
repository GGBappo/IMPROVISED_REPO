using System;
using UnityEditor;
using UnityEngine;
using Unity.GraphToolkit.Editor;

[Graph(AssetExtension)]
[Serializable]
public class DialogueGraph : Graph
{
    // from what i could see this file extension doesn't actually effect the file or graph view itself
    // i think if i were to create an extension for the dialogue stuff then it would compile
    // only those with the extension 
    // i think thats pretty obvious but okay
    public const string AssetExtension = "dialogue";
    
    
    [MenuItem("Assets/Create/Graph Toolkit Samples/DialogueGraph", false)]
    static void CreateAssetFile()
    {
        // im guessing that this is how the graph actually gets made?
        // there doesnt seem to be much information about this function
        // so i'll take it as is!
        GraphDatabase.PromptInProjectBrowserToCreateNewAsset<DialogueGraph>();
    }
}
