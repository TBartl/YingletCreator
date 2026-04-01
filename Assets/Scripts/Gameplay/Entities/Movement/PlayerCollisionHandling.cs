using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
public interface IPlayerCollisionHandling
{
	bool CanJump { get; }
	bool Grounded { get; }
	void ClearCanJump();
	UnityAction<float> OnImpactedGround { get; set; }
}
public class PlayerCollisionHandling : MonoBehaviour, IPlayerCollisionHandling
{

	private Rigidbody _rb;

	public const float GROUND_DOT = .7f;
	const float VEL_IMPACT_THRESHOLD = 2f;
	const float COYOTE_TIME = .2f;

	public bool CanJump { get; private set; } = true;
	public bool Grounded { get; private set; }
	public UnityAction<float> OnImpactedGround { get; set; } = delegate { };

	float _lastJumpClearTime = 0; // The player stays on the ground the frame they jump, this prevents that from giving them another jump


	private void Awake()
	{
		_rb = this.GetComponent<Rigidbody>();
	}

	public void ClearCanJump()
	{
		CanJump = false;
		StopClearCanJumpCoroutine();
		_lastJumpClearTime = Time.time;
	}

	private void FixedUpdate()
	{
		Grounded = false;
	}

	void OnCollisionEnter(Collision other)
	{
		HandleCollision(other);
	}

	void OnCollisionStay(Collision other)
	{
		HandleCollision(other);
	}

	void HandleCollision(Collision other)
	{
		bool touchingSolid = other.contacts.Any(contact =>
		{
			//var material = contact.collider.sharedMaterial ?? contact.rigidbody?.sharedMaterial;

			float verticalDot = Vector3.Dot(Vector3.up, contact.normal);

			if (verticalDot > GROUND_DOT && other.relativeVelocity.y > VEL_IMPACT_THRESHOLD)
			{
				OnImpactedGround(other.relativeVelocity.y);
			}

			bool validCollision = verticalDot > GROUND_DOT;

			return validCollision;
		});
		if (touchingSolid && Time.time > _lastJumpClearTime + .01f)
		{
			Grounded = true;
			CanJump = true;

			StopClearCanJumpCoroutine();
			clearCanJumpCoroutine = StartCoroutine(ClearCanJumpAfterTime());
		}
	}

	Coroutine clearCanJumpCoroutine;
	IEnumerator ClearCanJumpAfterTime()
	{
		yield return new WaitForSeconds(COYOTE_TIME);
		CanJump = false;
		clearCanJumpCoroutine = null;
	}
	void StopClearCanJumpCoroutine()
	{
		if (clearCanJumpCoroutine == null) return;
		StopCoroutine(clearCanJumpCoroutine);
		clearCanJumpCoroutine = null;
	}
}