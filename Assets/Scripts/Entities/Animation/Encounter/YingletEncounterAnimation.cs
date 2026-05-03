using Character.Data;
using Reactivity;
using UnityEngine;

public class YingletEncounterAnimation : ReactiveBehaviour
{
	[SerializeField] AssetReferenceT<PoseId> _defaultPose;

	private IYingletAnimationBridge _animBridge;
	private ICharacterEncounterReference _encounterReference;
	Computed<PoseId> _currentEncounterPose;

	private void Start()
	{
		_animBridge = this.GetComponentSafe<IYingletAnimationBridge>();
		_encounterReference = this.GetCharacterRootComponent<ICharacterEncounterReference>();
		_currentEncounterPose = CreateComputed(ComputeEncounterPose);
		AddReflector(ReflectPose);
	}

	private PoseId ComputeEncounterPose()
	{
		var currentEncounter = _encounterReference.Encounter.Val;
		if (currentEncounter == null) return null;
		var pose = currentEncounter.Data.PoseId;
		if (pose == null) return _defaultPose.LoadSync();
		return pose;
	}

	private void ReflectPose()
	{
		_animBridge.SetEncounterPose(_currentEncounterPose.Val?.Clip);
	}
}
