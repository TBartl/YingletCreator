using Character.Creator;
using UnityEngine;



public class SnapshotterRelayDataRepository : MonoBehaviour, ICustomizationDataRepository, IInitializable
{
	private ICustomizationDataRepository _characterDataRepo;

	public ObservableCustomizationData CustomizationData => _characterDataRepo.CustomizationData;

	public void Initialize()
	{
		var relay = this.GetComponentSafe<ISnapshotterRelay>();
		_characterDataRepo = relay.RelayedCharacter.GetComponentInChildrenSafe<ICustomizationDataRepository>();
	}
}
