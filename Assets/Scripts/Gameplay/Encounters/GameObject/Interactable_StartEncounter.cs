using Encounters.Runtime;
using Networking;
using Reactivity;
using Unity.Netcode;
using UnityEngine;

public class Interactable_StartEncounter : MonoBehaviour, IInteractable, IInitializable
{
	[SerializeField] EncounterGraph _encounter;
	[SerializeField] string _tooltipName;

	Observable<EncounterInstance> _encounterInstance = new Observable<EncounterInstance>();
	private INetEventBus _eventBus;
	private INetIdentityProvider _identityProvider;
	private INetIdentity _netIdentity;

	public string TooltipText { get; private set; }

	public Vector3 TooltipOffset => Vector3.up * .6f;


	public void Initialize()
	{
		_eventBus = Singletons.GetSingleton<INetEventBus>();
		_identityProvider = Singletons.GetSingleton<INetIdentityProvider>();
		_netIdentity = this.GetComponentSafe<INetIdentity>();

		_eventBus.Subscribe<Message_InteractWithEncounter>(OnMessage_InteractWithEncounter);

		TooltipText = $"{_tooltipName}\n[E] Interact";
	}

	private void OnDestroy()
	{
		_eventBus?.Unsubscribe<Message_InteractWithEncounter>(OnMessage_InteractWithEncounter);
	}


	public bool CanInteract(ICharacterInteraction character)
	{
		return _encounterInstance.Val == null;
	}

	private void OnMessage_InteractWithEncounter(Message_InteractWithEncounter message, ulong senderClientId)
	{
		if (message.InteractableId != _netIdentity.NetId) return;
		if (_encounterInstance.Val != null) return;

		var character = _identityProvider.GetById(message.CharacterNetId);
		if (character == null) return;

		var characterEncounter = character.gameObject.GetComponentInChildrenSafe<ICharacterEncounterReference>();
		if (characterEncounter.Encounter.Val != null) return;

		var encounterInstance = new EncounterInstance(this.gameObject, character.gameObject.GetComponentSafe<ICharacterRoot>());
		_encounterInstance.Val = encounterInstance;
		characterEncounter.SetEncounter(encounterInstance);
	}


	public void Interact(ICharacterInteraction character)
	{
		_eventBus.SendToAll(new Message_InteractWithEncounter(_netIdentity.NetId, character.Identity.NetId));
	}
}

struct Message_InteractWithEncounter : INetMessage
{
	public ulong InteractableId;
	public ulong CharacterNetId;
	public Message_InteractWithEncounter(ulong interactableId, ulong characterNetId)
	{
		InteractableId = interactableId;
		CharacterNetId = characterNetId;
	}
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref InteractableId);
		serializer.SerializeValue(ref CharacterNetId);
	}
}