using Character.Data;
using Reactivity;
using UnityEngine;

public interface ICharacterClipProvider
{
	PortraitId Portrait { get; }
	StanceId Stance { get; }
}

internal class CharacterClipProvider : ReactiveBehaviour, ICharacterClipProvider, IInitializable
{
	[SerializeField] private PortraitId _defaultPortrait;
	[SerializeField] private StanceId _defaultStance;


	private ICharacterToggleProvider _toggleProvider;
	private Computed<PortraitId> _portrait;
	private Computed<StanceId> _stance;

	public PortraitId Portrait => _portrait.Val;
	public StanceId Stance => _stance.Val;

	public void Initialize()
	{
		_toggleProvider = this.GetComponentSafe<ICharacterToggleProvider>();
		_portrait = CreateComputed(ComputePortrait);
		_stance = CreateComputed(ComputeStance);
	}

	private PortraitId ComputePortrait()
	{
		return _toggleProvider.Toggles.GetLastComponentOrDefault<PortraitId>() ?? _defaultPortrait;
	}

	private StanceId ComputeStance()
	{
		return _toggleProvider.Toggles.GetLastComponentOrDefault<StanceId>() ?? _defaultStance;
	}
}
