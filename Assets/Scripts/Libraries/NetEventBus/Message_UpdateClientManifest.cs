
using Unity.Netcode;

/// <summary>
/// Sent to all clients by the server when the connected clients change
/// </summary>
public struct Message_UpdateClientManifest : INetMessage
{
	public ulong[] ClientIds;

	public NetworkDelivery DeliveryMethod => NetworkDelivery.ReliableSequenced;
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref ClientIds);
	}
}
