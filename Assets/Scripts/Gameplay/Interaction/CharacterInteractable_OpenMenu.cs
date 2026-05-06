using UnityEngine;

public class CharacterInteractable_OpenMenu : MonoBehaviour, ICharacterInteractable
{
	[SerializeField] MenuType _menuToOpen;
	[SerializeField] string _tooltipText = "Customize Character";

	private IMenuManager _menuManager;

	public string TooltipText => $"[E] {_tooltipText}";

	[field: SerializeField]
	public Vector3 TooltipOffset { get; private set; }


	private void Awake()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
	}

	public bool CanInteract(ICharacterInteraction character)
	{
		// Should probably check that we're not already in it?
		// Or maybe that logic should live in CharacterInteraction?
		return true;
	}

	public void Interact(ICharacterInteraction character)
	{
		_menuManager.PushMenu(_menuToOpen);
		//character.gameObject.GetComponentInChildren<IGameCharacterDataRepository>().Increment();
	}
}
