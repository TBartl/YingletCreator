using Character.Data;
using Reactivity;
using UnityEngine;

public interface ICharacterPortraitProvider
{
	PortraitId Portrait { get; }
}

internal class CharacterPortraitProvider : ReactiveBehaviour, ICharacterPortraitProvider, IInitializable
{
	[SerializeField] private PortraitId _defaultPortrait;
	private ICharacterToggleProvider _toggleProvider;
	private Computed<PortraitId> _portrait;

	public PortraitId Portrait => _portrait.Val;

	public void Initialize()
	{
		_toggleProvider = this.GetComponentSafe<ICharacterToggleProvider>();
		_portrait = CreateComputed(ComputePortrait);
	}

	private PortraitId ComputePortrait()
	{
		return _toggleProvider.Toggles.GetLastComponentOrDefault<PortraitId>() ?? _defaultPortrait;
	}

}
