using UnityEngine;

/// <summary>
/// Contains references to assets that will be commonly referenced from scripts, and aren't worth repeatedly serializing on a bunch of different components
/// </summary>
public interface ICommonGameplayAssets
{
	CharacterResourceId ResourceEnergy { get; }
	CharacterResourceId ResourceClams { get; }
	CharacterResourceId ResourceRerolls { get; }
}
internal class CommonGameplayAssets : MonoBehaviour, ICommonGameplayAssets
{
	[SerializeField] AssetReferenceT<CharacterResourceId> _resourceEnergy;
	[SerializeField] AssetReferenceT<CharacterResourceId> _resourceClams;
	[SerializeField] AssetReferenceT<CharacterResourceId> _resourceRerolls;

	public CharacterResourceId ResourceEnergy => _resourceEnergy.LoadSync();
	public CharacterResourceId ResourceClams => _resourceClams.LoadSync();
	public CharacterResourceId ResourceRerolls => _resourceRerolls.LoadSync();
}
