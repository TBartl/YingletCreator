using Character.Creator.UI;
using System.Linq;
using UnityEngine;

internal class ReactiveOffset_OnSpecificMenus : MonoBehaviour, IReactiveOffsetMutator
{
	[SerializeField] Vector3 _offset;
	[SerializeField] MenuType[] _hideOnMenus;
	private IMenuManager _menuManager;


	private void Awake()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
	}

	public Vector3 MutateOffset(Vector3 currentOffset)
	{
		if (_hideOnMenus.Contains(_menuManager.OpenMenu.Val))
		{
			return _offset;
		}
		return currentOffset;
	}
}
