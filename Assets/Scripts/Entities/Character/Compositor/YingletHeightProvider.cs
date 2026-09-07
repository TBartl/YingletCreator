using Character.Compositor;
using UnityEngine;

/// <summary>
/// Provides various values relating to height.
/// Useful for positioning cameras pointing at eye level
/// </summary>
public interface IYingletHeightProvider
{
	/// <summary>
	/// Effectively just returns the scale based on sliders
	/// </summary>
	float HeightBasedOnScale { get; }

	/// <summary>
	/// Also accounts for the stance
	/// </summary>
	float HeightBasedOnStance { get; }
}

public class YingletHeightProvider : MonoBehaviour, IYingletHeightProvider, IInitializable
{
	public float HeightBasedOnScale => _compositedYingletRoot.lossyScale.y;
	public float HeightBasedOnStance => _compositedYingletRoot.lossyScale.y * (1 + _clipProvider.Stance.HeightOffset);
	private Transform _compositedYingletRoot;
	private ICharacterClipProvider _clipProvider;

	public void Initialize()
	{
		_compositedYingletRoot = this.GetComponentInChildren<CompositedYingletRoot>().transform;
		_clipProvider = this.GetComponentInParentSafe<ICharacterClipProvider>();
	}
}
