using UnityEngine;

public class CharacterParticles : MonoBehaviour
{
	[SerializeField] float MAX_LAND_SPEED = 6;
	[SerializeField] float MIN_LAND_SPEED = 4;

	[SerializeField] GameObject _jumpParticles;
	[SerializeField] GameObject _landParticles;
	[SerializeField] GameObject _runParticles;

	IFootstepTracker _footstepTracker;
	ICharacterMovement _movement;
	ICharacterCollisionHandling _collisionHandling;

	void Awake()
	{
		_footstepTracker = this.GetComponentInParent<IFootstepTracker>();
		_movement = this.GetComponentInParent<ICharacterMovement>();
		_collisionHandling = this.GetComponentInParent<ICharacterCollisionHandling>();

		_footstepTracker.OnFootstep += OnFootstep;
		_movement.OnJump += OnJump;
		_collisionHandling.OnImpactedGround += OnImpactedGround;
	}

	private void OnFootstep(Vector3 vector)
	{
		Instantiate(_runParticles, this.transform.position, Quaternion.identity);
	}

	void OnJump(Vector3 position, Vector3 velocity)
	{
		Instantiate(_jumpParticles, position, Quaternion.FromToRotation(Vector3.up, velocity));
	}

	private void OnImpactedGround(PhysicsMaterial material, float speed)
	{
		if (speed < MIN_LAND_SPEED)
		{
			return;
		}
		Vector3 point = this.transform.position;
		Debug.Log("Landed with speed " + speed + " at point " + point);
		//Quaternion rotation = Quaternion.FromToRotation(Vector3.up, (_collisionHandling.transform.position - point).normalized);
		Quaternion rotation = Quaternion.identity;
		GameObject go = Instantiate(_landParticles, point, rotation);
		go.transform.localScale = go.transform.localScale * Mathf.Min(1, speed / MAX_LAND_SPEED);
	}
}