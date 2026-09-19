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

public interface IEncounterInstance : IDisposable
{
	/// <summary>
	/// To be called after references to this are setup
	/// This allows us to differentiate between "we are watching an encounter start" and "we are catching up on an encounter"
	/// </summary>
	void Start();
	void ProgressToNode(IEncounterNode next);

	IReadOnlyObservable<EncounterNodeVisitRecord> CurrentNode { get; }

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
	IRoom Room { get; }

	string FormattedCharacterName { get; }

	event Action OnFinished;

	IEncounterNetworking Networking { get; }
	IEncounterMemory Memory { get; }
	EncounterInstanceExtraData Data { get; }

	/// <summary>
	/// A history of all nodes visited during this encounter, in order
	/// Not reactive
	/// </summary>
	IList<EncounterNodeVisitRecord> NodeHistory { get; }

	int LastBlockingNode { get; }
}

public sealed class EncounterInstance : IEncounterInstance
{
	Observable<EncounterNodeVisitRecord> _currentNode = new();
	IList<EncounterNodeVisitRecord> _nodeHistory = new ObservableList<EncounterNodeVisitRecord>();
	IList<object> _nodeResultData = new List<object>();
	private EncounterGraph _encounterGraph;
	Lazy<string> _formattedCharacterName;
	Computed<int> _lastBlockingNode;

	public GameObject EncounterSource { get; }
	public ICharacterRoot Character { get; }
	public IRoom Room { get; }

	public IReadOnlyObservable<EncounterNodeVisitRecord> CurrentNode => _currentNode;

	public string FormattedCharacterName => _formattedCharacterName.Value;

	public IEncounterNetworking Networking { get; private set; }
	public IEncounterMemory Memory { get; }
	public EncounterInstanceExtraData Data { get; private set; }

	public IList<object> NodeResultData => _nodeResultData;

	public IList<EncounterNodeVisitRecord> NodeHistory => _nodeHistory;

	public int LastBlockingNode => _lastBlockingNode.Val;

	public event Action OnFinished;

	public EncounterInstance(EncounterGraph encounterGraph, IEncounterMemory encounterMemory, GameObject encounterSource, ICharacterRoot character)
	{
		_encounterGraph = encounterGraph;
		Memory = encounterMemory;
		EncounterSource = encounterSource;
		Room = character.GetComponentInChildrenSafe<ICharacterRoomDetector>().CurrentRoom.Val;
		Character = character;

		_formattedCharacterName = new Lazy<string>(GetCharacterName);
		Networking = new EncounterNetworking(this);
		Data = new EncounterInstanceExtraData();
		_lastBlockingNode = new Computed<int>(ComputeLastBlockingNode);
	}

	public void Dispose()
	{
		foreach (var record in _nodeHistory)
		{
			var disposable = record.VisitData as IDisposable;
			disposable?.Dispose();
		}
		_lastBlockingNode.Destroy();
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
		var visitData = next.GenerateVisitData(this);
		var record = new EncounterNodeVisitRecord(next, visitData);
		_nodeHistory.Add(record);

		_currentNode.Val = record;
		next.Run(this);
	}


	private string GetCharacterName()
	{
		var dataRepo = Character.GetComponentInChildrenSafe<ICustomizationDataRepository>().CustomizationData;
		var name = dataRepo.Name.Val;

		var characterClass = Character.GetComponentInChildrenSafe<IClassReference>().Class;


		var formattedCharacterName = $"<b><color={characterClass.TextColorHtml}>{name}</color></b>";
		return formattedCharacterName;
	}

	private int ComputeLastBlockingNode()
	{
		for (int i = _nodeHistory.Count - 2; i >= 0; i--)
		{
			if (_nodeHistory[i].Node.Blocking)
				return i;
		}
		return -1;
	}
}


public sealed class EncounterNodeVisitRecord
{
	public EncounterNodeVisitRecord(IEncounterNode node, IEncounterVisitData visitData)
	{
		Node = node;
		VisitData = visitData;
	}
	public IEncounterNode Node { get; }
	public IEncounterVisitData VisitData { get; }
}
