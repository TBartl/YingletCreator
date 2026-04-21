using Reactivity;

public class ModalCharacterPickerSelection : ReactiveBehaviour, ISelectable
{
	private IModalCharacterPickerManager _manager;
	private Computed<bool> _selected;

	public IReadOnlyObservable<bool> Selected => _selected;

	private void Awake()
	{
		_manager = Singletons.GetSingleton<IModalCharacterPickerManager>();
		_selected = CreateComputed(ComputeSelected);
	}

	private bool ComputeSelected()
	{
		return _manager.Current.Val != null;
	}
}
