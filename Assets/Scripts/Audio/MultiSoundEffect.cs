using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffect", menuName = "Scriptable Objects/Sound/MultiSoundEffect")]
public class MultiSoundEffect : SoundEffectBase
{
	[SerializeField] AudioClip[] _clips;
	[SerializeField][Range(0, 1)] float _volume = 1;
	[SerializeField][Tooltip("Values between 0 and 3, with 1 being no pitch shift")] Vector2 _randomPitchRange = Vector2.one;

	public override string Name => name;
	public override AudioClip Clip => _clips[UnityEngine.Random.Range(0, _clips.Length)];
	public override float Volume => _volume;
	public override Vector2 RandomPitchRange => _randomPitchRange;
}