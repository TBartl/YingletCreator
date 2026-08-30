using Reactivity;

public class RegenerateSnapshotOnStatusChange : ReactiveBehaviour
{
	private IYingSnapshotManager _snapshotManager;
	private ICharacterRoot _root;
	private ICharacterStatuses _statuses;

	private void Start()
	{
		_snapshotManager = Singletons.GetSingleton<IYingSnapshotManager>();
		_root = this.GetComponentInParentSafe<ICharacterRoot>();
		_statuses = _root.GetComponentInChildrenSafe<ICharacterStatuses>();

		AddReflector(Reflect);
	}

	private void Reflect()
	{
		// Just called to observe
		// Could cache last and see which statuses are visual, but this doesn't happen enough to care
		var _ = _statuses.Statuses.GetEnumerator();

		_snapshotManager.RegenerateCharacterSnapshot(_root);

	}
}
