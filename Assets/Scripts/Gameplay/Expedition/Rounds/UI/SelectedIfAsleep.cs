using Character.Creator.UI;
using Reactivity;
using UnityEngine;

internal class SelectedIfAsleep : ReactiveBehaviour, ISelectable, IInitializable
{
	[SerializeField] ReactiveOffsetValues _offset;
	private IPartyMemberHUDReference _reference;
	private Computed<ICharacterRoundState> _roundState;
	private Computed<bool> _isAsleep;

	public IReadOnlyObservable<bool> Selected => _isAsleep;

	public void Initialize()
	{
		_reference = this.GetComponentInParentSafe<IPartyMemberHUDReference>();
		_roundState = CreateComputed(ComputeRoundState);
		_isAsleep = CreateComputed(ComputeIsAsleep);
	}

	private ICharacterRoundState ComputeRoundState()
	{
		var character = _reference.Character;
		if (character == null) return null;
		return character.GetComponentInChildrenSafe<ICharacterRoundState>();
	}

	private bool ComputeIsAsleep()
	{
		var roundState = _roundState.Val;
		if (roundState == null) return false;
		return roundState.IsAsleep.Val;
	}
}
