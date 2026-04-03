
using Unity.Netcode;

/// <summary>
/// Sent to all clients by the server when the connected clients change
/// </summary>
public struct UpdateClientManifest : INetMessage
{
	public ulong[] ClientIds;

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref ClientIds);
	}
}
