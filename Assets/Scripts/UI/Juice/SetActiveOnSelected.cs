using Reactivity;

public class SetActiveOnSelected : ReactiveBehaviour
{
	ISelectable _selection;

	private void Awake()
	{
		_selection = this.GetComponentInParentSafe<ISelectable>();
	}

	private void Start()
	{
		AddReflector(ReflectSelection);
	}

	private void ReflectSelection()
	{
		this.gameObject.SetActive(_selection.Selected.Val);
	}
}
