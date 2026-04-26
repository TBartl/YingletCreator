using Character.Creator.UI;
using UnityEngine;

internal class ReactiveOffset_OnSelected : MonoBehaviour, IReactiveOffsetMutator, IInitializable
{
	[SerializeField] ReactiveOffsetValues _offset;
	private ISelectable _selectable;

	public void Initialize()
	{
		_selectable = this.GetComponentSafe<ISelectable>();
	}

	public ReactiveOffsetValues MutateOffset(ReactiveOffsetValues currentOffset)
	{
		if (_selectable.Selected.Val)
		{
			return _offset;
		}
		return currentOffset;
	}
}
