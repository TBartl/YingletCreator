using Reactivity;
using UnityEngine;


public class MovementPointTrackingLocationProvider : MonoBehaviour, IPointTrackingLocationProvider
{
	[SerializeField] Transform _head;
	[SerializeField] float _offsetFromHead = 2;
	[SerializeField] float _minDotBetweenSpeedAndAcc = -.4f; // Avoid super sharp turns and coming to a halt

	Observable<bool> _active = new Observable<bool>(false);
	private Rigidbody _rb;
	private IAccelerationTracker _accelTracker;

	public IReadOnlyObservable<bool> Active => _active;
	public Vector3 Position { get; private set; }

	private void Awake()
	{
		_rb = this.GetComponentInParent<Rigidbody>();
		_accelTracker = this.GetComponentInParent<IAccelerationTracker>();
	}

	void Update()
	{
		bool isMoving = _rb.linearVelocity.sqrMagnitude > 0.01f;

		var accelXZ = _accelTracker.AccelerationXZ;
		bool isAccelerating = accelXZ.magnitude > .1f;
		bool isAboveDotThreshold = Vector3.Dot(_rb.linearVelocity.normalized, accelXZ.normalized) > _minDotBetweenSpeedAndAcc;

		_active.Val = isMoving && isAccelerating && isAboveDotThreshold;

		if (_active.Val)
		{
			Position = _head.transform.position + accelXZ.normalized * _offsetFromHead;
		}
	}
}
