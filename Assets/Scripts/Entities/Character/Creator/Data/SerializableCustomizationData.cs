using Character.Compositor;
using Character.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Character.Creator
{
	/// <summary>
	/// Representation of save data that is entirely serializable
	/// </summary>
	[System.Serializable]
	public sealed class SerializableCustomizationData
	{
		// Used in <see cref="CustomizationDataUpgradeUtils"/>
		const int CURRENT_VERSION = 4;

		public string Name;

		public string CreationTimeString;

		public int Version;

		public DateTime CreationTime
		{
			get
			{
				if (string.IsNullOrWhiteSpace(CreationTimeString))
				{
					return DateTime.MinValue;
				}
				return DateTime.Parse(CreationTimeString);
			}
			set => CreationTimeString = value.ToString("o");
		}


		public SerializableCustomizationSliderData SliderData;
		public SerializableCustomizationColorData ColorData;
		public SerializableCustomizationToggleData ToggleData;
		public SerializableCustomizationNumberData NumberData;
		public SerializableCustomizationPortraitData PortraitData;
		// Note: If adding anything else, make sure you implement it in SerializableCustomizationDataComparison

		public static SerializableCustomizationData FromJSON(string dataBlob)
		{
			try
			{
				return JsonUtility.FromJson<SerializableCustomizationData>(dataBlob);
			}
			catch (ArgumentException)
			{
				return null;
			}
		}

		public SerializableCustomizationData(ObservableCustomizationData data)
		{
			Version = CURRENT_VERSION; // If we're saving off of observable data, put us on the latest version
			Name = data.Name.Val;
			CreationTime = data.CreationTime;
			SliderData = new SerializableCustomizationSliderData(data.SliderData);
			ColorData = new SerializableCustomizationColorData(data.ColorData, data.ToggleData);
			ToggleData = new SerializableCustomizationToggleData(data.ToggleData);
			NumberData = new SerializableCustomizationNumberData(data.NumberData);
			PortraitData = new SerializableCustomizationPortraitData(data.PortraitData);
		}


	}


	[System.Serializable]
	public sealed class SerializableCustomizationSliderData
	{
		public SerializableCustomizationSliderData(ObservableCustomizationSliderData data)
		{
			SliderValues = data.SliderValues.Select(kvp => new SliderFloatPair(kvp.Key.UniqueAssetID, kvp.Value.Val)).ToArray();
		}

		/// <summary>
		/// The float value is always in the range of 0 to 1
		/// </summary>
		public SliderFloatPair[] SliderValues;

		[System.Serializable]
		public sealed class SliderFloatPair
		{
			public SliderFloatPair(string id, float value)
			{
				Id = id;
				Value = value;

			}
			public string Id;
			public float Value;
		}
	}

	[System.Serializable]
	public sealed class SerializableCustomizationColorData
	{
		public SerializableCustomizationColorData(ObservableCustomizationColorData data, ObservableCustomizationToggleData toggleData)
		{
			var actuallyUsedColors = new HashSet<ReColorId>();
			foreach (var toggle in toggleData.Toggles)
			{
				foreach (var texture in toggle.AddedTextures)
				{
					actuallyUsedColors.Add(texture.ReColorId);
				}
			}

			ColorizeValues = data.ColorizeValues
				.Where(c => !c.Key.CleanupIfUnused || actuallyUsedColors.Contains(c.Key))
				.Select(kvp => new ColorizeValuesPair(kvp.Key.UniqueAssetID, new(kvp.Value.Val)))
				.ToArray();
		}

		/// <summary>
		/// The float value is always in the range of 0 to 1
		/// </summary>
		public ColorizeValuesPair[] ColorizeValues;

		[System.Serializable]
		public sealed class ColorizeValuesPair
		{
			public ColorizeValuesPair(string id, SerializableColorizeValues values)
			{
				Id = id;
				Values = values;

			}
			public string Id;
			public SerializableColorizeValues Values;
		}
	}

	[System.Serializable]
	public sealed class SerializableCustomizationToggleData
	{
		public SerializableCustomizationToggleData(ObservableCustomizationToggleData data)
		{
			ToggleIds = data.Toggles.Select(t => t.UniqueAssetID).ToArray();
		}

		public string[] ToggleIds;
	}



	[System.Serializable]
	public sealed class SerializableCustomizationNumberData
	{
		public SerializableCustomizationNumberData(ObservableCustomizationNumberData data)
		{
			IntValues = data.IntValues.Select(kvp => new NumberIdIntPair(kvp.Key.UniqueAssetID, kvp.Value.Val)).ToArray();
		}

		public NumberIdIntPair[] IntValues;

		[System.Serializable]
		public sealed class NumberIdIntPair
		{
			public NumberIdIntPair(string id, int value)
			{
				Id = id;
				Value = value;

			}
			public string Id;
			public int Value;
		}
	}

	[System.Serializable]
	public sealed class SerializableCustomizationPortraitData
	{
		public SerializableCustomizationPortraitData(ObservableCustomizationPortraitData data)
		{
			PortraitId = data.PortraitId.Val?.UniqueAssetID;
			UseOverrideExpressions = data.UseOverrideExpressions.Val;
			OverrideEyeExpression = data.OverrideEyeExpression.Val;
			OverrideMouthExpression = data.OverrideMouthExpression.Val;
		}

		public string PortraitId;
		public bool UseOverrideExpressions;
		public int OverrideEyeExpression;
		public int OverrideMouthExpression;
	}
}