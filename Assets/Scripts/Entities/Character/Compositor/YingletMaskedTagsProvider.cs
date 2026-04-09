using Character.Creator;
using Reactivity;
using System.Collections.Generic;
using System.Linq;

namespace Character.Compositor
{
	/// <summary>
	/// Mantains a running set of the masked tags based on the toggles
	/// </summary>
	public interface IYingletMaskedTagsProvider
	{
		IEnumerable<CharacterElementTag> MaskedTags { get; }
	}

	public class YingletMaskedTagsProvider : ReactiveBehaviour, IYingletMaskedTagsProvider
	{
		EnumerableSetReflector<CharacterElementTag> _maskedTags = new();
		private ICustomizationDataRepository _dataRepository;

		public IEnumerable<CharacterElementTag> MaskedTags => _maskedTags.Items;

		void Awake()
		{
			_dataRepository = GetComponentInParent<ICustomizationDataRepository>();
			AddReflector(ComputeMaskedTags);
		}

		private void ComputeMaskedTags()
		{
			var set = new HashSet<CharacterElementTag>();
			var toggles = _dataRepository.CustomizationData.ToggleData.Toggles.ToArray();
			foreach (var toggle in toggles)
			{
				foreach (var component in toggle.Components)
				{
					if (component is ICharacterElementTagMask mask)
					{
						foreach (var tag in mask.MaskedTags)
						{
							set.Add(tag);
						}
					}
				}
			}
			_maskedTags.Enumerate(set);
		}
	}
}