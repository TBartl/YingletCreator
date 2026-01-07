using Character.Creator;
using Reactivity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Character.Compositor
{
	public class TextureGatherer_EyeAdditions : ReactiveBehaviour, ITextureGathererMutator
	{
		[SerializeField] EyeMixTextureReferences _eyeMixTextureReferences;
		private ICustomizationSelectedDataRepository _dataRepo;

		// If the reference ever changes (which it will) we'll need to make this observable
		private Computed<EyeMixTextures> _computedEye;
		private Computed<IEnumerable<IMixTexture>> _computedTextures;

		void Awake()
		{
			_dataRepo = this.GetComponentInParent<ICustomizationSelectedDataRepository>();
			_computedEye = CreateComputed(ComputeEye);
			_computedTextures = CreateComputed(ComputeTextures);
		}

		private EyeMixTextures ComputeEye()
		{

			var toggles = _dataRepo.CustomizationData.ToggleData.Toggles.ToArray();
			foreach (var toggle in toggles)
			{
				if (toggle.EyeTextures)
				{
					return toggle.EyeTextures;
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
