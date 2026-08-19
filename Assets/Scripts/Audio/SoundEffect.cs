using UnityEngine;

/// <summary>
/// Wrapper around AudioClip with some extra configurations
/// </summary>
public interface ISoundEffect
{
	string Name { get; }
	AudioClip Clip { get; }
	float Volume { get; }
	Vector2 RandomPitchRange { get; }
}

public abstract class SoundEffectBase : ScriptableObject, ISoundEffect
{
	public abstract string Name { get; }
	public abstract AudioClip Clip { get; }
	public abstract float Volume { get; }
	public abstract Vector2 RandomPitchRange { get; }
}

[CreateAssetMenu(fileName = "SoundEffect", menuName = "Scriptable Objects/Sound/SoundEffect")]
public class SoundEffect : SoundEffectBase
{
	[SerializeField] AudioClip _clip;
	[SerializeField][Range(0, 1)] float _volume = 1;
	[SerializeField][Tooltip("Values between 0 and 3, with 1 being no pitch shift")] Vector2 _randomPitchRange = Vector2.one;

	public override string Name => name;
	public override AudioClip Clip => _clip;
	public override float Volume => _volume;
	public override Vector2 RandomPitchRange => _randomPitchRange;
}
