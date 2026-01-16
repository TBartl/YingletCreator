using Reactivity;

public class ConfirmationSelection : ReactiveBehaviour, ISelectable
{
	private IConfirmationManager _confirmationManager;
	Computed<bool> _selected;
	public IReadOnlyObservable<bool> Selected => _selected;

	private void Awake()
	{
		_confirmationManager = Singletons.GetSingleton<IConfirmationManager>();
		_selected = CreateComputed(ComputeSelected);
	}

	private bool ComputeSelected()
	{
		return _confirmationManager.Current.Val != null;
	}
}
