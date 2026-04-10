using Reactivity;
using UnityEngine;

public interface ICharacterCreatorTracker
{
	public IReadOnlyObservable<bool> IsInCharacterCreator { get; }
}

public class CharacterCreatorTracker : ReactiveBehaviour, ICharacterCreatorTracker
{
	[SerializeField] MenuType _characterCreatorMenu;
	private IMenuManager _menuManager;
	private Computed<bool> _isInCharacterCreator;

	public IReadOnlyObservable<bool> IsInCharacterCreator => _isInCharacterCreator;

	private void Awake()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
		_isInCharacterCreator = CreateComputed(() => _menuManager.OpenMenu.Val == _characterCreatorMenu);
	}
}
