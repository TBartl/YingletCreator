
using UnityEngine;

public interface IEncounterInstance
{
	GameObject EncounterSource { get; }
	ICharacterRoot Character { get; }
}

public sealed class EncounterInstance : IEncounterInstance
{
	public GameObject EncounterSource { get; private set; }
	public ICharacterRoot Character { get; private set; }

	public EncounterInstance(GameObject encounterSource, ICharacterRoot character)
	{
		this.EncounterSource = encounterSource;
		this.Character = character;
	}
}
