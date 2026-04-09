using Character.Compositor;
using Character.Data;
using Reactivity;
namespace Character.Creator
{
	/// <summary>
	/// Returns observable data associated to a single character
	/// This is often times implemented at the root of the character
	/// Multiple can exist at a time
	/// </summary>
	public interface ICustomizationDataRepository
	{
		ObservableCustomizationData CustomizationData { get; }
	}


	public static class CustomizationDataRepositoryExtensionMethods
	{
		public static float GetSliderValue(this ICustomizationDataRepository dataRepo, CharacterSliderId id)
		{

			if (dataRepo.CustomizationData.SliderData.SliderValues.TryGetValue(id, out Observable<float> value))
			{
				return value.Val;
			}
			return 0.5f;
		}
		public static void SetSliderValue(this ICustomizationDataRepository dataRepo, CharacterSliderId id, float value)
		{
			ObservableDictUtils<CharacterSliderId, float>.SetOrUpdate(dataRepo.CustomizationData.SliderData.SliderValues, id, value);
		}
		public static IColorizeValues GetColorizeValues(this ICustomizationDataRepository dataRepository, ReColorId id)
		{
			if (dataRepository.CustomizationData.ColorData.ColorizeValues.TryGetValue(id, out Observable<IColorizeValues> values))
			{
				return values.Val;
			}
			return id.ColorGroup.DefaultColors;
		}
		public static void SetColorizeValues(this ICustomizationDataRepository dataRepo, ReColorId id, IColorizeValues values)
		{
			ObservableDictUtils<ReColorId, IColorizeValues>.SetOrUpdate(dataRepo.CustomizationData.ColorData.ColorizeValues, id, values);
		}

		public static bool GetToggle(this ICustomizationDataRepository dataRepo, CharacterToggleId id)
		{
			return dataRepo.CustomizationData.GetToggle(id);
		}
		public static void FlipToggle(this ICustomizationDataRepository dataRepo, CharacterToggleId id)
		{
			dataRepo.CustomizationData.FlipToggle(id);
		}

		public static int GetInt(this ICustomizationDataRepository dataRepo, CharacterIntId id)
		{
			return dataRepo.CustomizationData.NumberData.GetInt(id);
		}
		public static void SetInt(this ICustomizationDataRepository dataRepo, CharacterIntId id, int value)
		{
			ObservableDictUtils<CharacterIntId, int>.SetOrUpdate(dataRepo.CustomizationData.NumberData.IntValues, id, value);
		}
	}
}
