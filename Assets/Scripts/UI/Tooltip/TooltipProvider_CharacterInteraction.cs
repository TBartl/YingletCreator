using Reactivity;
using UnityEngine;

public sealed class InteractableTooltip : ITooltip
{
	private readonly IInteractable _interactable;

	public InteractableTooltip(IInteractable interactable)
	{
		_interactable = interactable;
	}

	public string Text => _interactable.TooltipText;

	public Vector2 Position => Camera.main.WorldToScreenPoint(_interactable.transform.position + _interactable.TooltipOffset);

	public Vector2 SizeDelta => Vector2.zero;
}

public class TooltipProvider_CharacterInteraction : ReactiveBehaviour, ITooltipProvider
{
	[SerializeField] MenuType _validMenu;

	private ICharacterSpawner _characterSpawner;
	private IMenuManager _menuManager;
	private Computed<bool> _onDesiredMenu;
	Computed<ICharacterInteractionRadius> _interactionRadius;
	Computed<ITooltip> _desiredTooltip;
	public IReadOnlyObservable<ITooltip> DesiredTooltip => _desiredTooltip;

	private void Awake()
	{
		_characterSpawner = Singletons.GetSingleton<ICharacterSpawner>();
		_menuManager = Singletons.GetSingleton<IMenuManager>();
		_onDesiredMenu = CreateComputed(() => _menuManager.OpenMenu.Val == _validMenu);
		_interactionRadius = CreateComputed(() => _characterSpawner.MyCharacter?.GetComponentInChildren<ICharacterInteractionRadius>());
		_desiredTooltip = CreateComputed(ComputeDesiredTooltip);
	}

	private ITooltip ComputeDesiredTooltip()
	{
		if (!_onDesiredMenu.Val) return null;

		var interactionRadius = _interactionRadius.Val;
		if (interactionRadius == null) return null;

		var highlighted = interactionRadius.Highlighted;
		if (highlighted == null) return null;

		return new InteractableTooltip(highlighted);
	}
}
