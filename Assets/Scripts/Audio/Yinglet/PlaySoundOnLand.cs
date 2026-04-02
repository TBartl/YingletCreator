using UnityEngine;

public class PlaySoundOnLand : MonoBehaviour
{

	private AudioPlayer _audioPlayer;
	private ISurfaceSoundProvider _surfaceSoundProvider;
	private ICharacterCollisionHandling _collisionHandling;
	[SerializeField] float MAX_IMPACT_SPEED = 10f;

	void Start()
	{
		_audioPlayer = Singletons.GetSingleton<AudioPlayer>();
		_surfaceSoundProvider = Singletons.GetSingleton<ISurfaceSoundProvider>();
		_collisionHandling = this.GetCharacterRootComponent<ICharacterCollisionHandling>();
		_collisionHandling.OnImpactedGround += OnImpactedGround;
	}

	private void OnDestroy()
	{
		_collisionHandling.OnImpactedGround -= OnImpactedGround;
	}

	private void OnImpactedGround(PhysicsMaterial material, float speed)
	{
		var sound = _surfaceSoundProvider.GetSound(material, SurfaceSoundType.Landing);
		var options = new AudioPlayOptions()
		{
			Volume = Mathf.Clamp01(speed / MAX_IMPACT_SPEED)
		};
		_audioPlayer.Play(sound, options);
	}
}