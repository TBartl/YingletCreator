using Character.Creator;
using Reactivity;
using System.Linq;

namespace Character.Compositor
{
	public class TextureApplier : ReactiveBehaviour
	{
		private IMixTextureOrderer _mixTextureOrderer;
		private IMaterialGeneration _materialGeneration;
		private IndividualMaterialTexturerReferences _individualReferences;
		private EnumerableDictReflector<MaterialWithDescription, IndividualMaterialTexturer> _enumerableReflector;

		private void Awake()
		{
			_mixTextureOrderer = Singletons.GetSingleton<IMixTextureOrderer>();
			_materialGeneration = this.GetCompositedYingletComponent<IMaterialGeneration>();
			_individualReferences = new IndividualMaterialTexturerReferences(
				this.GetComponentInParent<ICustomizationDataRepository>(),
				this.GetComponent<ITextureGatherer>(),
				_mixTextureOrderer);
			_enumerableReflector = new(Create, Delete);
			AddReflector(ReflectMaterials);
		}

		private void ReflectMaterials()
		{
			var mats = _materialGeneration.GeneratedMaterialsWithDescription.ToArray();
			_enumerableReflector.Enumerate(mats);
		}

		private IndividualMaterialTexturer Create(MaterialWithDescription description)
		{
			return new IndividualMaterialTexturer(_individualReferences, description);
		}

		private void Delete(IndividualMaterialTexturer texturer)
		{
			texturer.Dispose();
		}
	}
}



/* EnumerableReflector
 *	- Key: IMaterialDescription's key-value-pairs (expose something new?)
 *	- Value: Some instance of a class that does its own computed / reflector
 *	
 *	MaterialTexturer
 *	- Constructor creates a render texture and applies it to a material
 *	- Dispose removes it from the material and destroys the render texturer (and computed?)
 *  - Computes (or EnumerableReflector's?) all the applicable mix textures 
 *  - Reflectively applies them, looking up relevant colors
 *  - Should be regenerated if mix textures change, or if colors change
 */