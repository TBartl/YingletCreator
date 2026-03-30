using UnityEngine;

public static class VectorExtensionMethods
{
	public static Vector3 Multiply(this Vector3 v1, Vector3 v2)
	{
		return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
	}

	public static Vector3 GetReciprocal(this Vector3 v)
	{
		return new Vector3(
			v.x != 0 ? 1f / v.x : 0f,
			v.y != 0 ? 1f / v.y : 0f,
			v.z != 0 ? 1f / v.z : 0f
		);
	}

	/// <summary>
	/// When you call Quaternion.eulerAngles, you get a value between 0 and 360 for each axis
	/// This changes the range to -180 to 180, which is often more useful for determining shortest rotation direction
	/// </summary>
	public static Vector3 DirectionalizeEulerAngles(this Vector3 eulerAngles)
	{
		return new Vector3(
			eulerAngles.x > 180f ? eulerAngles.x - 360f : eulerAngles.x,
			eulerAngles.y > 180f ? eulerAngles.y - 360f : eulerAngles.y,
			eulerAngles.z > 180f ? eulerAngles.z - 360f : eulerAngles.z
		);
	}
	public static Quaternion SmoothDamp(this Quaternion current, Quaternion target, ref Vector4 currentVelocity, float smoothTime)
	{
		// Convert to vector form for damping
		Vector4 curr = new Vector4(current.x, current.y, current.z, current.w);
		Vector4 targ = new Vector4(target.x, target.y, target.z, target.w);

		// Ensure shortest path
		if (Vector4.Dot(curr, targ) < 0.0f)
		{
			targ = -targ;
		}

		Vector4 result = new Vector4(
			Mathf.SmoothDamp(curr.x, targ.x, ref currentVelocity.x, smoothTime),
			Mathf.SmoothDamp(curr.y, targ.y, ref currentVelocity.y, smoothTime),
			Mathf.SmoothDamp(curr.z, targ.z, ref currentVelocity.z, smoothTime),
			Mathf.SmoothDamp(curr.w, targ.w, ref currentVelocity.w, smoothTime)
		);

		result.Normalize();
		return new Quaternion(result.x, result.y, result.z, result.w);
	}

	public static Vector3 WithoutY(this Vector3 vector)
	{
		return new Vector3(vector.x, 0, vector.z);
	}

	public static Quaternion SmoothTo(this Quaternion from, Quaternion to, float sharpness, float dt)
	{
		// exponential smoothing (frame-rate independent)
		float t = 1f - Mathf.Exp(-sharpness * dt);
		return Quaternion.Slerp(from, to, t);
	}
}
