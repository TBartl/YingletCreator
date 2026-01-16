using Character.Creator.UI;
using UnityEngine;

internal class ReactiveOffset_OnPhotoMode : MonoBehaviour, IReactiveOffsetMutator
{
	[SerializeField] Vector3 _offset;

	private IPhotoModeState _photoModeState;

	private void Awake()
	{
		_photoModeState = this.GetComponentInParent<IPhotoModeState>();
	}

	public Vector3 MutateOffset(Vector3 currentOffset)
	{
		if (_photoModeState.IsInPhotoMode.Val)
		{
			return _offset;
		}
		return currentOffset;
	}
}
