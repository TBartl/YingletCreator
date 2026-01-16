using Reactivity;
public interface IMenuButtonTextSelection : ISelectable
{
	IMenuButton HoveredMenuButton { get; }
}
public class MenuButtonTextSelection : ReactiveBehaviour, IMenuButtonTextSelection
{
	private IUiHoverManager _uiHoverManager;
	private Computed<IMenuButton> _hoveredMenuButton;
	private Computed<bool> _selected;

	public IReadOnlyObservable<bool> Selected => _selected;
	public IMenuButton HoveredMenuButton => _hoveredMenuButton.Val;

	private void Awake()
	{
		_uiHoverManager = Singletons.GetSingleton<IUiHoverManager>();
		_hoveredMenuButton = CreateComputed(ComputeHoveredMenuButton);
		_selected = CreateComputed(ComputeSelected);
	}

	private IMenuButton ComputeHoveredMenuButton()
	{
		return _uiHoverManager.Current?.gameObject?.GetComponent<IMenuButton>();
	}

	private bool ComputeSelected()
	{
		return _hoveredMenuButton.Val != null;
	}
}
