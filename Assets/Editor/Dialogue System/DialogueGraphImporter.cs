using UnityEngine;
using UnityEditor.AssetImporters;
using Unity.GraphToolkit.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


// THESE COMMENTS WILL BE FOR MY OWN SANITY
// so from what im getting thus far
// like i previously mentioned in another script we dont have a legit runtime thing yet
// we also have no way of populaing the RuntimeDialogueGraph thingy
[ScriptedImporter(1, DialogueGraph.AssetExtension)]
public class DialogueGraphImporter : ScriptedImporter
{
    // this is ran whenever an asset is imported
    // "imported" here means saving, creating, and loading our graph
    // just to clarify, as you can see its an override function
    // this function is already a needed function from ScriptedImporter
    public override void OnImportAsset(AssetImportContext ctx)
    {
        // this is just getting the path of the acc graph
        DialogueGraph editorGraph = GraphDatabase.LoadGraphForImporter<DialogueGraph>(ctx.assetPath);
        // THIS is getting and instantiaing an instance of our runtime scriptable object
        // note that this is the GRAPH not NODE runtime (if you're confused go to the scriptable object code you'll see what i mean!)
        RuntimeDialogueGraph runtimeGraph = ScriptableObject.CreateInstance<RuntimeDialogueGraph>();

        // here we're mapping all the node IDs in order to interact with them
        // INode is a built in interface that comes with the Unity.GraphToolkit.Editor package
        // the docs basically have it the same thing as a normal node, which is pretty obv
        // given that its an interface FOR nodes!
        var nodeIDMap =  new Dictionary<INode, string>();

        // in this foreach loop we'll be manually assigning each node with its own unique ID
        // this is done because apparently there is no thing that does that in the API according
        // to the video and docs so ya!
        foreach (var node in editorGraph.GetNodes())
        {
            nodeIDMap[node] = Guid.NewGuid().ToString();
        }

        // if we break it down slowly this makes sense!
        // on import we'd probably wanna find the beginning of the nodes right?
        // first we'll get the start node (this class/type was defined in our Dialogue Nodes script!)
        var startNode = editorGraph.GetNodes().OfType<StartNode>().FirstOrDefault();

        // once found, we'll assign it to the entry node ID
        // dubbed "EntryNodeID" in the runtime scriptable object
        if (startNode != null)
        {
            // note that entry port is a port that is coming OUT of the start node
            // hence why we're calling GetOutputPorts() and not GetInputPorts()
            // later on you'll see that in the end node we do the opposite since we're recieving the data from the last node before the end node.
            var entryPort = startNode.GetOutputPorts()
                .FirstOrDefault()
                .firstConnectedPort;
            
            if (entryPort != null)
            {
                runtimeGraph.EntryNodeID = nodeIDMap[entryPort.GetNode()];
            }
        }

        // then we'll loop through all the nodes once more
        // if we encounter a start OR end node then we'll skip it!
        foreach (var node in editorGraph.GetNodes())
        {
            if (node is StartNode || node is EndNode)
            {
                continue;
            }

            // when we DONT encounter a start or end node, we'll create a new instance of our runtime node
            var runtimeNode = new RuntimeDialogueNode {NodeID = nodeIDMap[node]};
            // if this node is a dialogue node we'll process it!
            // if not then we'll be processing each node as is
            if (node is DialogueNode dialogueNode)
            {
                ProcessDialogueNode(dialogueNode, runtimeNode, nodeIDMap);
            }
            else if (node is ChoiceNode choiceNode)
            {
                ProcessChoiceNode(choiceNode, runtimeNode, nodeIDMap);
            } 
            else if (node is ActionNode actionNode){
                ProcessActionNode(actionNode, runtimeNode, nodeIDMap);
            }

            // finally we'll add our runtime node to the runtime graph
            runtimeGraph.AllNodes.Add(runtimeNode);
        }

        // finally we'll add our runtime graph to the asset database and set it as the main object
        ctx.AddObjectToAsset("RuntimeDialogueGraph", runtimeGraph);
        ctx.SetMainObject(runtimeGraph);
    }

    // this function is used to process the dialogue node and assign its values to the runtime node
    // we'll be using this in the foreach loop in the OnImportAsset function to process each dialogue node and assign its values to the runtime node
    private void ProcessDialogueNode(DialogueNode node, RuntimeDialogueNode runtimeNode, Dictionary<INode, string> nodeIDMap)
    {
        // here we're getting the values of our ports pretty easy stuff here
        runtimeNode.SpeakerName = GetPortValue<string>(node.GetInputPortByName("Speaker Name"));
        runtimeNode.DialogueText = GetPortValue<string>(node.GetInputPortByName("Dialogue Text"));
        runtimeNode.SpeakerImage = GetPortValue<Sprite>(node.GetInputPortByName("Speaker Image"));

        // now the next most important part is getting the ID of the next node/the node that it is connected to
        // this is done by getting the output port of the current node and checking if it is connected to another node
        var nextNodePort = node.GetOutputPortByName("out")?.firstConnectedPort; // we keep this as nullable in the event it isnt connected
        if (nextNodePort != null)
        {
            runtimeNode.NextNodeID = nodeIDMap[nextNodePort.GetNode()];
        }
    }

    private void ProcessChoiceNode(ChoiceNode node, RuntimeDialogueNode runtimeNode, Dictionary<INode, string> nodeIDMap)
    {
        runtimeNode.SpeakerName = GetPortValue<string>(node.GetInputPortByName("Speaker Name"));
        runtimeNode.DialogueText = GetPortValue<string>(node.GetInputPortByName("Dialogue Text"));
        runtimeNode.SpeakerImage = GetPortValue<Sprite>(node.GetInputPortByName("Speaker Image"));

        var choiceOutputPorts = node.GetOutputPorts().Where(p => p.name.StartsWith("Choice "));

        foreach (var outputPort in choiceOutputPorts)
        {
            var index = outputPort.name.Substring("Choice ".Length);
            var textPort = node.GetInputPortByName($"Choice text {index}") ?? node.GetInputPortByName($"Choice Text {index}");

            var choiceData = new ChoiceData
            {
                ChoiceText = GetPortValue<string>(textPort),
                DestinationNodeID = outputPort.firstConnectedPort != null ? nodeIDMap[outputPort.firstConnectedPort.GetNode()] : null
            };

            runtimeNode.Choices.Add(choiceData);
        }
    }


    private void ProcessActionNode(ActionNode node, RuntimeDialogueNode runtimeNode, Dictionary<INode, string> nodeIDMap){
        // here we're getting the value of the action port and assigning it to the runtime node's action data
        var option = node.GetNodeOptionByName("actionType");
        option.TryGetValue(out ActionNodeType actionType);

        var actionData = new ActionData { Action = actionType };

        if (actionType == ActionNodeType.ChangeStartMenuState)
        {
            var startMenuStatePort = node.GetInputPortByName("Start Menu State");
            actionData.startMenuState = GetPortValue<StartMenuState>(startMenuStatePort);
        }
        if (actionType == ActionNodeType.ChangeDialogueBoxPosition)
        {
            var dialogueBoxPositionPort = node.GetInputPortByName("Dialogue Box Position");
            actionData.dialogueBoxPosition = GetPortValue<DialogueBoxPosition>(dialogueBoxPositionPort);
        }

        runtimeNode.Action = actionData;

        var nextNodePort = node.GetOutputPortByName("out")?.firstConnectedPort; 
        if (nextNodePort != null)
        {
            runtimeNode.NextNodeID = nodeIDMap[nextNodePort.GetNode()];
        }
    }

    // this function is a generic function which gets the value of the port
    // this function checks whether the port is connected to a variable in the blackboard or not
    // if not, it takes the value that is typed in manually in the port itself
    // we use generics since we literally have no idea what the value of the port could be!
    private T GetPortValue<T>(IPort port)
    {
        if (port == null){
            return default;
        }

        // here we're checking if our port is connected
        // to a variable from the blackboard which if it is
        // we return it!
        if (port.isConnected)
        {
            if (port.firstConnectedPort != null && port.firstConnectedPort.TryGetValue(out T connectedValue))
            {
                return connectedValue;
            }

            if (port.firstConnectedPort.GetNode() is IVariableNode variableNode)
            {
                if (variableNode.variable.TryGetDefaultValue(out T value))
                {
                    return value;
                }
            }
        }

        if (port.TryGetValue(out T directValue))
        {
            return directValue;
        }

        return default;
    }
}
