/// <summary>
/// To be used in tandem with the SafeDependencyUtils 
/// This allows for a component to be lazy-initialized only when something does a GetComponentSafe for it
/// This provides a few benefits:
/// - Early protection against null references (GetComponent doesn't do this)
/// - No concern over race conditions
/// - Only needed components will be initialized
/// </summary>
public interface IInitializable
{
	void Initialize();
}