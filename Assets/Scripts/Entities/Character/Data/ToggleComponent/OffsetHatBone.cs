using UnityEngine;

namespace Character.Data
{
	public interface IOffsetHatBone
	{
		public float Amount { get; }
	}
	[CreateAssetMenu(fileName = "OffsetHatBone", menuName = "Scriptable Objects/Character Data/ToggleComponents/OffsetHatBone")]
	public class OffsetHatBone : CharacterToggleComponent, IOffsetHatBone
	{
		[SerializeField] float _amount;
		public float Amount => _amount;
	}
}