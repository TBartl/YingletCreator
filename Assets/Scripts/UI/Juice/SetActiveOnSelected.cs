using Reactivity;

public class SetActiveOnSelected : ReactiveBehaviour
{
	ISelectable _selection;

	private void Start()
	{
		_selection = this.GetComponentInParentSafe<ISelectable>();
		AddReflector(ReflectSelection);
	}

	private void ReflectSelection()
	{
		this.gameObject.SetActive(_selection.Selected.Val);
	}
}
