using Character.Compositor;
using UnityEngine;

public class PupilOffsetMutator_FollowSway : MonoBehaviour, IPupilOffsetMutator
{
	[SerializeField] Transform _headBone;
	[SerializeField] float _degreesToOffsetMult = .1f;

	private Transform _root;
	private IPointTrackingWeightProvider _weightProvider;
	Vector3 _originalForward;

	void Awake()
	{
		_root = this.GetComponentInParent<CompositedYingletRoot>().transform;
		_weightProvider = this.GetComponentInParent<IPointTrackingWeightProvider>();
		_originalForward = GetHeadForward();
	}

	private Vector3 GetHeadForward()
	{
		return _root.InverseTransformVector(_headBone.forward);
	}

	Vector2 CalculateOffset()
	{
		var angle = GetYRotation(_originalForward, GetHeadForward());
		if (angle > 180) angle -= 360;

		var weight = Mathf.Pow(1 - _weightProvider.Weight, 4); // Lower this significantly if we're doing any sort of looking. If we don't do this Pow, it snaps weirdly
		return new Vector2(angle * _degreesToOffsetMult, 0) * weight;
	}

	public static float GetYRotation(Vector3 from, Vector3 to)
	{
		// Get the angle in radians between the two vectors on the XY plane
		float angle = Mathf.Atan2(to.x, to.z) - Mathf.Atan2(from.x, from.z);

		// Convert to degrees
		return Mathf.Rad2Deg * angle;
	}

	public PupilOffsets Mutate(PupilOffsets input)
	{
		var offset = CalculateOffset();
		return input.ShiftBothBy(offset);
	}
}
