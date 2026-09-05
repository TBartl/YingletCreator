using Reactivity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Character.Compositor
{
	public class TextureGatherer_EyeAdditions : ReactiveBehaviour, ITextureGathererMutator
	{
		[SerializeField] EyeMixTextureReferences _eyeMixTextureReferences;
		private ICharacterToggleProvider _toggleProvider;

		// If the reference ever changes (which it will) we'll need to make this observable
		private Computed<AddEyeMixTextures> _computedEye;
		private Computed<IEnumerable<IMixTexture>> _computedTextures;

		void Awake()
		{
			_toggleProvider = this.GetComponentInParentSafe<ICharacterToggleProvider>();
			_computedEye = CreateComputed(ComputeEye);
			_computedTextures = CreateComputed(ComputeTextures);
		}

		private AddEyeMixTextures ComputeEye()
		{
			var toggles = _toggleProvider.Toggles;
			foreach (var toggle in toggles.Reverse())
			{
				foreach (var component in toggle.Components)
				{
					if (component is AddEyeMixTextures eyeComponent)
					{
						return eyeComponent;
					}
				}
			}
			return null;
		}
		private IEnumerable<IMixTexture> ComputeTextures()
		{
			var eyeTextures = _computedEye.Val;
			if (eyeTextures == null) return Enumerable.Empty<IMixTexture>();
			return eyeTextures.GenerateMixTextures(_eyeMixTextureReferences).ToArray(); ;
		}

		public void Mutate(ref ISet<IMixTexture> set)
		{
			var textures = _computedTextures.Val;
			foreach (var texture in textures)
			{
				set.Add(texture);
			}
		}
	}
}
