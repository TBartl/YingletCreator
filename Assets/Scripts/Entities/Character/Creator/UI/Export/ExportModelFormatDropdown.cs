
using Reactivity;

internal class ExportModelFormatDropdown : ReactiveDropdown<ExportModelFormat>
{
	Observable<ExportModelFormat> _format = new Observable<ExportModelFormat>(ExportModelFormat.BLENDER);

	protected override ExportModelFormat Value { get => _format.Val; set => _format.Val = value; }

	public ExportModelFormat CurrentFormat => _format.Val;

	protected override MenuSettingsDropdownOption[] GetAllOptions()
	{
		return new MenuSettingsDropdownOption[]
		{
			new MenuSettingsDropdownOption("Blender (.blend)", ExportModelFormat.BLENDER),
			new MenuSettingsDropdownOption("G3MF (.g3b)",      ExportModelFormat.G3MF),
			new MenuSettingsDropdownOption("glTF (.glb)",      ExportModelFormat.GLTF),
			new MenuSettingsDropdownOption("VRM 0.X (.vrm)",   ExportModelFormat.VRM_0_x),
			new MenuSettingsDropdownOption("VRM 1.0 (.vrm)",   ExportModelFormat.VRM_1_0),
		};
	}
}
