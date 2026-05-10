using Reactivity;
using UnityEngine;

public class InsertRoundTransitionMenu : ReactiveBehaviour
{
	[SerializeField] MenuType _menuToInsert;
	[SerializeField] MenuType _menuToInsertAfter;
	private IMenuManager _menuManager;
	private IGlobalRoundProvider _globalRoundProvider;
	private Computed<bool> _showTransitionMenu;

	void Start()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
		_globalRoundProvider = Singletons.GetSingleton<IGlobalRoundProvider>();
		_showTransitionMenu = CreateComputed(ComputeShowTransitionMenu);

		_showTransitionMenu.OnChanged += OnShowTransitionMenuChanged;
	}

	bool ComputeShowTransitionMenu()
	{
		var roundManager = _globalRoundProvider.RoundManager;
		if (roundManager == null) return false;

		var transitionState = roundManager.TransitionState.Val;
		return transitionState == RoundTransitionState.TransitioningIn || transitionState == RoundTransitionState.IncrementRound;
	}

	private void OnShowTransitionMenuChanged(bool from, bool to)
	{
		if (to)
		{
			_menuManager.InsertMenuAfter(_menuToInsert, _menuToInsertAfter);
		}
		else
		{
			_menuManager.RemoveMenu(_menuToInsert);
		}
	}
}
