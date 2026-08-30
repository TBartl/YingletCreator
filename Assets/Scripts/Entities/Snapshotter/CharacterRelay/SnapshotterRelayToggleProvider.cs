using Character.Data;
using UnityEngine;


public class SnapshotterRelayToggleRepository : MonoBehaviour, ICharacterToggleProvider, IInitializable
{
	private ICharacterToggleProvider _characterToggleProvider;

	public IReadOnlySet<CharacterToggleId> Toggles => _characterToggleProvider.Toggles;

	public void Initialize()
	{
		var relay = this.GetComponentSafe<ISnapshotterRelay>();
		_characterToggleProvider = relay.RelayedCharacter.GetComponentInChildrenSafe<ICharacterToggleProvider>();
	}
}
