using JigglePhysics;
using Reactivity;

public class DisableJiggleInEncounter : ReactiveBehaviour
{
	private JiggleRigBuilder _jiggleRigBuilder;
	private ICharacterEncounterReference _encounter;
	private Computed<bool> _inEncounter;

	private void Start()
	{
		_jiggleRigBuilder = this.GetComponentSafe<JiggleRigBuilder>();
		_encounter = this.GetCharacterRootComponent<ICharacterEncounterReference>();
		_inEncounter = CreateComputed(() => _encounter.Encounter.Val != null);
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		_jiggleRigBuilder.enabled = !_inEncounter.Val;
	}
}
