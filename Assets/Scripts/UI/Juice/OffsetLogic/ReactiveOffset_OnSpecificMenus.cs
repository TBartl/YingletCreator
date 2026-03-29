using Character.Creator.UI;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

internal class ReactiveOffset_OnSpecificMenus : MonoBehaviour, IReactiveOffsetMutator
{
	[SerializeField] ReactiveOffsetValues _offset;

	[FormerlySerializedAs("_hideOnMenus")]
	[SerializeField] MenuType[] _menus;

	private IMenuManager _menuManager;


	private void Awake()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
	}

	public ReactiveOffsetValues MutateOffset(ReactiveOffsetValues currentOffset)
	{
		if (_menus.Contains(_menuManager.OpenMenu.Val))
		{
			return _offset;
		}
		return currentOffset;
	}
}
