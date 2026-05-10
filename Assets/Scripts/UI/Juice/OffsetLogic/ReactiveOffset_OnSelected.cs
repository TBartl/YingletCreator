using Character.Creator.UI;
using UnityEngine;

internal class ReactiveOffset_OnSelected : MonoBehaviour, IReactiveOffsetMutator, IInitializable
{
	[SerializeField] ReactiveOffsetValues _offset;
	private ISelectable _selectable;

	public void Initialize()
	{
		_selectable = this.GetComponentInParentSafe<ISelectable>();
	}

	public IReactiveOffsetValues MutateOffset(IReactiveOffsetValues currentOffset)
	{
		if (_selectable.Selected.Val)
		{
			return _offset;
		}
		return currentOffset;
	}
}
