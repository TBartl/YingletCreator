using UnityEngine;

internal class TabToAnotherCharacterOnSleep : MonoBehaviour
{
	private ICharacterRoot _root;
	private IExpeditionCharacterManager _characterManager;
	private ICharacterRoundState _roundState;

	private void Start()
	{
		_root = this.GetComponentInParentSafe<ICharacterRoot>();
		_characterManager = this.GetExpeditionComponent<IExpeditionCharacterManager>();
		_roundState = this.GetCharacterRootComponent<ICharacterRoundState>();

		_roundState.IsAsleep.OnChanged += OnSleepStateChanged;
	}

	private void OnDestroy()
	{
		if (_roundState != null)
		{
			_roundState.IsAsleep.OnChanged -= OnSleepStateChanged;
		}
	}

	private void OnSleepStateChanged(bool from, bool asleep)
	{
		// Only if we are falling asleep
		if (!asleep) return;

		// Only if we are the active character
		var activeCharacter = _characterManager.ActiveCharacter.Val;
		if (activeCharacter.Root != _root) return;

		// Only if this character is mine
		if (!activeCharacter.IsMine) return;

		_characterManager.TryTabToNextCharacter();
	}
}
