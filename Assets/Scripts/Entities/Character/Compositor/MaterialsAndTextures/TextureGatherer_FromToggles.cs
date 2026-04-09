using Character.Creator;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Character.Compositor
{
	public class TextureGatherer_FromToggles : MonoBehaviour, ITextureGathererMutator
	{
		private ICustomizationDataRepository _dataRepo;

		void Awake()
		{
			_dataRepo = this.GetComponentInParent<ICustomizationDataRepository>();
		}
		public void Mutate(ref ISet<IMixTexture> set)
		{
			var toggles = _dataRepo.CustomizationData.ToggleData.Toggles.ToArray();
			foreach (var toggle in toggles)
			{
				foreach (var tex in toggle.AddedTextures)
				{
					set.Add(tex);
				}
			}
		}
	}
}