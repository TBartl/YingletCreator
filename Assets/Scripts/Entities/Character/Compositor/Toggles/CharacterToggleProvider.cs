using Character.Creator;
using Character.Data;
using Reactivity;
using System.Linq;

/// <summary>
/// Provides the full set of toggles
/// This can include toggles from the data as well as toggles added via status effects
/// </summary>
public interface ICharacterToggleProvider
{
	IReadOnlySet<CharacterToggleId> Toggles { get; }
}


public class CharacterToggleProvider : ReactiveBehaviour, ICharacterToggleProvider, IInitializable
{
	private ICustomizationDataRepository _dataRepo;
	ObservableUpdateableSet<CharacterToggleId> _observableToggleSet = new();

	public IReadOnlySet<CharacterToggleId> Toggles => _observableToggleSet;

	public void Initialize()
	{
		_dataRepo = this.GetComponentInParent<ICustomizationDataRepository>();

		AddReflector(Reflect);
	}

	private void Reflect()
	{
		var dataRepoToggles = _dataRepo.CustomizationData.ToggleData.Toggles.ToHashSet();
		_observableToggleSet.Update(dataRepoToggles);
	}
}