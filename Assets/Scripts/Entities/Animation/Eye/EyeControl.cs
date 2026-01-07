using UnityEngine;

internal interface IEyeControl
{
	bool IdleEyeMovementEnabled { get; set; }
}

internal sealed class EyeControl : MonoBehaviour, IEyeControl
{
	public bool IdleEyeMovementEnabled { get; set; } = true;

}
