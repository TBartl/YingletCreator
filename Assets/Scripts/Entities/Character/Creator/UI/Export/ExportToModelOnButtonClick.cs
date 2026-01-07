using UnityEngine;

public class ExportToModelOnButtonClick : ExportOnButtonClickBase
{
	private enum ExportJSONModelFormat
	{
		NONE,
		G3MF, // .g3b or .g3tf if shift is pressed
		GLTF, // .glb or .gltf if shift is pressed
		VRM_0_x, // .vrm 0.x or .gltf if shift is pressed
		VRM_1_0, // .vrm 1.0 or .gltf if shift is pressed
	}
	[SerializeField] private ExportJSONModelFormat _exportFormat = ExportJSONModelFormat.NONE;
	[SerializeField] private JigglePhysics.JiggleRigBuilder _jiggleRigBuilder;
	[SerializeField] private Texture2D _pupilTexture;
	[SerializeField] private TMPro.TMP_Dropdown _meshOptimizationDropdown;
	[SerializeField] private TMPro.TMP_Dropdown _imageFormatDropdown;
	[SerializeField] private TMPro.TMP_Dropdown _floatPrecisionDropdown;
	private string _fileExtension = ".glb";
	private bool _useTextFormat = false;

	private void Update()
	{
		_useTextFormat = IsAnyModifierPressed();
		string binaryExtension = ".glb";
		string textExtension = ".gltf";
		switch (_exportFormat)
		{
			case ExportJSONModelFormat.NONE:
				Debug.LogError("The button needs to have a valid export format selected.");
				return;
			case ExportJSONModelFormat.G3MF:
				binaryExtension = ".g3b";
				textExtension = ".g3tf";
				break;
			case ExportJSONModelFormat.GLTF:
				binaryExtension = ".glb";
				textExtension = ".gltf";
				break;
			case ExportJSONModelFormat.VRM_0_x:
			case ExportJSONModelFormat.VRM_1_0:
				binaryExtension = ".vrm";
				textExtension = ".gltf";
				break;
		}
		if (_useTextFormat)
		{
			_tooltip.SetText(_tooltip.Text.Replace(binaryExtension, textExtension));
			_fileExtension = textExtension;
		}
		else
		{
			_tooltip.SetText(_tooltip.Text.Replace(textExtension, binaryExtension));
			_fileExtension = binaryExtension;
		}
	}

	protected override void OnExportButtonClicked()
	{
		if (_yingletRoot is null)
		{
			Debug.LogError("Yinglet root is not assigned.");
			return;
		}
		// Prepare data.
		ModelMaterial.pupilTexture = _pupilTexture;
		MeshObjectOptimization meshObjOpt = (MeshObjectOptimization)_meshOptimizationDropdown.value;
		EyeExpression eyeExp = GetEyeExpression();
		// Begin export.
		ModelDocument yinglet = ModelDocument.CreateFromYingletSkeleton(_skeletonHips);
		yinglet.SetExplicitBoneLengths(GetHeadAntennaeLength(), GetHeadEarLength());
		yinglet.ConvertYingletMeshes(_yingletRoot, meshObjOpt, eyeExp);
		yinglet.SetRootNodeName(GetCharacterNodeName());
		yinglet.ExportSpringRigs(_jiggleRigBuilder);
		ModelBaseFormat baseFormat = ModelBaseFormat.GLTF;
		switch (_exportFormat)
		{
			case ExportJSONModelFormat.NONE:
				Debug.LogError("The button needs to have a valid export format selected.");
				return;
			case ExportJSONModelFormat.G3MF:
				baseFormat = ModelBaseFormat.G3MF;
				break;
			case ExportJSONModelFormat.VRM_0_x:
				baseFormat = ModelBaseFormat.GLTF;
				yinglet.MessEverythingUpForStupidVRM0xRequirements();
				break;
			case ExportJSONModelFormat.GLTF:
			case ExportJSONModelFormat.VRM_1_0:
				baseFormat = ModelBaseFormat.GLTF;
				break;
		}
		SetFloatPrecisionForModelAccessors(baseFormat, _floatPrecisionDropdown.value);
		yinglet.PerformOptionalCleanups();
		yinglet.EncodeMeshDataIntoAccessors(baseFormat);
		yinglet.EncodeAnimationAccessors(baseFormat);
		yinglet.EncodeTextures(_imageFormatDropdown.value);
		yinglet.EncodeThumbnail(GetThumbnailTexture(), _imageFormatDropdown.value);
		string savePath = GetSavePath();
		// Note: The non-VRM 0.x formats all include the VRM 1.0 metadata.
		// This is because VRM 1.0 is a clean superset of the standard rig, so
		// there is no downside to including the data, and it could be used outside
		// of VRM applications, like reading the toon materials or spring bone data.
		switch (_exportFormat)
		{
			case ExportJSONModelFormat.NONE:
				Debug.LogError("The button needs to have a valid export format selected.");
				return;
			case ExportJSONModelFormat.G3MF:
				yinglet.ExportToG3MF(savePath + _fileExtension);
				break;
			case ExportJSONModelFormat.GLTF:
				yinglet.ExportToGLTF(savePath + _fileExtension);
				break;
			case ExportJSONModelFormat.VRM_0_x:
				yinglet.ExportToGLTF(savePath + "_vrm0" + _fileExtension, 0);
				break;
			case ExportJSONModelFormat.VRM_1_0:
				yinglet.ExportToGLTF(savePath + "_vrm1" + _fileExtension, 1);
				break;
		}
		EmitExportEvent();
	}

	private static void SetFloatPrecisionForModelAccessors(ModelBaseFormat format, int floatPrecisionDropdownValue)
	{
		if (floatPrecisionDropdownValue == 0) // Automatic
		{
			// Automatic based on format: G3MF = float16, glTF = float32.
			floatPrecisionDropdownValue = (format == ModelBaseFormat.G3MF) ? 1 : 2;
		}
		if (floatPrecisionDropdownValue == 1) // Force 16-bit (half)
		{
			ModelAccessor.preferredFloatComponentType = "float16";
		}
		else if (floatPrecisionDropdownValue == 2) // Force 32-bit (single)
		{
			ModelAccessor.preferredFloatComponentType = "float32";
		}
	}

	private static bool IsAnyModifierPressed()
	{
		return (
			Input.GetKey(KeyCode.LeftShift) ||
			Input.GetKey(KeyCode.RightShift) ||
			Input.GetKey(KeyCode.LeftControl) ||
			Input.GetKey(KeyCode.RightControl) ||
			Input.GetKey(KeyCode.LeftAlt) ||
			Input.GetKey(KeyCode.RightAlt) ||
			Input.GetKey(KeyCode.LeftCommand) ||
			Input.GetKey(KeyCode.RightCommand) ||
			Input.GetKey(KeyCode.LeftWindows) ||
			Input.GetKey(KeyCode.RightWindows));
	}
}
