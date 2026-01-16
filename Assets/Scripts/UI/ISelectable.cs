using Reactivity;

public interface ISelectable
{
	IReadOnlyObservable<bool> Selected { get; }
}
