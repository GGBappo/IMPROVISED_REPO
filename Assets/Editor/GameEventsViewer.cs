using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements; // Required for Toolbar elements
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public class GameEventsGraphWindow : EditorWindow
{
    private GameEventsGraphView graphView;
    private DropdownField filterDropdown;
    private ToolbarSearchField searchField;

    [MenuItem("Tools/Game Events Node Graph")]
    public static void OpenWindow()
    {
        var window = GetWindow<GameEventsGraphWindow>("Game Events Graph");
        window.minSize = new Vector2(800, 600);
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
        graphView.ScanAndGenerateGraph();
    }

    private void OnDisable()
    {
        if (graphView != null)
        {
            rootVisualElement.Remove(graphView);
        }
    }

    private void OnProjectChange()
    {
        if (graphView != null) graphView.ScanAndGenerateGraph();
    }

    private void ConstructGraphView()
    {
        graphView = new GameEventsGraphView { name = "Game Events Graph" };
        graphView.style.flexGrow = 1; // Dynamically fills all space below the toolbar
        rootVisualElement.Add(graphView);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();
        
        // --- KEY / LEGEND ---
        var keyLabel = new Label(" KEY: ") { style = { unityTextAlign = TextAnchor.MiddleLeft, unityFontStyleAndWeight = FontStyle.Bold, marginLeft = 5 } };
        toolbar.Add(keyLabel);

        // Purple Event Swatch
        var eventColor = new VisualElement { style = { backgroundColor = new Color(0.4f, 0.2f, 0.6f, 0.9f), width = 12, height = 12, marginTop = 5, marginLeft = 5, marginRight = 5 } };
        toolbar.Add(eventColor);
        toolbar.Add(new Label("Game Event") { style = { unityTextAlign = TextAnchor.MiddleLeft, marginTop = 2 } });

        // Dark Gray Script Swatch
        var scriptColor = new VisualElement { style = { backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.9f), width = 12, height = 12, marginTop = 5, marginLeft = 15, marginRight = 5 } };
        toolbar.Add(scriptColor);
        toolbar.Add(new Label("Script/Class") { style = { unityTextAlign = TextAnchor.MiddleLeft, marginTop = 2 } });

        // SPACER (Pushes everything after it to the right side of the screen)
        var spacer = new VisualElement { style = { flexGrow = 1 } };
        toolbar.Add(spacer);

        // --- SORT / FILTER DROPDOWN ---
        var sortLabel = new Label("Highlight Type: ") { style = { unityTextAlign = TextAnchor.MiddleLeft, unityFontStyleAndWeight = FontStyle.Bold, marginTop = 2 } };
        toolbar.Add(sortLabel);

        filterDropdown = new DropdownField(new List<string> { "Show All", "Game Events Only", "Scripts Only" }, 0);
        filterDropdown.RegisterValueChangedCallback(evt => {
            if (searchField != null) searchField.value = ""; // Clear search box when using dropdown
            graphView.FilterNodesByType(evt.newValue);
        });
        filterDropdown.style.width = 140;
        toolbar.Add(filterDropdown);

        // --- SEARCH BAR ---
        searchField = new ToolbarSearchField();
        searchField.RegisterValueChangedCallback(evt => {
            if (filterDropdown != null) filterDropdown.index = 0; // Reset dropdown when typing
            graphView.SearchNodes(evt.newValue);
        });
        searchField.style.width = 200;
        searchField.style.marginLeft = 10;
        toolbar.Add(searchField);

        // Insert at index 0 so it stays locked to the very top of the window
        rootVisualElement.Insert(0, toolbar); 
    }
}

public class GameEventsGraphView : GraphView
{
    public List<Node> EventNodes = new List<Node>();
    public List<Node> ScriptNodes = new List<Node>();

    public GameEventsGraphView()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
    }

    public void ScanAndGenerateGraph()
    {
        DeleteElements(graphElements.ToList());
        EventNodes.Clear();
        ScriptNodes.Clear();

        var eventReferences = new Dictionary<string, HashSet<string>>();
        string[] files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            if (fileName == "GameEventsGraphWindow.cs" || fileName == "GameEvents.cs" || fileName == "GameEventsViewer.cs") continue;

            string[] lines = File.ReadAllLines(file);
            foreach (string line in lines)
            {
                if (line.Contains("GameEvents."))
                {
                    Match match = Regex.Match(line, @"GameEvents\.([A-Za-z0-9_]+)");
                    if (match.Success)
                    {
                        string eventName = match.Groups[1].Value;
                        if (!eventReferences.ContainsKey(eventName))
                        {
                            eventReferences[eventName] = new HashSet<string>();
                        }
                        eventReferences[eventName].Add(fileName);
                    }
                }
            }
        }

        DrawForceDirectedGraph(eventReferences);
    }

    private void DrawForceDirectedGraph(Dictionary<string, HashSet<string>> eventData)
    {
        Dictionary<string, Node> allSpawnedNodes = new Dictionary<string, Node>();
        List<Node> physicsNodes = new List<Node>();
        List<Edge> physicsEdges = new List<Edge>();
        
        Dictionary<Node, Vector2> logicalPositions = new Dictionary<Node, Vector2>();

        foreach (var kvp in eventData)
        {
            string eventName = kvp.Key;
            
            if (!allSpawnedNodes.ContainsKey(eventName))
            {
                Node eventNode = CreateCompactNode(eventName, true);
                allSpawnedNodes.Add(eventName, eventNode);
                physicsNodes.Add(eventNode);
                
                logicalPositions[eventNode] = new Vector2(UnityEngine.Random.Range(-500f, 500f), UnityEngine.Random.Range(-500f, 500f));
                AddElement(eventNode);
            }

            foreach (var scriptName in kvp.Value)
            {
                if (!allSpawnedNodes.ContainsKey(scriptName))
                {
                    Node scriptNode = CreateCompactNode(scriptName, false);
                    allSpawnedNodes.Add(scriptName, scriptNode);
                    physicsNodes.Add(scriptNode);
                    
                    logicalPositions[scriptNode] = new Vector2(UnityEngine.Random.Range(-500f, 500f), UnityEngine.Random.Range(-500f, 500f));
                    AddElement(scriptNode);
                }

                Edge edge = ConnectNodes(allSpawnedNodes[eventName], allSpawnedNodes[scriptName]);
                if (edge != null)
                {
                    physicsEdges.Add(edge);
                }
            }
        }

        ApplyForceDirectedLayout(physicsNodes, physicsEdges, logicalPositions);
    }

    private Node CreateCompactNode(string title, bool isEvent)
    {
        var node = new Node { title = title };

        if (isEvent)
        {
            var outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            outputPort.portName = ""; 
            node.outputContainer.Add(outputPort);
            node.titleContainer.style.backgroundColor = new Color(0.4f, 0.2f, 0.6f, 0.9f); 
            EventNodes.Add(node); // Track for filtering
        }
        else
        {
            var inputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            inputPort.portName = ""; 
            node.inputContainer.Add(inputPort);
            node.titleContainer.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.9f); 
            ScriptNodes.Add(node); // Track for filtering
        }

        node.RefreshExpandedState();
        node.RefreshPorts();
        return node;
    }

    private Edge ConnectNodes(Node outputNode, Node inputNode)
    {
        if (outputNode.outputContainer.childCount == 0 || inputNode.inputContainer.childCount == 0) return null;

        var outputPort = outputNode.outputContainer[0] as Port;
        var inputPort = inputNode.inputContainer[0] as Port;

        var edge = new Edge { output = outputPort, input = inputPort };
        edge.input.Connect(edge);
        edge.output.Connect(edge);
        AddElement(edge);
        return edge;
    }

    private void ApplyForceDirectedLayout(List<Node> nodes, List<Edge> edges, Dictionary<Node, Vector2> positions)
    {
        int iterations = 150; 
        float optimalDistance = 250f; 
        float temperature = 200f; 

        Dictionary<Node, Vector2> displacements = new Dictionary<Node, Vector2>();

        for (int i = 0; i < iterations; i++)
        {
            displacements.Clear();
            foreach (var node in nodes) displacements[node] = Vector2.zero;

            for (int v = 0; v < nodes.Count; v++)
            {
                for (int u = 0; u < nodes.Count; u++)
                {
                    if (v == u) continue;
                    
                    Vector2 delta = positions[nodes[v]] - positions[nodes[u]];
                    float dist = delta.magnitude;
                    
                    if (dist < 0.1f) 
                    {
                        delta = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
                        dist = delta.magnitude;
                    }

                    float force = (optimalDistance * optimalDistance) / dist;
                    displacements[nodes[v]] += (delta.normalized * force);
                }
            }

            foreach (var edge in edges)
            {
                Node nodeV = edge.output.node as Node;
                Node nodeU = edge.input.node as Node;
                if (nodeV == null || nodeU == null) continue;

                Vector2 delta = positions[nodeV] - positions[nodeU];
                float dist = delta.magnitude;

                if (dist > 0)
                {
                    float force = (dist * dist) / optimalDistance;
                    Vector2 dispForce = delta.normalized * force;

                    displacements[nodeV] -= dispForce;
                    displacements[nodeU] += dispForce;
                }
            }

            foreach (var node in nodes)
            {
                Vector2 disp = displacements[node];
                float dist = disp.magnitude;

                if (dist > 0)
                {
                    positions[node] += (disp / dist) * Mathf.Min(dist, temperature);
                }
            }

            temperature *= 0.95f;
        }

        foreach (var node in nodes)
        {
            node.SetPosition(new Rect(positions[node], Vector2.zero));
        }
    }

    // --- NEW FILTERING LOGIC --- //

    public void FilterNodesByType(string filterType)
    {
        bool showEvents = filterType == "Show All" || filterType == "Game Events Only";
        bool showScripts = filterType == "Show All" || filterType == "Scripts Only";

        foreach (var node in EventNodes) node.style.opacity = showEvents ? 1f : 0.1f;
        foreach (var node in ScriptNodes) node.style.opacity = showScripts ? 1f : 0.1f;

        UpdateEdgeOpacities();
    }

    public void SearchNodes(string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            foreach (var node in EventNodes.Concat(ScriptNodes)) node.style.opacity = 1f;
            UpdateEdgeOpacities();
            return;
        }

        query = query.ToLower();
        foreach (var node in EventNodes.Concat(ScriptNodes))
        {
            node.style.opacity = node.title.ToLower().Contains(query) ? 1f : 0.1f;
        }
        
        UpdateEdgeOpacities();
    }

    private void UpdateEdgeOpacities()
    {
        foreach (var edge in this.edges.ToList())
        {
            var outNode = edge.output.node as Node;
            var inNode = edge.input.node as Node;

            if (outNode != null && inNode != null)
            {
                // If either node attached to the wire is faded out, fade the wire too
                bool isVisible = outNode.style.opacity.value > 0.5f && inNode.style.opacity.value > 0.5f;
                edge.style.opacity = isVisible ? 1f : 0.03f;
            }
        }
    }
}