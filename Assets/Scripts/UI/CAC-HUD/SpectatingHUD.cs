using Networking;
using Reactivity;
using TMPro;

public class SpectatingHUD : ReactiveBehaviour, ISelectable, IInitializable
{
	private IActiveCharacterProvider _characterProvider;
	private IClientNameLookup _nameLookup;
	private TMP_Text _text;
	Computed<ICharacterIdentity> _spectatedPlayer;
	Computed<bool> _selected;
	Computed<string> _clientName;

	public IReadOnlyObservable<bool> Selected => _selected;

	public void Initialize()
	{
		_characterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();
		_nameLookup = Singletons.GetSingleton<IClientNameLookup>();
		_text = this.GetComponentInChildrenSafe<TMP_Text>();
		_spectatedPlayer = CreateComputed(ComputeSpectatedPlayer);
		_selected = CreateComputed(() => _spectatedPlayer.Val != null);
		_clientName = CreateComputed(ComputeClientName);
	}

	void Start()
	{
		this.InitializeIfNeeded();
		AddReflector(ReflectText);
	}

	private string ComputeClientName()
	{
		var spectatedPlayer = _spectatedPlayer.Val;
		if (spectatedPlayer == null) return null;
		return _nameLookup.GetNameForClient(spectatedPlayer.OwnerClientId);
	}

	private ICharacterIdentity ComputeSpectatedPlayer()
	{
		var activeCharacter = _characterProvider.ActiveCharacter.Val;
		if (activeCharacter == null) return null;
		var identity = activeCharacter.GetComponentSafe<ICharacterIdentity>();
		if (identity.IsMine) return null;
		return identity;
	}

	private void ReflectText()
	{
		var clientName = _clientName.Val;
		if (clientName == null) return;
		_text.text = $"Spectating '{clientName}'";
	}
}
