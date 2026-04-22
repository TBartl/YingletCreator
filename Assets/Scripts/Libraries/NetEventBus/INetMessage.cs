using Unity.Netcode;

public interface INetMessage : INetworkSerializable
{
	NetworkDelivery DeliveryMethod => NetworkDelivery.ReliableSequenced;

	/// <summary>
	/// If true, this message will ultimately end back up at the client
	/// For a server, this will be immediate
	/// For a pure client, this will be when the server has been re-sent back by the server back to us
	/// </summary>
	bool SendToSelf => true;
}