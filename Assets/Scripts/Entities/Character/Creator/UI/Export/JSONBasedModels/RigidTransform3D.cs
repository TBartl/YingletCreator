using UnityEngine;

/// <summary>
/// A non-flipped rigid transform in 3D space (Vector3 position and Quaternion rotation).
/// Be sure to NOT use `new RigidTransform3D();` as that will contain an invalid Quaternion, instead use `Identity`.
/// </summary>
public struct RigidTransform3D
{
	public Vector3 position;
	public Quaternion rotation;

	public static readonly RigidTransform3D Identity = new RigidTransform3D(Vector3.zero, Quaternion.identity);
	private const float EPSILON = 1e-6f;

	public RigidTransform3D(Vector3 pos, Quaternion rot)
	{
		position = pos;
		rotation = rot;
	}

	public RigidTransform3D Inverse()
	{
		Quaternion invRot = Quaternion.Inverse(rotation);
		Vector3 invPos = invRot * -position;
		return new RigidTransform3D(invPos, invRot);
	}

	public RigidTransform3D RoundToZero()
	{
		Vector3 pos = new Vector3(
			Mathf.Abs(position.x) < EPSILON ? 0.0f : position.x,
			Mathf.Abs(position.y) < EPSILON ? 0.0f : position.y,
			Mathf.Abs(position.z) < EPSILON ? 0.0f : position.z);
		Quaternion rot = new Quaternion(
			Mathf.Abs(rotation.x) < EPSILON ? 0.0f : rotation.x,
			Mathf.Abs(rotation.y) < EPSILON ? 0.0f : rotation.y,
			Mathf.Abs(rotation.z) < EPSILON ? 0.0f : rotation.z,
			Mathf.Abs(rotation.w) < EPSILON ? 0.0f : rotation.w);
		return new RigidTransform3D(pos, rot);
	}

	public Matrix4x4 ToMatrix4x4()
	{
		Matrix4x4 mat = Matrix4x4.TRS(position, rotation, Vector3.one);
		return mat;
	}

	public RigidTransform3D ToRightHanded()
	{
		return new RigidTransform3D(
			new Vector3(-position.x, position.y, position.z),
			new Quaternion(rotation.x, -rotation.y, -rotation.z, rotation.w));
	}

	public static RigidTransform3D FromUnityGlobalSpin180Y(Transform transform)
	{
		return new Quaternion(0, 1, 0, 0) * new RigidTransform3D(transform.position, transform.rotation);
	}

	public static RigidTransform3D RelativeTransform(RigidTransform3D parentGlobal, RigidTransform3D childGlobal)
	{
		Quaternion parentGlobalInvRot = Quaternion.Inverse(parentGlobal.rotation);
		Vector3 localPos = parentGlobalInvRot * (childGlobal.position - parentGlobal.position);
		Quaternion localRot = parentGlobalInvRot * childGlobal.rotation;
		return new RigidTransform3D(localPos, localRot);
	}

	public static RigidTransform3D operator *(Quaternion parentRot, RigidTransform3D childTransform)
	{
		Vector3 rotatedPos = parentRot * childTransform.position;
		Quaternion rotatedRot = parentRot * childTransform.rotation;
		return new RigidTransform3D(rotatedPos, rotatedRot);
	}
}
