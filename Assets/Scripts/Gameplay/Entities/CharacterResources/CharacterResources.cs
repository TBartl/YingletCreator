using Reactivity;
using UnityEngine;


public interface ICharacterResources
{
	int GetResource(CharacterResourceId type);
	void SetResource(CharacterResourceId type, int value);
}

public class CharacterResources : MonoBehaviour, ICharacterResources, IInitializable
{
	public const int MAX_ENERGY = 5;

	ObservableDict<CharacterResourceId, int> _resources = new ObservableDict<CharacterResourceId, int>();
	private ICommonGameplayAssets _assets;

	public void Initialize()
	{
		_assets = Singletons.GetSingleton<ICommonGameplayAssets>();

		_resources[_assets.ResourceEnergy] = MAX_ENERGY;
		_resources[_assets.ResourceClams] = 2;
		_resources[_assets.ResourceRerolls] = 0;
	}

	public int GetResource(CharacterResourceId type)
	{
		_resources.TryGetValue(type, out var value);
		return value;
	}

	public void SetResource(CharacterResourceId type, int value)
	{
		_resources[type] = value;
	}
}
