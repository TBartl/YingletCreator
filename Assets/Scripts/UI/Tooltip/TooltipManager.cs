using Reactivity;

public interface ITooltipProvider
{
	IReadOnlyObservable<ITooltip> DesiredTooltip { get; }
}

public interface ITooltipManager
{
	IReadOnlyObservable<ITooltip> CurrentTooltip { get; }
}

public class TooltipManager : ReactiveBehaviour, ITooltipManager
{
	public IReadOnlyObservable<ITooltip> CurrentTooltip => _currentTooltip;

	private ITooltipProvider[] _providers;
	Computed<ITooltip> _currentTooltip;

	private void Awake()
	{
		_providers = this.GetComponentsInChildren<ITooltipProvider>();
		_currentTooltip = CreateComputed(ComputeCurrentTooltip);
	}

	private ITooltip ComputeCurrentTooltip()
	{
		foreach (var provider in _providers)
		{
			var tooltip = provider.DesiredTooltip.Val;
			if (tooltip != null) return tooltip;
		}
		return null;
	}
}
