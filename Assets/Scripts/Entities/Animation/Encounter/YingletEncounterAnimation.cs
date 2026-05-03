using Character.Data;
using Reactivity;
using UnityEngine;

public class YingletEncounterAnimation : ReactiveBehaviour
{
	[SerializeField] AssetReferenceT<PoseId> _defaultPose;

	private IYingletAnimationBridge _animBridge;
	private IMirrorAnimationJobBinder _mirrorBinder;
	private ICharacterEncounterReference _encounterReference;
	Computed<PoseId> _currentEncounterPose;

	private void Start()
	{
		_animBridge = this.GetComponentSafe<IYingletAnimationBridge>();
		_mirrorBinder = this.GetComponentSafe<IMirrorAnimationJobBinder>();

		_encounterReference = this.GetCharacterRootComponent<ICharacterEncounterReference>();
		_currentEncounterPose = CreateComputed(ComputeEncounterPose);
		AddReflector(ReflectPose);
		AddReflector(ReflectMirror);
	}
	private PoseId ComputeEncounterPose()
	{
		var currentEncounter = _encounterReference.Encounter.Val;
		if (currentEncounter == null) return null;
		var pose = currentEncounter.Data.PoseId;
		if (pose == null) return _defaultPose.LoadSync();
		return pose;
	}

	private bool ComputeMirror()
	{
		var currentEncounter = _encounterReference.Encounter.Val;
		if (currentEncounter == null) return false;
		return currentEncounter.Data.Mirror;
	}

	private void ReflectPose()
	{
		_animBridge.SetEncounterPose(_currentEncounterPose.Val?.Clip);
	}

	private void ReflectMirror()
	{
		_mirrorBinder.SetMirror(ComputeMirror());
	}

}
