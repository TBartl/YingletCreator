using Character.Creator.UI;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

internal class ReactiveOffset_OnSpecificMenus : MonoBehaviour, IReactiveOffsetMutator
{
	[SerializeField] ReactiveOffsetValues _offset;
	[SerializeField] bool _inverse;

	[FormerlySerializedAs("_hideOnMenus")]
	[SerializeField] MenuType[] _menus;


	private IMenuManager _menuManager;


	private void Awake()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
	}

	public ReactiveOffsetValues MutateOffset(ReactiveOffsetValues currentOffset)
	{
		bool onMenu = _inverse ? !_menus.Contains(_menuManager.OpenMenu.Val) : _menus.Contains(_menuManager.OpenMenu.Val);
		if (onMenu)
		{
			return _offset;
		}
		return currentOffset;
	}
}
