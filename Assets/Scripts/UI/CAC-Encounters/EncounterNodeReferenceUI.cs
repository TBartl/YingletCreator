using Reactivity;

/// <summary>
/// This should be on every UI prefab created for a node
/// It primarily allows us to ensure only the most recent nodes are interactable
/// and to lower the opacity on old nodes
/// </summary>
public interface IEncounterNodeReferenceUI
{
	void SetReference(IEncounterInstance encounterInstance, int indexInHistory);
}

public class EncounterNodeReferenceUI : ReactiveBehaviour, IEncounterNodeReferenceUI, IUIInteractable
{
	private IEncounterInstance _encounterInstance;
	private int _indexInHistory;
	private Computed<bool> _interactable;

	public IReadOnlyObservable<bool> Interactable => _interactable;

	public void SetReference(IEncounterInstance encounterInstance, int indexInHistory)
	{
		_encounterInstance = encounterInstance;
		_indexInHistory = indexInHistory;
		_interactable = CreateComputed(ComputeInteractable);
	}

	bool ComputeInteractable()
	{
		return _indexInHistory > _encounterInstance.LastBlockingNode;
	}
}
