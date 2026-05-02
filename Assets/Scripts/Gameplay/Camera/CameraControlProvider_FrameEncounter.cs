

using Reactivity;
using UnityEngine;

internal class CameraControlProvider_FrameEncounter : ReactiveBehaviour, ICameraControlProvider, IInitializable
{
	private IActiveEncounterProvider _activeEncounterProvider;
	private Computed<Transform> _encounterTransform;
	private Vector3 _pos;
	private Quaternion _rot;

	public bool WantsControl => _encounterTransform.Val != null;

	public (Vector3, Quaternion) CalculateTransform()
	{
		var encounterTransform = _encounterTransform.Val;
		if (encounterTransform != null)
		{
			_pos = encounterTransform.position;
			_rot = encounterTransform.rotation;
		}
		return (_pos, _rot);
	}

	public void Initialize()
	{
		_activeEncounterProvider = Singletons.GetSingleton<IActiveEncounterProvider>();
		_encounterTransform = CreateComputed(ComputeEncounterTransform);

	}

	private Transform ComputeEncounterTransform()
	{
		var activeEncounter = _activeEncounterProvider.ActiveEncounter.Val;
		if (activeEncounter == null) return null;
		return activeEncounter.EncounterSource.GetComponentInChildren<EncounterCameraPosition>()?.transform;
	}
}
