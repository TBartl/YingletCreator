using Reactivity;
using UnityEngine;

public class InsertEncounterMenu : ReactiveBehaviour
{
	[SerializeField] MenuType _menuToInsert;
	[SerializeField] MenuType _menuToInsertAfter;
	private IMenuManager _menuManager;
	private IActiveEncounterProvider _activeEncounterProvider;
	private Computed<bool> _inEncounter;

	void Start()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
		_activeEncounterProvider = Singletons.GetSingleton<IActiveEncounterProvider>();
		_inEncounter = CreateComputed(() => _activeEncounterProvider.ActiveEncounter.Val != null);
		_inEncounter.OnChanged += OnEncounterChanged;
	}

	private void OnEncounterChanged(bool from, bool to)
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
