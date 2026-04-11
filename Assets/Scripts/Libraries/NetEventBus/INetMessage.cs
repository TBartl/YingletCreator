using Unity.Netcode;

public interface INetMessage : INetworkSerializable
{
	NetworkDelivery DeliveryMethod { get; }
}