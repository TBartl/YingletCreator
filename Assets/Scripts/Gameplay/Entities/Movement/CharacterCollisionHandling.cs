using Networking;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;


public delegate void ImpactGroundEvent(PhysicsMaterial physicsMaterial, float speed, Vector3 position);

public interface ICharacterCollisionHandling
{
	bool CanJump { get; }
	bool Grounded { get; }
	PhysicsMaterial LastGroundMaterial { get; }
	void ClearCanJump();
	ImpactGroundEvent OnImpactedGround { get; set; }
}

public class CharacterCollisionHandling : MonoBehaviour, ICharacterCollisionHandling
{

	public const float GROUND_DOT = .7f;
	const float VEL_IMPACT_THRESHOLD = 2f;
	const float COYOTE_TIME = .2f;

	public bool CanJump { get; private set; } = true;
	public bool Grounded { get; private set; }
	public PhysicsMaterial LastGroundMaterial { get; private set; }
	public ImpactGroundEvent OnImpactedGround { get; set; } = delegate { };

	private INetEventBus _eventBus;
	private ICharacterIdentity _identity;
	private INetworkRigidbody _networkRB;
	private int _raycastLayerMask;
	float _lastJumpClearTime = 0; // The player stays on the ground the frame they jump, this prevents that from giving them another jump
	bool _wasGrounded;

	private void Awake()
	{
		_eventBus = Singletons.GetSingleton<INetEventBus>();
		_identity = this.GetComponentInParentSafe<ICharacterIdentity>();
		_networkRB = this.GetComponent<INetworkRigidbody>();

		_raycastLayerMask = LayerMask.GetMask("Default");

		_eventBus.Subscribe<Message_ImpactedGround>(OnMessageImpactedGround);
	}

	private void OnDestroy()
	{
		_eventBus.Unsubscribe<Message_ImpactedGround>(OnMessageImpactedGround);
	}

	public void ClearCanJump()
	{
		CanJump = false;
		StopClearCanJumpCoroutine();
		_lastJumpClearTime = Time.time;
	}

	private void FixedUpdate()
	{
		_wasGrounded = Grounded;
		Grounded = false;
	}

	void OnCollisionEnter(Collision other)
	{
		HandleCollision(other);
	}

	void OnCollisionStay(Collision other)
	{
		HandleCollision(other);
	}

	void HandleCollision(Collision other)
	{
		if (Grounded)
		{
			// Something already grounded us this update - don't check for more since we don't want multiple triggers
			return;
		}


		bool touchingSolid = other.contacts.Any(contact =>
		{
			var material = contact.otherCollider.sharedMaterial;

			float verticalDot = Vector3.Dot(Vector3.up, contact.normal);



			if (_identity.IsMine && !_wasGrounded && verticalDot > GROUND_DOT && other.relativeVelocity.y > VEL_IMPACT_THRESHOLD)
			{
				OnImpactedGround(material, other.relativeVelocity.y, this.transform.position);

				// Velocity is kinda weird networked, so we can't rely on physics for a clean ground impact
				// Send it as a message instead
				SendImpactedGroundMessage(other.relativeVelocity.y, this.transform.position);
			}

			bool validCollision = verticalDot > GROUND_DOT;

			if (validCollision)
			{
				LastGroundMaterial = material;
			}

			return validCollision;
		});
		if (touchingSolid && Time.time > _lastJumpClearTime + .01f)
		{
			Grounded = true;
			CanJump = true;

			StopClearCanJumpCoroutine();
			clearCanJumpCoroutine = StartCoroutine(ClearCanJumpAfterTime());
		}
	}

	Coroutine clearCanJumpCoroutine;
	IEnumerator ClearCanJumpAfterTime()
	{
		yield return new WaitForSeconds(COYOTE_TIME);
		CanJump = false;
		clearCanJumpCoroutine = null;
	}
	void StopClearCanJumpCoroutine()
	{
		if (clearCanJumpCoroutine == null) return;
		StopCoroutine(clearCanJumpCoroutine);
		clearCanJumpCoroutine = null;
	}

	void SendImpactedGroundMessage(float impactVelocity, Vector3 impactPosition)
	{
		_eventBus.SendToAll(new Message_ImpactedGround(_identity.NetId, impactVelocity, impactPosition));
	}

	private void OnMessageImpactedGround(Message_ImpactedGround message, ulong senderClientId)
	{
		if (_identity.IsMine) return; // We already know, return
		if (senderClientId != _identity.OwnerClientId) return; // Not from the owner, return
		if (message.NetId != _identity.NetId) return; // Not for this character, return

		StartCoroutine(DelayImpactedGround(message.ImpactVelocity, message.ImpactPosition));
	}

	IEnumerator DelayImpactedGround(float impactVelocity, Vector3 impactPosition)
	{
		yield return new WaitForSeconds((float)_networkRB.BufferTime);

		// Can't easily send the material, so raycast down to try and assume it
		PhysicsMaterial material = null;
		if (Physics.Raycast(impactPosition + Vector3.up, Vector3.down, out RaycastHit hit, 4, _raycastLayerMask))
		{
			material = hit.collider.sharedMaterial;
		}

		OnImpactedGround(material, impactVelocity, impactPosition);
	}
}

public struct Message_ImpactedGround : INetMessage
{
	public ulong NetId;
	public float ImpactVelocity;
	public Vector3 ImpactPosition;

	public Message_ImpactedGround(ulong netId, float impactVelocity, Vector3 impactPosition)
	{
		NetId = netId;
		ImpactVelocity = impactVelocity;
		ImpactPosition = impactPosition;
	}

	public NetworkDelivery DeliveryMethod => NetworkDelivery.Reliable;
	public bool SendToSelf => false;

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref NetId);
		serializer.SerializeValue(ref ImpactVelocity);
		serializer.SerializeValue(ref ImpactPosition);
	}
}