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
		var activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();
		_characterHeightProvider = CreateComputed(() =>
		{
			var myCharacter = activeCharacterProvider.ActiveCharacter.Val;
			if (myCharacter == null) return null;

			return myCharacter.GetComponentInChildren<IYingletHeightProvider>();
		});
	}
}
