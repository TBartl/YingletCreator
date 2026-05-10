using Character.Creator.UI;
using UnityEngine;

internal class ReactiveOffset_InExpedition : MonoBehaviour, IReactiveOffsetMutator, IInitializable
{
	[SerializeField] ReactiveOffsetValues _offset;
	private IExpeditionManager _expeditionManager;

	public void Initialize()
	{
		_expeditionManager = Singletons.GetSingleton<IExpeditionManager>();
	}

	public IReactiveOffsetValues MutateOffset(IReactiveOffsetValues currentOffset)
	{
		if (_expeditionManager.State.Val == ExpeditionState.Running)
		{
			return _offset;
		}
		return currentOffset;
	}
}
