using Reactivity;


/// <summary>
/// A lot of the UI is interested in expedition-specific round information
/// This provides a convenient way to access it
/// </summary>
public interface IGlobalRoundProvider
{
	IExpeditionRoundManager RoundManager { get; }
	ICharacterRoundState ActiveCharacterState { get; }
}

public class GlobalRoundProvider : ReactiveBehaviour, IGlobalRoundProvider, IInitializable
{
	Computed<IExpeditionRoundManager> _roundManager;
	Computed<ICharacterRoundState> _activeCharacterState;
	private IActiveCharacterProvider _activeCharacterProvider;

	public IExpeditionRoundManager RoundManager => _roundManager.Val;
	public ICharacterRoundState ActiveCharacterState => _activeCharacterState.Val;

	public void Initialize()
	{
		_roundManager = this.CreateExpeditionComputed<IExpeditionRoundManager>();
		_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();
		_activeCharacterState = CreateComputed(ComputeActiveCharacterState);
	}

	private ICharacterRoundState ComputeActiveCharacterState()
	{
		return _activeCharacterProvider.ActiveExpeditionCharacter.Val?.GetComponentInChildrenSafe<ICharacterRoundState>();
	}
}
