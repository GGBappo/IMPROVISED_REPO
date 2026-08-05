using UnityEngine;

public class NPC : MonoBehaviour
{
	[SerializeField] private Sprite _npcSprite;
	[SerializeField] private RuntimeDialogueGraph _dialogueGraph;

	[SerializeField] private SpriteRenderer _spriteRenderer;

	public Sprite NPCSprite => _npcSprite;
	public RuntimeDialogueGraph DialogueGraph => _dialogueGraph;

	protected virtual void Awake()
	{
		if (_spriteRenderer == null)
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}

		ApplySprite();
	}

	protected virtual void OnValidate()
	{
		if (_spriteRenderer == null)
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}

		ApplySprite();
	}

	public virtual void Interact()
	{
        Debug.Log($"Interacting with NPC: {gameObject.name}");
		GameEvents.RequestDialogueStart(_dialogueGraph);
	}

	public virtual void SetDialogueGraph(RuntimeDialogueGraph dialogueGraph)
	{
		_dialogueGraph = dialogueGraph;
	}

	public virtual void SetSprite(Sprite npcSprite)
	{
		_npcSprite = npcSprite;
		ApplySprite();
	}

	protected void ApplySprite()
	{
		if (_spriteRenderer != null)
		{
			_spriteRenderer.sprite = _npcSprite;
		}
	}
}