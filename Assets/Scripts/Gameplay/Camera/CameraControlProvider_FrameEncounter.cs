

using Reactivity;
using UnityEngine;

internal class CameraControlProvider_FrameEncounter : ReactiveBehaviour, ICameraControlProvider, IInitializable
{
	private IActiveEncounterProvider _activeEncounterProvider;
	private Computed<Transform> _encounterTransform;

	public bool WantsControl => _encounterTransform.Val != null;

	public (Vector3, Quaternion) CalculateTransform()
	{
		var encounterTransform = _encounterTransform.Val;
		if (encounterTransform == null) return (Vector3.zero, Quaternion.identity);
		return (encounterTransform.position, encounterTransform.rotation);
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
