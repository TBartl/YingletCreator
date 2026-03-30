

using Reactivity;
using UnityEngine;

internal class CameraControlProvider_FollowPlayer : ReactiveBehaviour, ICameraControlProvider
{
	public bool WantsControl => true; // This is effectively the default for now (unless player despawns or something idk we'll figure that out)

	public (Vector3, Quaternion) CalculateTransform()
	{
		return (Vector3.zero, Quaternion.identity);
	}
}
