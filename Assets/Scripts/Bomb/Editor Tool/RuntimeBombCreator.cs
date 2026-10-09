using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class RuntimeBombCreatorGraph : ScriptableObject
{
    public string EntryNodeID;
    public List<RuntimeDialogueNode> AllNodes = new List<RuntimeDialogueNode>();
}