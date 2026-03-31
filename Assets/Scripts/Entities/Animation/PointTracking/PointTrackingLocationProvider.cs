using Reactivity;
using UnityEngine;

public interface IPointTrackingLocationProvider
{
	IReadOnlyObservable<bool> Active { get; }
	Vector3 Position { get; }
}

public class PointTrackingLocationProvider : MonoBehaviour, IPointTrackingLocationProvider
{
	[SerializeField] Transform _headCenter;
	[SerializeField] Transform _forwardProvider;

	[SerializeField] float MaxDistance = 1.4f;
	[SerializeField] float OffsetMouseFromPlane = .2f;
	[SerializeField] float MaxDotProduct = 0;

	Observable<bool> _active = new Observable<bool>(false);
	private IUiHoverManager _uiHoverManager;
	private IPointTrackingForcer _forcer;

	public IReadOnlyObservable<bool> Active => _active;
	public Vector3 Position { get; private set; }

	Vector3 ForwardDir => -_forwardProvider.forward;

	private void Awake()
	{
		// Might be better to feed some of these in via mutator components so this class is less bloated
		_uiHoverManager = Singletons.GetSingleton<IUiHoverManager>();
		_forcer = this.GetComponent<IPointTrackingForcer>();
	}

	void Update()
	{
		if (_uiHoverManager.HoveringUi && !_forcer.Forcing)
		{
			_active.Val = false;
			return;
		}
		if (Input.GetMouseButton(1)) // Might be spinning. Kind of hacky and would need to be made more generic with the ui hover manager if I ever bring this into a real game
		{
			_active.Val = false;
			return;
		}

		var camForward = Camera.main.transform.forward;
		var planeCenter = _headCenter.position - camForward * OffsetMouseFromPlane;
		Plane cursorPlane = new Plane(camForward, planeCenter);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (!cursorPlane.Raycast(ray, out float enter))
		{
			_active.Val = false;
			return;
		}
		var hitPoint = ray.GetPoint(enter);
		if (Vector3.Distance(hitPoint, planeCenter) > MaxDistance && !_forcer.Forcing)
		{
			_active.Val = false;
			return;
		}

		var direction = (hitPoint - _headCenter.transform.position).normalized;
		if (Vector3.Dot(ForwardDir, direction) < MaxDotProduct && !_forcer.Forcing)
		{
			_active.Val = false;
			return;
		}

		Position = hitPoint;
		_active.Val = true;
	}
}
