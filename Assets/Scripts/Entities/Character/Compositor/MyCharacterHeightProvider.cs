using Reactivity;

/// <summary>
/// Singleton implementation of MyCharacterHeightProvider
/// </summary>
public class MyCharacterHeightProvider : ReactiveBehaviour, IYingletHeightProvider
{
	Computed<IYingletHeightProvider> _characterHeightProvider;

	public float YScale => _characterHeightProvider.Val?.YScale ?? 1;

	void Awake()
	{
		var characterSpawner = Singletons.GetSingleton<ICharacterSpawner>();
		_characterHeightProvider = CreateComputed(() =>
		{
			var myCharacter = characterSpawner.MyCharacter;
			if (myCharacter == null) return null;

			return myCharacter.GetComponentInChildren<IYingletHeightProvider>();
		});
	}
}
