using Character.Data;
using UnityEngine;


namespace Character.Compositor
{

	[CreateAssetMenu(fileName = "EyeMixTextureReferences", menuName = "Scriptable Objects/Character Compositor/EyeMixTextureReferences")]
	public class EyeMixTextureReferences : ScriptableObject
	{
		[SerializeField] AssetReferenceT<ReColorId> _leftFillReColorIdReference;
		[SerializeField] AssetReferenceT<ReColorId> _rightFillReColorIdReference;
		[SerializeField] AssetReferenceT<ReColorId> _coloredEylidReColorIdReference;

		[SerializeField] TargetMaterialTexture _fillTarget;
		[SerializeField] TargetMaterialTexture _eyelidTarget;
		[SerializeField] AssetReferenceT<MaterialDescription> _leftMaterialDescriptionReference;
		[SerializeField] AssetReferenceT<MaterialDescription> _rightMaterialDescriptionReference;
		[SerializeField] AssetReferenceT<CharacterElementTag> _eyeElementTagReference;

		public ReColorId LeftFillReColorId => _leftFillReColorIdReference.LoadSync();
		public ReColorId RightFillReColorId => _rightFillReColorIdReference.LoadSync();
		public ReColorId ColoredEylidReColorId => _coloredEylidReColorIdReference.LoadSync();

		public TargetMaterialTexture FillTarget => _fillTarget;
		public TargetMaterialTexture EyelidTarget => _eyelidTarget;

		public MaterialDescription LeftMaterialDescription => _leftMaterialDescriptionReference.LoadSync();
		public MaterialDescription RightMaterialDescription => _rightMaterialDescriptionReference.LoadSync();

		public CharacterElementTag EyeElementTag => _eyeElementTagReference.LoadSync();
	}

}