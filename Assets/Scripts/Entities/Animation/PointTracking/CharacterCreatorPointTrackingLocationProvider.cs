using Networking;
using Reactivity;
using UnityEngine;

public interface IPointTrackingLocationProvider
{
	IReadOnlyObservable<bool> Active { get; }
	bool MoveUpperBody { get; }
	Vector3 Position { get; }
}


public class CharacterCreatorPointTrackingLocationProvider : MonoBehaviour, IPointTrackingLocationProvider
{
	[SerializeField] Transform _headCenter;
	[SerializeField] Transform _forwardProvider;

	[SerializeField] float MaxDistance = 1.4f;
	[SerializeField] float OffsetMouseFromPlane = .2f;
	[SerializeField] float MaxDotProduct = 0;


	Observable<bool> _active = new Observable<bool>(false);
	private IUiHoverManager _uiHoverManager;
	private IPointTrackingForcer _forcer;
	private ICharacterIdentity _identity;
	private ICharacterCreatorTracker _characterCreatorTracker;

	public IReadOnlyObservable<bool> Active => _active;
	public Vector3 Position { get; private set; }

	public bool MoveUpperBody => true;

	Vector3 ForwardDir => -_forwardProvider.forward;

	private void Awake()
	{
		// Might be better to feed some of these in via mutator components so this class is less bloated
		_uiHoverManager = Singletons.GetSingleton<IUiHoverManager>();
		_forcer = this.GetComponent<IPointTrackingForcer>(); // This was added for screensaver mode. That logic probably isn't relevant anymore
		_identity = this.GetComponentInParentSafe<ICharacterIdentity>();
		_characterCreatorTracker = Singletons.GetSingleton<ICharacterCreatorTracker>();
	}

	bool Forcing => _forcer != null && _forcer.Forcing;

	void Update()
	{
		if (Input.GetMouseButton(1)) // Might be spinning. Kind of hacky and would need to be made more generic with the ui hover manager if I ever bring this into a real game
		{
			_active.Val = false;
			return;
		}

		if (!_identity.IsActiveAndMine)
		{
			_active.Val = false;
			return;
		}
		if (!_characterCreatorTracker.IsInCharacterCreator.Val)
		{
			_active.Val = false;
			return;
		}

		if (_uiHoverManager.HoveringUi && !Forcing)
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
		if (Vector3.Distance(hitPoint, planeCenter) > MaxDistance && !Forcing)
		{
			_active.Val = false;
			return;
		}

		var direction = (hitPoint - _headCenter.transform.position).normalized;
		if (Vector3.Dot(ForwardDir, direction) < MaxDotProduct && !Forcing)
		{
			_active.Val = false;
			return;
		}

		Position = hitPoint;
		_active.Val = true;
	}
}