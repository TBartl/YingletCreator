using Character.Creator;
using Reactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Character.Compositor
{
	internal sealed class IndividualMaterialTexturer : IDisposable
	{
		public static readonly Shader COLORIZE_SHADER = Shader.Find("CharacterCompositor/Colorize");
		public static readonly Shader COLORIZE_WITH_MASK_SHADER = Shader.Find("CharacterCompositor/ColorizeWithMask");

		static readonly int MASK_TEX_PROPERTY_ID = Shader.PropertyToID("_MaskTex");
		static readonly int GRAYSCALE_TEX_PROPERTY_ID = Shader.PropertyToID("_GrayscaleTex");

		static readonly int MAIN_COLOR_PROPERTY_ID = Shader.PropertyToID("_Color");
		static readonly int DARK_COLOR_PROPERTY_ID = Shader.PropertyToID("_DarkColor");

		private IndividualMaterialTexturerReferences _references;
		private MaterialWithDescription _material;
		private EnumerableSetReflector<IMixTexture> _relevantMixTextures;
		private Reflector _reflectRelevantMixTextures;
		private Reflector _reflectTextureComposite;
		private List<RenderTexture> _cachedRenderTextures = new List<RenderTexture>();

		public IndividualMaterialTexturer(IndividualMaterialTexturerReferences references, MaterialWithDescription material)
		{
			_references = references;
			_material = material;

			_relevantMixTextures = new EnumerableSetReflector<IMixTexture>();
			_reflectRelevantMixTextures = new Reflector(ReflectRelevantMixTextures);
			_reflectTextureComposite = new Reflector(ReflectTextureComposite);
		}

		private void ReflectRelevantMixTextures()
		{
			var allTextures = _references.TextureGatherer.AllRelevantTextures.ToArray();
			var relevantTextures = allTextures.Where(t => t.TargetMaterialDescription == _material.Description).ToArray();
			_relevantMixTextures.Enumerate(relevantTextures);
		}

		private void ReflectTextureComposite()
		{
			CleanupRenderTextures();

			var relevantTextures = _relevantMixTextures.Items.ToArray();
			var sortedTextures = SortMixTextures(relevantTextures, _references.MixTexturerOrderer);

			if (_material.Material == null) return;

			// Some materials have multiple textures (notably the eyes). Update them separately
			var buckets = sortedTextures.GroupBy(p => p.TargetMaterialTexture);
			foreach (var bucket in buckets)
			{
				if (!bucket.Any()) continue;
				var rt = UpdateMaterialTexture(bucket.Key, bucket);
				if (rt != null) // Certain targets don't actually create a render texture
				{
					_cachedRenderTextures.Add(rt);
				}
			}
		}

		RenderTexture UpdateMaterialTexture(TargetMaterialTexture materialTexture, IEnumerable<IMixTexture> applicableMixTextures)
		{
			// Special case, use mask directly
			if (materialTexture == TargetMaterialTexture.MouthMask)
			{
				_material.Material.ApplyTexture(applicableMixTextures.First().Grayscale, materialTexture);
				return null;
			}

			Vector2Int largestSizes = applicableMixTextures.Any()
				? new Vector2Int(
					applicableMixTextures.Max(m => m.Grayscale.width),
					applicableMixTextures.Max(m => m.Grayscale.height))
				: new Vector2Int(1, 1);

			var renderTextures = new DoubleBufferedRenderTexture(largestSizes);

			foreach (var applicableMixTexture in applicableMixTextures)
			{
				if (applicableMixTexture.Mask == null)
				{
					_references.BlitMaterial.shader = COLORIZE_SHADER;
				}
				else
				{
					_references.BlitMaterial.shader = COLORIZE_WITH_MASK_SHADER;
					_references.BlitMaterial.SetTexture(MASK_TEX_PROPERTY_ID, applicableMixTexture.Mask);
				}
				_references.BlitMaterial.SetTexture(GRAYSCALE_TEX_PROPERTY_ID, applicableMixTexture.Grayscale);
				ApplyMixTexturePropsToMaterial(_references.BlitMaterial, applicableMixTexture);
				renderTextures.Blit(_references.BlitMaterial);
			}

			var renderTexture = renderTextures.Finalize();
			_material.Material.ApplyTexture(renderTexture, materialTexture);

			// Ensure no render texture is left active
			if (RenderTexture.active == renderTexture)
			{
				RenderTexture.active = null;
			}

			return renderTexture;
		}

		void ApplyMixTexturePropsToMaterial(Material material, IMixTexture mixTexture)
		{
			var reColorId = mixTexture.ReColorId;
			if (reColorId == null)
			{
				material.SetColor(MAIN_COLOR_PROPERTY_ID, Color.white);
				material.SetColor(DARK_COLOR_PROPERTY_ID, Color.black);
				return;
			}

			var colorizeValues = _references.DataRepository.GetColorizeValues(reColorId);

			var main = colorizeValues.Base.GetColor();
			var shade = colorizeValues.Shade.GetColor();

			material.SetColor(MAIN_COLOR_PROPERTY_ID, main);
			material.SetColor(DARK_COLOR_PROPERTY_ID, shade);
		}

		static IEnumerable<IMixTexture> SortMixTextures(IEnumerable<IMixTexture> mixTextures, IMixTextureOrderer mixTextureOrderer)
		{
			var sourceArray = mixTextureOrderer.GetOrderedMixTextures().ToArray();
			var mixTextureToOrder = new Dictionary<IMixTexture, int>();
			for (int i = 0; i < sourceArray.Length; i++)
			{
				mixTextureToOrder[sourceArray[i]] = i;
			}
			return mixTextures.OrderBy(m =>
			{
				if (!m.Sortable) return 0;

				if (mixTextureToOrder.TryGetValue(m, out int index))
				{
					return index;
				}
				Debug.LogWarning($"MixTexture '{m.name}' did not exist in the mix texture ordering. Add it to the `_Order` scriptable object");
				return int.MaxValue;
			}).ToArray();
		}

		void CleanupRenderTextures()
		{
			foreach (var rt in _cachedRenderTextures)
			{
				// Ensure the render texture is not active before releasing
				if (RenderTexture.active == rt)
				{
					RenderTexture.active = null;
				}
				if (rt != null && rt.IsCreated())
				{
					rt.Release();
					GameObject.Destroy(rt);
				}
			}
			_cachedRenderTextures.Clear();
		}

		public void Dispose()
		{
			_reflectTextureComposite.Destroy();
			_reflectRelevantMixTextures.Destroy();
			CleanupRenderTextures();
		}
	}

	internal sealed class IndividualMaterialTexturerReferences : IDisposable
	{
		public IndividualMaterialTexturerReferences(ICustomizationDataRepository dataRepository, ITextureGatherer textureGatherer, IMixTextureOrderer mixTextureOrdererer)
		{
			DataRepository = dataRepository;
			TextureGatherer = textureGatherer;
			MixTexturerOrderer = mixTextureOrdererer;

			BlitMaterial = new Material(IndividualMaterialTexturer.COLORIZE_SHADER);
		}

		public ICustomizationDataRepository DataRepository { get; }
		public ITextureGatherer TextureGatherer { get; }
		public IMixTextureOrderer MixTexturerOrderer { get; }
		public Material BlitMaterial { get; }

		public void Dispose()
		{
			GameObject.Destroy(BlitMaterial);
		}
	}
}