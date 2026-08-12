using Character.Compositor;
using Character.Data;
using Reactivity;
using System;
using UnityEngine;

namespace Character.Creator
{
	public sealed class ObservableCustomizationData
	{
		public Observable<string> Name { get; } = new();
		public ObservableCustomizationSliderData SliderData { get; }
		public ObservableCustomizationColorData ColorData { get; }
		public ObservableCustomizationToggleData ToggleData { get; }
		public ObservableCustomizationNumberData NumberData { get; }
		public ObservableCustomizationPortraitData PortraitData { get; }
		public ObservableCustomizationGenderData GenderData { get; }
		public DateTime CreationTime { get; }

		public ObservableCustomizationData(SerializableCustomizationData serializableData, ICompositeResourceLoader resourceLoader)
		{
			Name.Val = serializableData.Name;
			CreationTime = serializableData.CreationTime;
			SliderData = new(serializableData.SliderData, resourceLoader);
			ColorData = new(serializableData.ColorData, resourceLoader);
			ToggleData = new(serializableData.ToggleData, resourceLoader);
			NumberData = new(serializableData.NumberData, resourceLoader);
			PortraitData = new(serializableData.PortraitData, resourceLoader);
			GenderData = new(serializableData.GenderData, resourceLoader);
			CustomizationDataUpgradeUtils.UpgradeIfNeeded(this, serializableData.Version, resourceLoader);
		}
	}

	public sealed class ObservableCustomizationSliderData
	{
		public ObservableCustomizationSliderData(SerializableCustomizationSliderData sliderData, ICompositeResourceLoader resourceLoader)
		{
			if (sliderData?.SliderValues == null) return;
			foreach (var sliderValue in sliderData.SliderValues)
			{
				var key = resourceLoader.Load<CharacterSliderId>(sliderValue.Id);
				if (key == null)
				{
					Debug.LogWarning($"Skipping unknown slider {key}");
					continue;
				}
				SliderValues[key] = new(sliderValue.Value);
			}
		}

		/// <summary>
		/// The float value is always in the range of 0 to 1
		/// </summary>
		public ObservableDict<CharacterSliderId, Observable<float>> SliderValues { get; } = new();
	}

	public sealed class ObservableCustomizationColorData
	{
		public ObservableCustomizationColorData(SerializableCustomizationColorData colorData, ICompositeResourceLoader resourceLoader)
		{
			if (colorData?.ColorizeValues == null) return;
			foreach (var colorizeValues in colorData.ColorizeValues)
			{
				var key = resourceLoader.Load<ReColorId>(colorizeValues.Id);
				if (key == null)
				{
					Debug.LogWarning($"Skipping unknown color data {key}");
					continue;
				}
				ColorizeValues[key] = new(colorizeValues.Values);
			}
		}

		public ObservableDict<ReColorId, Observable<IColorizeValues>> ColorizeValues { get; } = new();
	}
	public sealed class ObservableCustomizationToggleData
	{
		public ObservableCustomizationToggleData(SerializableCustomizationToggleData toggleData, ICompositeResourceLoader resourceLoader)
		{
			if (toggleData?.ToggleIds == null) return;
			foreach (var toggleIdString in toggleData.ToggleIds)
			{
				var toggleId = resourceLoader.Load<CharacterToggleId>(toggleIdString);
				if (toggleId == null)
				{
					Debug.LogWarning($"Skipping unknown toggle {toggleId}");
					continue;
				}
				Toggles.Add(toggleId);
			}
		}

		public ObservableHashSet<CharacterToggleId> Toggles { get; } = new();
	}

	public sealed class ObservableCustomizationNumberData
	{
		public ObservableCustomizationNumberData(SerializableCustomizationNumberData numberData, ICompositeResourceLoader resourceLoader)
		{
			if (numberData?.IntValues == null) return;
			foreach (var sliderValue in numberData.IntValues)
			{
				var key = resourceLoader.Load<CharacterIntId>(sliderValue.Id);
				if (key == null)
				{
					Debug.LogWarning($"Skipping unknown number {key}");
					continue;
				}
				IntValues[key] = new(sliderValue.Value);
			}
		}

		public ObservableDict<CharacterIntId, Observable<int>> IntValues { get; } = new();
	}

	public sealed class ObservableCustomizationPortraitData
	{
		public ObservableCustomizationPortraitData(SerializableCustomizationPortraitData portraitData, ICompositeResourceLoader resourceLoader)
		{
			if (portraitData == null || string.IsNullOrWhiteSpace(portraitData.PortraitId))
			{
				return;
			}
			var portraitId = resourceLoader.Load<PortraitId>(portraitData.PortraitId);
			if (portraitId == null)
			{
				Debug.LogWarning($"Skipping unknown portrait {portraitData.PortraitId}");
				return;
			}
			PortraitId.Val = portraitId;
			UseOverrideExpressions.Val = portraitData.UseOverrideExpressions;
			OverrideEyeExpression.Val = portraitData.OverrideEyeExpression;
			OverrideMouthExpression.Val = portraitData.OverrideMouthExpression;
		}

		public Observable<PortraitId> PortraitId { get; } = new();
		public Observable<bool> UseOverrideExpressions { get; } = new();
		public Observable<int> OverrideEyeExpression { get; } = new();
		public Observable<int> OverrideMouthExpression { get; } = new();
	}

	public sealed class ObservableCustomizationGenderData
	{
		public ObservableCustomizationGenderData(SerializableCustomizationGenderData pronounData, ICompositeResourceLoader resourceLoader)
		{
			if (pronounData == null)
			{
				Pronouns.Val = CharacterPronouns.TheyThem;
				return;
			}
			Pronouns.Val = pronounData.Pronouns;
		}

		public Observable<CharacterPronouns> Pronouns { get; } = new(CharacterPronouns.TheyThem);
	}
}
