using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class RuntimeDialogueGraph : ScriptableObject
{
    public string EntryNodeID;
    public List<RuntimeDialogueNode> AllNodes = new List<RuntimeDialogueNode>();
}

[Serializable]
public class RuntimeDialogueNode
{
    public string NodeID;
    public string SpeakerName;
    public string DialogueText;
    public Sprite SpeakerImage;
    public List<ChoiceData> Choices = new List<ChoiceData>();
    public ActionData Action;
    public string NextNodeID;
}

[Serializable]
public class ChoiceData
{
    public string ChoiceText;
    public string DestinationNodeID;
}

[Serializable]
public class ActionData
{
    public ActionNodeType Action;

    public StartMenuState startMenuState;
    public DialogueBoxPosition dialogueBoxPosition;
}