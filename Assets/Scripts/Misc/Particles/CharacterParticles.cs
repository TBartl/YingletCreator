using UnityEngine;

public class CharacterParticles : MonoBehaviour
{
	[SerializeField] float DISTANCE_BETWEEN_RUN_PARTICLES = .3f;
	[SerializeField] float MAX_LAND_SPEED = 6;

	[SerializeField] GameObject _jumpParticles;
	[SerializeField] GameObject _landParticles;
	[SerializeField] GameObject _runParticles;

	ICharacterMovement _movement;
	ICharacterCollisionHandling _collisionHandling;
	Rigidbody rb;

	float distanceUntilRunParticle;

	void Awake()
	{
		_movement = this.GetComponentInParent<ICharacterMovement>();
		_collisionHandling = this.GetComponentInParent<ICharacterCollisionHandling>();
		rb = this.GetComponentInParent<Rigidbody>();

		_movement.OnJump += OnJump;
		_collisionHandling.OnImpactedGround += OnImpactedGround;
	}


	void LateUpdate()
	{
		distanceUntilRunParticle -= Mathf.Abs(rb.linearVelocity.x) * Time.deltaTime;
		if (_collisionHandling.Grounded && distanceUntilRunParticle < 0)
		{
			//Vector3 point = _collisionHandling.LastGroundedCollisionPoint;
			Vector3 point = this.transform.position;
			Instantiate(_runParticles, point, Quaternion.identity);
			distanceUntilRunParticle = DISTANCE_BETWEEN_RUN_PARTICLES;
		}
	}
	void OnJump(Vector3 velocity)
	{
		Instantiate(_jumpParticles, this.transform.position, Quaternion.FromToRotation(Vector3.up, rb.linearVelocity));
	}

	private void OnImpactedGround(PhysicsMaterial material, float speed)
	{
		Vector3 point = this.transform.position;
		//Quaternion rotation = Quaternion.FromToRotation(Vector3.up, (_collisionHandling.transform.position - point).normalized);
		Quaternion rotation = Quaternion.identity;
		GameObject go = Instantiate(_landParticles, point, rotation);
		go.transform.localScale = go.transform.localScale * Mathf.Min(1, speed / MAX_LAND_SPEED);
	}
}