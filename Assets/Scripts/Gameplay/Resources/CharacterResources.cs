using Reactivity;
using UnityEngine;

public enum CharacterResourceType
{
	Energy,
	Clams,
	Rerolls,
}

public interface ICharacterResources
{
	int GetResource(CharacterResourceType type);
}

public class CharacterResources : MonoBehaviour, ICharacterResources, IInitializable
{
	ObservableDict<CharacterResourceType, int> _resources = new ObservableDict<CharacterResourceType, int>();
	public void Initialize()
	{
		_resources[CharacterResourceType.Energy] = 5;
		_resources[CharacterResourceType.Clams] = 2;
		_resources[CharacterResourceType.Rerolls] = 0;
	}

	public int GetResource(CharacterResourceType type)
	{
		_resources.TryGetValue(type, out var value);
		return value;
	}
}
