using Character.Creator;
using Character.Data;
using Encounters.Runtime;
using Reactivity;
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class EncounterInstanceExtraData
{
	Observable<PoseId> _poseId = new();
	public PoseId PoseId { get => _poseId.Val; set => _poseId.Val = value; }

	Observable<bool> _mirror = new();
	public bool Mirror { get => _mirror.Val; set => _mirror.Val = value; }

	Observable<FullCharacterExpressions> _characterExpressions = new();
	public FullCharacterExpressions CharacterExpressions { get => _characterExpressions.Val; internal set => _characterExpressions.Val = value; }
}

public interface IEncounterInstance
{
	/// <summary>
	/// To be called after references to this are setup
	/// This allows us to differentiate between "we are watching an encounter start" and "we are catching up on an encounter"
	/// </summary>
	void Start();
	void ProgressToNode(IEncounterNode next);

	IReadOnlyObservable<IEncounterNode> CurrentNode { get; }

	/// <summary>
	/// Nodes can put arbitrary data here relating to their execution. For example:
	/// - PromptChoiceNodes may store their result
	/// - RollNodes may store the roll result
	/// This isn't particularly relevant for driving logic (the nodes themselves should be doing that)
	/// But it can be useful for observers like the UI
	/// </summary>
	IList<object> NodeResultData { get; }

	GameObject EncounterSource { get; }
	ICharacterRoot Character { get; }

	string CharacterName { get; }

	event Action OnFinished;

	IEncounterMemory Memory { get; }
	EncounterInstanceExtraData Data { get; }

	/// <summary>
	/// A history of all nodes visited during this encounter, in order
	/// </summary>
	IList<IEncounterNode> NodeHistory { get; }
}

public sealed class EncounterInstance : IEncounterInstance
{
	Observable<IEncounterNode> _currentNode = new();
	IList<IEncounterNode> _nodeHistory = new List<IEncounterNode>();
	IList<object> _nodeResultData = new List<object>();
	private EncounterGraph _encounterGraph;
	Lazy<string> _characterName;

	public GameObject EncounterSource { get; }
	public ICharacterRoot Character { get; }

	public IReadOnlyObservable<IEncounterNode> CurrentNode => _currentNode;

	public string CharacterName => _characterName.Value;

	public IEncounterMemory Memory { get; }
	public EncounterInstanceExtraData Data { get; private set; }

	public IList<object> NodeResultData => _nodeResultData;

	public IList<IEncounterNode> NodeHistory => _nodeHistory;

	public event Action OnFinished;

	public EncounterInstance(EncounterGraph encounterGraph, IEncounterMemory encounterMemory, GameObject encounterSource, ICharacterRoot character)
	{
		_encounterGraph = encounterGraph;
		this.Memory = encounterMemory;
		this.EncounterSource = encounterSource;
		this.Character = character;

		_characterName = new Lazy<string>(GetCharacterName);
		Data = new EncounterInstanceExtraData();
	}

	public void Start()
	{
		ProgressToNode(_encounterGraph.StartNode);
	}

	public void ProgressToNode(IEncounterNode next)
	{
		if (next == null)
		{
			OnFinished?.Invoke();
			return;
		}
		_nodeHistory.Add(next);
		_currentNode.Val = next;
		_currentNode.Val.Run(this);
	}


	private string GetCharacterName()
	{
		var dataRepo = Character.GetComponentInChildrenSafe<ICustomizationDataRepository>().CustomizationData;
		return dataRepo.Name.Val;
	}
}
