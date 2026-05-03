
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

	GameObject EncounterSource { get; }
	ICharacterRoot Character { get; }

	string CharacterName { get; }

	event Action OnFinished;

	EncounterInstanceExtraData Data { get; }
}

public sealed class EncounterInstance : IEncounterInstance
{

	Observable<IEncounterNode> _currentNode = new();
	IList<IEncounterNode> _nodeHistory = new List<IEncounterNode>();
	private EncounterGraph _encounterGraph;
	Lazy<string> _characterName;

	public GameObject EncounterSource { get; private set; }
	public ICharacterRoot Character { get; private set; }

	public IReadOnlyObservable<IEncounterNode> CurrentNode => _currentNode;

	public string CharacterName => _characterName.Value;

	public EncounterInstanceExtraData Data { get; private set; }

	public event Action OnFinished;

	public EncounterInstance(EncounterGraph encounterGraph, GameObject encounterSource, ICharacterRoot character)
	{
		_encounterGraph = encounterGraph;
		this.EncounterSource = encounterSource;
		this.Character = character;

		_characterName = new Lazy<string>(GetCharacterName);
		Data = new EncounterInstanceExtraData();
	}

	public void Start()
	{
		_currentNode.Val = _encounterGraph.StartNode;
		_nodeHistory.Add(_currentNode.Val);
		_currentNode.Val.Run(this);
	}

	public void ProgressToNode(IEncounterNode next)
	{
		if (next == null)
		{
			OnFinished?.Invoke();
			return;
		}
		_currentNode.Val = next;
		_nodeHistory.Add(_currentNode.Val);
		_currentNode.Val.Run(this);
	}


	private string GetCharacterName()
	{
		var dataRepo = Character.GetComponentInChildrenSafe<ICustomizationDataRepository>().CustomizationData;
		return dataRepo.Name.Val;
	}
}
