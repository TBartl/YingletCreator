using Unity.Netcode;
using UnityEngine;

public struct TestMessage : INetMessage
{
	public string Text;
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
		_netBus.Subscribe<TestMessage>(OnTestMessage);
	}

	private void OnDestroy()
	{
		_netBus.Unsubscribe<TestMessage>(OnTestMessage);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log("Sending message");
			var message = new TestMessage() { Text = "Space pressed" };
			_netBus.SendToAll(message);
		}
	}

	private void OnTestMessage(TestMessage message)
	{
		Debug.Log("Recieved test message: " + message.Text);
	}
}
