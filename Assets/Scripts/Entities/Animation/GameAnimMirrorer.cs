
using Networking;
using Reactivity;

/// <summary>
/// Responsible for mirroring based on:
/// - Encounter
/// - In character creator
/// - Network identity
/// </summary>
public class GameAnimMirrorer : ReactiveBehaviour
{
	private IMirrorAnimationJobBinder _mirrorBinder;
	private ICharacterEncounterReference _encounterReference;
	private ICharacterCreatorTracker _characterCreatorTracker;
	private ICharacterIdentity _identity;
	private Computed<bool> _mirror;

	private void Start()
	{
		_mirrorBinder = this.GetComponentSafe<IMirrorAnimationJobBinder>();
		_encounterReference = this.GetComponentInParent<ICharacterEncounterReference>(); // Not safe - may be null in lobby
		_characterCreatorTracker = Singletons.GetSingleton<ICharacterCreatorTracker>();
		_identity = this.GetComponentInParentSafe<ICharacterIdentity>();

		_mirror = CreateComputed(ComputeMirror);
		AddReflector(Reflect);
	}

	private bool ComputeMirror()
	{
		// For encounter, we always adhere to the mirror provided by the encounter
		var encounter = _encounterReference?.Encounter?.Val;
		if (encounter != null)
		{
			return encounter.Data.Mirror;
		}

		// If we're in the character creator, no mirror
		if (_characterCreatorTracker.IsInCharacterCreator.Val && _identity.IsActiveAndMine)
		{
			return false;
		}

		// Otherwise, mirror based on the ID, just to make things a little more interesting
		return _identity.NetId % 2 == 0;
	}

	private void Reflect()
	{
		_mirrorBinder.SetMirror(_mirror.Val);
	}
}
