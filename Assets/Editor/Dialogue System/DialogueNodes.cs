using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Unity.GraphToolkit.Editor;

/// <summary>
/// A node that represents the starting point of the dialogue graph.
/// </summary>
[Serializable]
public class StartNode : Node
{
    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddOutputPort("out").Build();
    }
}

/// <summary>
/// A node that represents a dialogue in the dialogue graph.
/// </summary>
[Serializable]
public class DialogueNode : Node
{
    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort("in").Build();
        context.AddOutputPort("out").Build();

        context.AddInputPort<string>("Speaker Name").Build();
        context.AddInputPort<string>("Dialogue Text").Build();
        context.AddInputPort<Sprite>("Speaker Image").Build();
    }
}

/// <summary>
/// A node that represents a choice in the dialogue graph. Can in theory have multiple choices.
/// <br>Ports:</br>
/// <br></br>
/// - <b>Input:</b> "in" - The input port for the choice node.
/// <br></br>
/// - <b>Input:</b> "Speaker Name" - The input port for the speaker name of the choice node.
/// <br></br>
/// - <b>Input:</b> "Dialogue Text" - The input port for the dialogue text of the choice node.
/// <br></br>
/// - <b>Input:</b> "Choice text i" - The input port for the text of the Xth choice of the choice node.
/// <br></br>
/// - <b>Output:</b> "Choice i" - The output port for the Xth choice of the choice node.
/// This port will connect to the next node in the dialogue graph that corresponds to the Xth choice.
/// </summary>
[Serializable]
public class ChoiceNode : Node
{
    const string optionID = "portCount";
    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort("in").Build();

        context.AddInputPort<string>("Speaker Name").Build();
        context.AddInputPort<string>("Dialogue Text").Build();
        context.AddInputPort<Sprite>("Speaker Image").Build();

        var option = GetNodeOptionByName(optionID);
        option.TryGetValue(out int portCount);
        for (int i = 0; i < portCount; i++)
        {
            context.AddInputPort<string>($"Choice text {i + 1}").Build();
            context.AddOutputPort($"Choice {i + 1}").Build();
        }

    }

    protected override void OnDefineOptions(IOptionDefinitionContext context)
    {
        context.AddOption<int>(optionID).WithDefaultValue(2).Delayed();
    }
}


[Serializable]
public class ActionNode : Node
{
    const string actionOptionID = "actionType";

    protected override void OnDefineOptions(IOptionDefinitionContext context)
    {
        context.AddOption<ActionNodeType>(actionOptionID).WithDefaultValue(ActionNodeType.ShowDialouge);
    }

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort("in").Build();
        
        var option = GetNodeOptionByName(actionOptionID);
        option.TryGetValue(out ActionNodeType currentAction);

        if (currentAction == ActionNodeType.ChangeStartMenuState)
        {
            context.AddInputPort<StartMenuState>("Start Menu State").Build();
        }
        if (currentAction == ActionNodeType.ChangeDialogueBoxPosition)
        {
            context.AddInputPort<DialogueBoxPosition>("Dialogue Box Position").Build();
        }

        context.AddOutputPort("out").Build();
    }
}

/// <summary>
/// A node that represents the end of the dialogue graph. 
/// This node is not necessarily required, however it is a good practice to have one to signify the end of the dialogue.
/// <br>Ports:</br>
/// <br></br>
/// - <b>Input:</b> "in" - The input port for the end node. 
/// </summary>
[Serializable]
public class EndNode : Node
{
    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort("in").Build();
    }
}