using Unity.Netcode;
using UnityEngine;

public struct Message_TestMessage : INetMessage
{
	public string Text;

	public NetworkDelivery DeliveryMethod => NetworkDelivery.ReliableSequenced;

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref Text);
	}
}

internal class NetMessageTest : MonoBehaviour
{
	private INetEventBus _netBus;

	private void Start()
	{
		_netBus = Singletons.GetSingleton<INetEventBus>();
		_netBus.Subscribe<Message_TestMessage>(OnTestMessage);
	}

	private void OnDestroy()
	{
		_netBus.Unsubscribe<Message_TestMessage>(OnTestMessage);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log("Sending test message");
			var message = new Message_TestMessage() { Text = "Space pressed" };
			_netBus.SendToAll(message);
		}
	}

	private void OnTestMessage(Message_TestMessage message, ulong senderClientId)
	{
		Debug.Log("Recieved test message: " + message.Text);
	}


}
