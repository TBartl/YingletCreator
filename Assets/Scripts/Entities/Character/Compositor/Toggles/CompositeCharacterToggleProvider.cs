using Character.Creator;
using Character.Data;
using Reactivity;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Provides the full set of toggles
/// This can include toggles from the data as well as toggles added via status effects
/// </summary>
public interface ICharacterToggleProvider
{
	IReadOnlySet<CharacterToggleId> Toggles { get; }
}


public class CompositeCharacterToggleProvider : ReactiveBehaviour, ICharacterToggleProvider, IInitializable
{
	private ICustomizationDataRepository _dataRepo;
	private List<ICharacterToggleProvider> _otherToggleProviders;
	ObservableUpdateableSet<CharacterToggleId> _observableToggleSet = new();

	public IReadOnlySet<CharacterToggleId> Toggles => _observableToggleSet;

	public void Initialize()
	{
		_dataRepo = this.GetComponentInParent<ICustomizationDataRepository>();

		_otherToggleProviders = this.GetComponentsSafe<ICharacterToggleProvider>().Where(p => p != (ICharacterToggleProvider)this).ToList();

		AddReflector(Reflect);
	}

	private void Reflect()
	{
		// Start with the customization data toggles - everything has those
		var toggles = _dataRepo.CustomizationData.ToggleData.Toggles.ToHashSet();

		// Now add in the ones from like, status effects (if we have those)
		foreach (var provider in _otherToggleProviders)
		{
			var providerToggles = provider.Toggles;
			foreach (var providerToggle in providerToggles)
			{
				toggles.FlipToggle(providerToggle);
			}
		}

		_observableToggleSet.Update(toggles);
	}
}