using Character.Data;
using Reactivity;

namespace Character.Creator
{
	internal static class CustomizationDataUpgradeUtils
	{
		public static void UpgradeIfNeeded(ObservableCustomizationData data, int version, ICompositeResourceLoader resourceLoader)
		{
			// We need to disable reactivity, since this can read from some of the observable data
			using var disabler = new ReactivityTrackingSuspender();

			// Max version number stored in <see cref="SerializableCustomizationData"/> 
			if (version <= 0)
			{
				// Added ear and whisker toggles
				var earToggle = resourceLoader.Load<CharacterToggleId>("ba34ff2f204a7044d927022c326264b0");
				var whiskerToggle = resourceLoader.Load<CharacterToggleId>("67335fedf8c1d5b45b5e5d7f6781a70f");
				data.ToggleData.Toggles.Add(earToggle);
				data.ToggleData.Toggles.Add(whiskerToggle);
			}
			if (version <= 1)
			{
				// Add antenna toggle
				var antennaToggle = resourceLoader.Load<CharacterToggleId>("8cba46f50096a834fb85f242c51bb822");
				data.ToggleData.Toggles.Add(antennaToggle);
			}

			if (version <= 2)
			{
				// Fixing bug: FurPattern - Lemur (Head) was using incorrect RecolorID
				var dorsalLargeRecolorId = resourceLoader.Load<ReColorId>("446eca6588d0cd549ae8ddce16157ed3");
				var lemurHeadRecolorId = resourceLoader.Load<ReColorId>("6b70aecca43a2ad41ad199c07badec8b");

				if (data.ColorData.ColorizeValues.TryGetValue(dorsalLargeRecolorId, out var existingValues))
				{
					data.ColorData.ColorizeValues[lemurHeadRecolorId] = existingValues;
				}
			}
		}
	}
}
