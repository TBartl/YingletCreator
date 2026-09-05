using UnityEditor;

namespace Snapshotter
{
	public sealed class SnapshotterGatherPortraitIcons
	{

		[MenuItem("Custom/Snapshotter/Generate Built-In Portrait Icons")]
		public static void GeneratePortraitIcons()
		{
			SnapshotToSpriteSheetUtils.GeneratePortraitIcons(ModDefinitionUtils.GetBuiltinModDefinition());
			SnapshotToSpriteSheetUtils.UpdateIconsInScene();
		}
	}
}