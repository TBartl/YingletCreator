using Reactivity;

public interface IUIInteractable
{
	IReadOnlyObservable<bool> Interactable { get; }
}
