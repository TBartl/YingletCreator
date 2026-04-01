using UnityEngine;

public class RaycastShadowToGround : MonoBehaviour
{
	[SerializeField] private float _raycastDistance = 5f;
	private MeshRenderer _mr;
	private float _initialOffset;
	private int _raycastLayerMask;

	private void Start()
	{
		_mr = this.GetComponent<MeshRenderer>();
		_initialOffset = this.transform.localPosition.y;
		_raycastLayerMask = LayerMask.GetMask("Default");
	}

	private void LateUpdate()
	{
		PositionShadowOnGround();
	}

	private void PositionShadowOnGround()
	{
		// Create a ray pointing downward from the shadow's current position
		Ray ray = new Ray(transform.parent.position + Vector3.up, Vector3.down);

		// Perform the raycast against only the default layer
		if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _raycastLayerMask))
		{
			// Position the shadow at the hit point
			Vector3 newPosition = hit.point;
			newPosition.y += _initialOffset;
			this.transform.position = newPosition;
			_mr.enabled = true;
		}
		else
		{
			_mr.enabled = false;
		}
	}
}
