
using Character.Creator.UI;
using UnityEngine;

internal class ReactiveOffset_OnCenteredMenus : MonoBehaviour, IReactiveOffsetMutator
{
	[SerializeField] Vector3 _offset = Vector3.zero;

	IMenuManager _menuManager;

	private void Awake()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
	}

	public Vector3 MutateOffset(Vector3 currentOffset)
	{
		if (_menuManager.OpenMenu.Val?.Type == MenuTypeType.CenterScreen)
		{
			return _offset;
		}
		return currentOffset;
	}
}
