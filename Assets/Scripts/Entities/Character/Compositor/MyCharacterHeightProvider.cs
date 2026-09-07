using Reactivity;

/// <summary>
/// Singleton implementation of MyCharacterHeightProvider
/// </summary>
public class MyCharacterHeightProvider : ReactiveBehaviour, IYingletHeightProvider
{
	Computed<IYingletHeightProvider> _characterHeightProvider;

	public float HeightBasedOnScale => _characterHeightProvider.Val?.HeightBasedOnScale ?? 1;
	public float HeightBasedOnStance => _characterHeightProvider.Val?.HeightBasedOnStance ?? 1;

	void Awake()
	{
		var activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();
		_characterHeightProvider = CreateComputed(() =>
		{
			var myCharacter = activeCharacterProvider.ActiveCharacter.Val;
			if (myCharacter == null) return null;

			return myCharacter.GetComponentInChildrenSafe<IYingletHeightProvider>();
		});
	}
}
