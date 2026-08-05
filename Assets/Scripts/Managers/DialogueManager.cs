using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using static RuntimeSettings;

public enum DialogueBoxPosition
{
    Default,
    Up
}

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue Graph")]
    public RuntimeDialogueGraph runtimeDialogueGraph;

    [Header("Dialogue UI")]
    public GameObject dialogueUIPrefab;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public Image dialogueBox;
    public Button choiceButtonPrefab;
    public Transform choiceButtonContainer;
    public Image dialogueSpeakerImage;

    [Header("Dialogue Settings")]
    public Vector2 dialogueUIDefaultPosition;
    public Vector2 dialogueUIUpPosition;

    private Dictionary<string, RuntimeDialogueNode> _nodeLookup = new Dictionary<string, RuntimeDialogueNode>();
    private RuntimeDialogueNode currentNode;
    private bool isPrinting = false;
    private Tween _textPrint;
    private GlobalStateType _stateBeforeDialogue = GlobalStateType.Active;
    private bool _hasStoredState;

    private void OnEnable()
    {
        GameEvents.OnRequestShowDialogueUI += ShowDialogueUI;
        GameEvents.OnRequestHideDialogueUI += HideDialogueUI;
        GameEvents.OnRequestDialogueStart += StartDialogue;
        GameEvents.OnDialogueButtonPressed += AdvanceDialogue;
        GameEvents.OnRequestDialogueEnd += EndDialogue;
        GameEvents.OnGlobalStateChanged += TrackState;
        GameEvents.OnDialogueBoxMove += MoveDialogueBoxPosition;
    }

    private void OnDisable()
    {
        GameEvents.OnRequestShowDialogueUI -= ShowDialogueUI;
        GameEvents.OnRequestHideDialogueUI -= HideDialogueUI;
        GameEvents.OnRequestDialogueStart -= StartDialogue;
        GameEvents.OnDialogueButtonPressed -= AdvanceDialogue;
        GameEvents.OnRequestDialogueEnd -= EndDialogue;
        GameEvents.OnGlobalStateChanged -= TrackState;
        GameEvents.OnDialogueBoxMove -= MoveDialogueBoxPosition;
    }

    private void Start()
    {
        RebuildNodeLookup();
        dialogueBox.rectTransform.anchoredPosition = dialogueUIDefaultPosition;
    }

    private void Update()
    {
        if (dialogueUIPrefab.activeInHierarchy && currentNode != null) // this will be changed to utilize states sooner or later
        {
            if (Input.GetMouseButtonDown(0))
            {
                AdvanceDialogue();
            }
        }
    }

    private void ShowDialogueUI()
    {
        dialogueUIPrefab.SetActive(true);
        
    }
    private void HideDialogueUI()
    {
        dialogueUIPrefab.SetActive(false);
    }
    
    private void MoveDialogueBoxPosition(DialogueBoxPosition dialogueBoxPosition)
    {
        Debug.Log($"[DialogueManager] Moving Dialogue box to {dialogueBoxPosition} position");
        switch (dialogueBoxPosition)
        {
            case DialogueBoxPosition.Default:
                dialogueBox.rectTransform.DOAnchorPos(dialogueUIDefaultPosition, defaultTweenDuration).SetEase(Ease.OutSine);
                break;
            case DialogueBoxPosition.Up:
                Debug.Log(dialogueUIUpPosition);
                dialogueBox.rectTransform.DOAnchorPos(dialogueUIUpPosition, defaultTweenDuration).SetEase(Ease.OutSine);
                break;
        }
    }
    /// <summary>
    /// This method rebuilds the flat list of nodes by mapping them to a dictionary using their IDs.
    /// </summary>
    private void RebuildNodeLookup()
    {
        _nodeLookup.Clear();

        if (runtimeDialogueGraph == null || runtimeDialogueGraph.AllNodes == null)
        {
            return;
        }

        // populate the node lookup dictionary for quick access to nodes by their ID
        foreach (var node in runtimeDialogueGraph.AllNodes)
        {
            if (!string.IsNullOrEmpty(node.NodeID))
            {
                _nodeLookup[node.NodeID] = node;
            }
        }
    }


    /// <summary>
    /// This method starts the dialogue from the first node in the dictionary.
    /// </summary>
    /// <param name="dialogueGraph"></param>
    /// <param name="nodeID"></param>
    public void StartDialogue(RuntimeDialogueGraph dialogueGraph, string nodeID = null)
    {
        // it is possible to set a runtime dialogue in the inspector for this manager
        // im thinking a tutorial would be a good implemenation for this
        // however, if a runtime dialogue graph is provided (which most of the time will be the case)
        // then we'll set it and rebuild the nodes
        if (dialogueGraph != null && dialogueGraph != runtimeDialogueGraph)
        {
            runtimeDialogueGraph = dialogueGraph;
            RebuildNodeLookup();
        }

        if (runtimeDialogueGraph == null)
        {
            EndDialogue();
            return;
        }

        if (_nodeLookup.Count == 0)
        {
            RebuildNodeLookup();
        }

        // if we arent given a nodeID to start from, we'll assume it from the entry node
        if (string.IsNullOrEmpty(nodeID))
        {
            nodeID = runtimeDialogueGraph.EntryNodeID;
        }

        // if nodeID ISNT empty, we'll start from it
        if (!string.IsNullOrEmpty(nodeID))
        {
            ShowNode(nodeID);
        }

        else if (currentNode != null && !string.IsNullOrEmpty(currentNode.NextNodeID))
        {
            ShowNode(currentNode.NextNodeID);
        }
        else
        {
            EndDialogue();
        }
    }

    public void StartDialogue(string nodeID)
    {
        StartDialogue(runtimeDialogueGraph, nodeID);
    }

    public void AdvanceDialogue()
    {
        if (_textPrint != null && _textPrint.IsActive() && !_textPrint.IsComplete())
        {
            _textPrint.Complete(); 
        
            dialogueText.maxVisibleCharacters = 99999; 
        
            return; 
        }
        if (currentNode == null)
        {
            StartDialogue(runtimeDialogueGraph, runtimeDialogueGraph != null ? runtimeDialogueGraph.EntryNodeID : null);
            return;
        }

        if (currentNode.Choices != null && currentNode.Choices.Count > 0)
        {
            return;
        }

        if (!string.IsNullOrEmpty(currentNode.NextNodeID))
        {
            ShowNode(currentNode.NextNodeID);
        }
        else
        {
            EndDialogue();
        }
    }

    private void ShowNode(string nodeID)
    {
        if (_textPrint != null)
        {
            _textPrint.Kill(); 
        }

        foreach (Transform child in choiceButtonContainer)
        {
            Destroy(child.gameObject);
        }

        if (!_hasStoredState)
        {
            _hasStoredState = true;
            GameEvents.GlobalStateChanged(GlobalStateType.Dialogue);
        }

        if (!_nodeLookup.ContainsKey(nodeID))
        {
            EndDialogue();
            return;
        }

        currentNode = _nodeLookup[nodeID];
        dialogueUIPrefab.SetActive(true);

        if (speakerNameText != null)
        {
            speakerNameText.text = currentNode.SpeakerName;
        }

        if (dialogueSpeakerImage != null)
        {
            dialogueSpeakerImage.sprite = currentNode.SpeakerImage;
        }

        if (dialogueText != null)
        {
            dialogueText.text = currentNode.DialogueText;
            dialogueText.maxVisibleCharacters = 0;

            int totalCharacters = currentNode.DialogueText.Length;
            float printDuration = totalCharacters * 0.02f;

            _textPrint = DOTween.To(() => dialogueText.maxVisibleCharacters, x => dialogueText.maxVisibleCharacters = x, totalCharacters, printDuration)
                .SetEase(Ease.Linear) 
                .OnComplete(() => 
                {
                    if (currentNode.Choices.Count > 0)
                    {
                        foreach (var choice in currentNode.Choices)
                        {
                            var choiceData = choice;
                            var button = Instantiate(choiceButtonPrefab, choiceButtonContainer);

                            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

                            if (buttonText != null)
                            {
                                buttonText.text = choiceData.ChoiceText;
                            }

                            if (button != null)
                            {
                                button.onClick.AddListener(() =>
                                {
                                    if (!string.IsNullOrEmpty(choiceData.DestinationNodeID))
                                    {
                                        ShowNode(choiceData.DestinationNodeID);
                                    }
                                    else
                                    {
                                        EndDialogue();
                                    }
                                });
                            }
                        }
                    }
                    var choiceButtonContainerLayout = choiceButtonContainer.GetComponent<HorizontalLayoutGroup>();
                    LayoutRebuilder.ForceRebuildLayoutImmediate(choiceButtonContainerLayout.GetComponent<RectTransform>());

                });
        }
        

        ExecuteNodeAction(currentNode.Action);

        if (string.IsNullOrEmpty(currentNode.DialogueText) && currentNode.Choices.Count == 0)
        {
            if (!string.IsNullOrEmpty(currentNode.NextNodeID))
            {
                ShowNode(currentNode.NextNodeID);
            }
            else
            {
                EndDialogue();
            }
            return;
        }
        
    }

    private void ExecuteNodeAction(ActionData actionData)
    {
        if (actionData == null)
        {
            return;
        }

        switch (actionData.Action)
        {
            case ActionNodeType.ShowDialouge:
                GameEvents.RequestShowDialogueUI();
                break;
            case ActionNodeType.HideDialouge:
                GameEvents.RequestHideDialogueUI();
                break;
            case ActionNodeType.SpawnLatestAssignment:
                GameEvents.RequestLatestAssignmentFolderSpawn();
                break;
            case ActionNodeType.ChangeStartMenuState:
                GameEvents.StartMenuStateChanged(actionData.startMenuState);
                break;
            case ActionNodeType.ChangeDialogueBoxPosition:
                GameEvents.RequestDialogueBoxMove(actionData.dialogueBoxPosition);
                break;
            default:
                break;
        }
    }

    public void EndDialogue()
    {
        dialogueUIPrefab.SetActive(false);
        currentNode = null;
        _hasStoredState = false;

        foreach (Transform child in choiceButtonContainer)
        {
            Destroy(child.gameObject);
        }

        GameEvents.GlobalStateChanged(_stateBeforeDialogue);
    }

    private void TrackState(GlobalStateType newState)
    {
        if (newState != GlobalStateType.Dialogue)
        {
            _stateBeforeDialogue = newState;
        }
    }
}
