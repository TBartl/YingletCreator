using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
public interface ICharacterCollisionHandling
{
	bool CanJump { get; }
	bool Grounded { get; }
	PhysicsMaterial LastGroundMaterial { get; }
	void ClearCanJump();
	UnityAction<PhysicsMaterial, float> OnImpactedGround { get; set; }
}
public class CharacterCollisionHandling : MonoBehaviour, ICharacterCollisionHandling
{

	public const float GROUND_DOT = .7f;
	const float VEL_IMPACT_THRESHOLD = 2f;
	const float COYOTE_TIME = .2f;

	public bool CanJump { get; private set; } = true;
	public bool Grounded { get; private set; }
	public PhysicsMaterial LastGroundMaterial { get; private set; }
	public UnityAction<PhysicsMaterial, float> OnImpactedGround { get; set; } = delegate { };


	float _lastJumpClearTime = 0; // The player stays on the ground the frame they jump, this prevents that from giving them another jump
	bool _wasGrounded;

	public void ClearCanJump()
	{
		CanJump = false;
		StopClearCanJumpCoroutine();
		_lastJumpClearTime = Time.time;
	}

	private void FixedUpdate()
	{
		_wasGrounded = Grounded;
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
		if (Grounded)
		{
			// Something already grounded us this update - don't check for more since we don't want multiple triggers
			return;
		}


		bool touchingSolid = other.contacts.Any(contact =>
		{
			var material = contact.otherCollider.sharedMaterial;

			float verticalDot = Vector3.Dot(Vector3.up, contact.normal);

			if (!_wasGrounded && verticalDot > GROUND_DOT && other.relativeVelocity.y > VEL_IMPACT_THRESHOLD)
			{
				OnImpactedGround(material, other.relativeVelocity.y);
			}

			bool validCollision = verticalDot > GROUND_DOT;

			if (validCollision)
			{
				LastGroundMaterial = material;
			}

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