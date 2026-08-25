using System.Collections.Generic;
using UnityEngine;

namespace Character.Compositor
{
	public class TextureGatherer_FromToggles : MonoBehaviour, ITextureGathererMutator
	{
		private ICharacterToggleProvider _toggleProvider;

		void Awake()
		{
			_toggleProvider = this.GetComponentInParentSafe<ICharacterToggleProvider>();
		}
		public void Mutate(ref ISet<IMixTexture> set)
		{
			var toggles = _toggleProvider.Toggles;
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