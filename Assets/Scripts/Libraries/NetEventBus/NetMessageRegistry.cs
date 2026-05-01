using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

public interface INetMessageRegistry
{
	uint GetMessageId<T>(T netMessage) where T : INetMessage;
	INetMessage ReadMessage(uint messageId, ref FastBufferReader reader);

}

public sealed class NetMessageRegistry : INetMessageRegistry
{

	Dictionary<ulong, RegistryEntry> _idToEntry = new();
	Dictionary<Type, ulong> _typeToId = new();

	uint _lastMessageId = 0;
	public NetMessageRegistry()
	{
		// Annoyingly, I couldn't find a clean way to do a Reader.ReadNetworkSerializable with System.Type
		// As a result, I'm registering each message type manually here
		Register<Message_UpdateClientManifest>();
		Register<Message_TestMessage>();

		// Player
		Register<Message_SendRigidbodySnapshot>();
		Register<Message_Jump>();
		Register<Message_ImpactedGround>();
		Register<Message_UpdateCustomizationData>();

		Register<Message_AddExpeditionPartyMember>();
		Register<Message_RemoveExpeditionPartyMember>();
		Register<Message_InitializeExpeditionPartyForClient>();
		Register<Message_SendClientName>();
		Register<Message_TransitionToExpedition>();
		Register<Message_StartExpedition>();

		Register<Message_CreateLobbyCharacter>();
		Register<Message_RemoveLobbyCharacter>();

		Register<Message_CharacterEnteredRoom>();
		Register<Message_InteractWithEncounter>();

		// Do a little bit of reflection to see if there's any message types we missed out on and log a warning about them
		var reflectedMessageTypes = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(a => a.GetTypes())
			.Where(t => typeof(INetMessage).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

		var unregisteredTypes = reflectedMessageTypes.Where(t => !_typeToId.ContainsKey(t)).ToList();
		foreach (var unregisteredType in unregisteredTypes)
		{
			UnityEngine.Debug.LogWarning($"Found unregistered net message type: {unregisteredType.FullName}. This should probably be registered");
		}
	}


	private void Register<T>() where T : INetMessage, new()
	{
		var messageType = typeof(T);
		uint id = _lastMessageId;
		Func<FastBufferReader, INetMessage> readerFunc = reader =>
		{
			reader.ReadNetworkSerializable(out T message);
			return message;
		};
		_idToEntry[id] = new RegistryEntry(messageType, readerFunc);
		_typeToId[messageType] = id;
		_lastMessageId += 1;
	}


	public uint GetMessageId<T>(T netMessage) where T : INetMessage
	{
		var messageType = netMessage.GetType();
		if (_typeToId.TryGetValue(messageType, out var id))
		{
			return (uint)id;
		}
		throw new KeyNotFoundException($"Message type {messageType.FullName} is not registered in the NetMessageRegistry.");
	}

	public INetMessage ReadMessage(uint messageId, ref FastBufferReader reader)
	{
		if (_idToEntry.TryGetValue(messageId, out var entry))
		{
			return entry.ReaderFunc(reader);
		}
		throw new KeyNotFoundException($"Message ID {messageId} is not registered in the NetMessageRegistry.");
	}
	private class RegistryEntry
	{
		public Type MessageType { get; }
		public Func<FastBufferReader, INetMessage> ReaderFunc { get; }
		public RegistryEntry(Type messageType, Func<FastBufferReader, INetMessage> readerFunc)
		{
			MessageType = messageType;
			ReaderFunc = readerFunc;
		}
	}
}
