using Reactivity;

/// <summary>
/// This should be on every UI prefab created for a node
/// It primarily allows us to ensure only the most recent nodes are interactable
/// and to lower the opacity on old nodes
/// </summary>
public interface IEncounterNodeReferenceUI
{
	void SetReference(IEncounterInstance encounterInstance, EncounterNodeVisitRecord record, int indexInHistory);

	IEncounterInstance EncounterInstance { get; }
	EncounterNodeVisitRecord Record { get; }
	int IndexInHistory { get; }
}

public class EncounterNodeReferenceUI : ReactiveBehaviour, IEncounterNodeReferenceUI, IUIInteractable
{
	private IEncounterInstance _encounterInstance;
	private EncounterNodeVisitRecord _record;
	private int _indexInHistory;
	private Computed<bool> _interactable;

	public IReadOnlyObservable<bool> Interactable => _interactable;

	public IEncounterInstance EncounterInstance => _encounterInstance;
	public int IndexInHistory => _indexInHistory;
	public EncounterNodeVisitRecord Record => _record;

	public void SetReference(IEncounterInstance encounterInstance, EncounterNodeVisitRecord record, int indexInHistory)
	{
		_encounterInstance = encounterInstance;
		_record = record;
		_indexInHistory = indexInHistory;
		_interactable = CreateComputed(ComputeInteractable);
	}

	bool ComputeInteractable()
	{
		return _indexInHistory > _encounterInstance.LastBlockingNode;
	}
}
