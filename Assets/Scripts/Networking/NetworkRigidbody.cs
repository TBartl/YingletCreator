using Mirror;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface INetworkRigidbody
{
	/// <summary>
	/// Some other components will want to delay by this amount to better line up with the transform
	/// </summary>
	public double BufferTime { get; }
}

[RequireComponent(typeof(Rigidbody))]
public class NetworkRigidbody : MonoBehaviour, INetworkRigidbody
{
	[SerializeField] float _snapshotSendRate = 0.2f;
	[SerializeField] double _bufferTime = 0.1f;
	[SerializeField] int _snapshotBufferLimit = 20;

	private INetEventBus _eventBus;
	private IPlayerIdentity _identity;
	private Rigidbody _rb;
	readonly SortedList<double, RigidbodySnapshot> _snapshots = new();

	float _lastSnapshotSendTime = 0;

	public double BufferTime => _bufferTime;

	void Start()
	{
		_eventBus = Singletons.GetSingleton<INetEventBus>();
		_identity = this.GetComponentInParent<IPlayerIdentity>();
		_rb = this.GetComponent<Rigidbody>();
		_eventBus.Subscribe<Message_SendRigidbodySnapshot>(OnReceiveSnapshot);

	}

	private void OnDestroy()
	{
		_eventBus.Unsubscribe<Message_SendRigidbodySnapshot>(OnReceiveSnapshot);
	}

	private void Update()
	{
		UpdateSendSnapshots();
		UpdateApplySnapshots();
	}

	void UpdateSendSnapshots()
	{
		if (!_identity.IsMine) return;
		if (Time.time < _lastSnapshotSendTime + _snapshotSendRate) return;

		_eventBus.SendToAll(new Message_SendRigidbodySnapshot
		{
			Position = _rb.position,
			Velocity = _rb.linearVelocity,
			RemoteTime = (float)_eventBus.NetworkTime
		});

		_lastSnapshotSendTime = Time.time;
	}

	private void UpdateApplySnapshots()
	{
		if (_identity.IsMine) return;
		if (_snapshots.Count == 0) return;

		SnapshotInterpolation.StepInterpolation(
			_snapshots,
			_eventBus.NetworkTime - _bufferTime,
			out RigidbodySnapshot from,
			out RigidbodySnapshot to,
			out double t);

		RigidbodySnapshot computed = RigidbodySnapshot.Interpolate(from, to, t);
		_rb.MovePosition(computed.Position);
		_rb.linearVelocity = computed.Velocity;
	}

	private void OnReceiveSnapshot(Message_SendRigidbodySnapshot message, ulong senderClientId)
	{
		if (_identity.IsMine) return; // Optimization opportunity: The server probably shouldn't even be sending this to us
		if (senderClientId != _identity.ConnectionId) return;

		SnapshotInterpolation.InsertIfNotExists(
			_snapshots,
			_snapshotBufferLimit,
			new RigidbodySnapshot(message.RemoteTime, Time.timeAsDouble, message.Position, message.Velocity)
		);
	}
}

struct RigidbodySnapshot : Snapshot
{
	public Vector3 Position;
	public Vector3 Velocity;

	public double remoteTime { get; set; }
	public double localTime { get; set; }

	public RigidbodySnapshot(double remoteTime, double localTime, Vector3 position, Vector3 velocity)
	{
		this.remoteTime = remoteTime;
		this.localTime = localTime;
		this.Position = position;
		this.Velocity = velocity;
	}

	public static RigidbodySnapshot Interpolate(RigidbodySnapshot from, RigidbodySnapshot to, double t)
	{
		return new RigidbodySnapshot
		{
			Position = Vector3.LerpUnclamped(from.Position, to.Position, (float)t),
			Velocity = Vector3.LerpUnclamped(from.Velocity, to.Velocity, (float)t),
		};
	}
}

// Optimization opportunity: This can probably be sent without UDP
struct Message_SendRigidbodySnapshot : INetMessage
{
	public Vector3 Position;
	public Vector3 Velocity;
	public float RemoteTime;

	public Message_SendRigidbodySnapshot(Vector3 position, Vector3 velocity, float remoteTime)
	{
		Position = position;
		Velocity = velocity;
		RemoteTime = remoteTime;
	}

	public NetworkDelivery DeliveryMethod => NetworkDelivery.Unreliable;

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref Position);
		serializer.SerializeValue(ref Velocity);
		serializer.SerializeValue(ref RemoteTime);
	}
}
