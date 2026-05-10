using Reactivity;

public class EnableIfCharacterSleeping : ReactiveBehaviour
{
	private ICharacterRoundState _characterRoundManager;

	void Start()
	{
		_characterRoundManager = this.GetCharacterRootComponent<ICharacterRoundState>();
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		this.gameObject.SetActive(_characterRoundManager.IsAsleep.Val);
	}
}
