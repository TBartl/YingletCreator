
using Reactivity;
using System.Linq;

public class UIInteractableCompositor : ReactiveBehaviour, IUIInteractable, IInitializable
{
	private IUIInteractable[] _otherUiInteractables;
	private Computed<bool> _interactable;

	public IReadOnlyObservable<bool> Interactable => _interactable;

	public void Initialize()
	{
		_otherUiInteractables = this.GetComponentsSafe<IUIInteractable>().Where(x => x != (IUIInteractable)this).ToArray();
		_interactable = CreateComputed<bool>(ComputeInteractable);
	}

	private bool ComputeInteractable()
	{
		foreach (var other in _otherUiInteractables)
		{
			if (!other.Interactable.Val)
			{
				return false;
			}
		}
		return true;
	}
}
