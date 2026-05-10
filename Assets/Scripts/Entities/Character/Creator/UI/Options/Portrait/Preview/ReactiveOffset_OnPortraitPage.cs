using Character.Creator.UI;
using UnityEngine;

internal class ReactiveOffset_OnPortraitPage : MonoBehaviour, IReactiveOffsetMutator
{
	[SerializeField] ReactiveOffsetValues _offset;

	private IViewingPortraitPageTracker _viewingPortraitTracker;

	private void Awake()
	{
		_viewingPortraitTracker = this.GetComponentInParent<IViewingPortraitPageTracker>();
	}

	public IReactiveOffsetValues MutateOffset(IReactiveOffsetValues currentOffset)
	{
		if (_viewingPortraitTracker.IsViewingPortraitPage)
		{
			return _offset;
		}
		return currentOffset;
	}
}
