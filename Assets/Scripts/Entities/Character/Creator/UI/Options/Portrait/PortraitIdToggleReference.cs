using Character.Data;
using UnityEngine;

namespace Character.Creator.UI
{
	public interface ICharacterCreatorTogglePortraitIdReference : ICharacterCreatorToggleReference
	{
		PortraitId PortraitId { get; set; }
	}
	public class PortraitIdToggleReference : MonoBehaviour, ICharacterCreatorTogglePortraitIdReference
	{
		public PortraitId PortraitId { get; set; }

		public string DisplayName => PortraitId.DisplayName;

		public Sprite Sprite => PortraitId.Preview.Sprite;
	}
}