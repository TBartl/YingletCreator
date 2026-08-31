using Character.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Character.Compositor
{
	public interface ICharacterElementTagMask
	{
		IEnumerable<CharacterElementTag> MaskedTags { get; }
	}

	[CreateAssetMenu(fileName = "CharacterElementTagMask", menuName = "Scriptable Objects/Character Compositor/ToggleComponents/CharacterElementTagMask")]
	public class CharacterElementTagMask : CharacterToggleComponent, ICharacterElementTagMask
	{
		[SerializeField] AssetReferenceT<CharacterElementTag>[] _maskedTagReferences;
		public IEnumerable<CharacterElementTag> MaskedTags => _maskedTagReferences.Select(r => r.LoadSync());
	}

}
