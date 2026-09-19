using Networking;

public interface IEncounterNetworking
{
	public INetEventBus EventBus { get; }
	public INetIdentityProvider IdentityProvider { get; }
}

public class EncounterNetworking : IEncounterNetworking
{
	private readonly INetIdentityProvider _idProvider;
	private readonly INetEventBus _eventBus;

	public INetEventBus EventBus => _eventBus;
	public INetIdentityProvider IdentityProvider => _idProvider;

	public EncounterNetworking(IEncounterInstance encounter)
	{
		_idProvider = Singletons.GetSingleton<INetIdentityProvider>();
		_eventBus = Singletons.GetSingleton<INetEventBus>();
	}
}

