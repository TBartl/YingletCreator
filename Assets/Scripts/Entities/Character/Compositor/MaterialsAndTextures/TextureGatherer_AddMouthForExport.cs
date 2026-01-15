using Character.Data;
using Reactivity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Character.Compositor
{
	/// <summary>
	/// For export, we are adding a slap on mouth mesh. We want it to use the same texture that the head uses
	/// </summary>
	public class TextureGatherer_AddMouthForExport : ReactiveBehaviour, ITextureGathererMutator
	{
		[SerializeField] MenuType _menuType;
		[SerializeField] AssetReferenceT<MeshWithMaterial> _slapOnMouth;
		[SerializeField] AssetReferenceT<MaterialDescription> _slapOnMouthMaterial;
		private IMenuManager _menuManager;
		private Computed<bool> _onExportMenu;

		void Awake()
		{
			_menuManager = Singletons.GetSingleton<IMenuManager>();
			_onExportMenu = CreateComputed(ComputeOnExportMenu);
		}

		bool ComputeOnExportMenu()
		{
			return _menuManager.OpenMenu.Val == _menuType;
		}

		public void Mutate(ref ISet<IMixTexture> set)
		{
			if (!_onExportMenu.Val)
			{
				return;
			}

			// There might be multiple mouth textures for things like lipstick. Or none at all if the user hasn't selected anything
			var mouthTextures = set.Where(mixTex => mixTex.TargetMaterialTexture == TargetMaterialTexture.Mouth).ToList();
			foreach (var mouthTexture in mouthTextures)
			{
				set.Add(new SlapOnMouthTexture(mouthTexture, _slapOnMouthMaterial.LoadSync()));
			}
		}
	}

	sealed class SlapOnMouthTexture : IMixTexture
	{
		private readonly IMixTexture _source;

		public SlapOnMouthTexture(IMixTexture source, MaterialDescription slapOnMouthMaterial)
		{
			_source = source;
			TargetMaterialDescription = slapOnMouthMaterial;
		}

		public ReColorId ReColorId => _source.ReColorId;

		public MaterialDescription TargetMaterialDescription { get; }

		public Texture2D Grayscale => _source.Grayscale;

		public Texture2D Mask => null;
		public string name => nameof(SlapOnMouthTexture);

		public bool Sortable => false;

		public TargetMaterialTexture TargetMaterialTexture => TargetMaterialTexture.MainTexture;

		public IEnumerable<CharacterElementTag> Tags => Enumerable.Empty<CharacterElementTag>();
	}
}