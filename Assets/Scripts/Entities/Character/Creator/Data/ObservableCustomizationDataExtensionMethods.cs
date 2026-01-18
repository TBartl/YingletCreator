using Character.Data;
using Reactivity;
using System.Linq;
using UnityEngine;

namespace Character.Creator
{
	public static class ObservableCustomizationDataExtensionMethods
	{

		public static bool GetToggle(this ObservableCustomizationData data, CharacterToggleId id)
		{
			return data.ToggleData.Toggles.Contains(id);
		}

		public static void FlipToggle(this ObservableCustomizationData data, CharacterToggleId id)
		{

			using var suspender = new ReactivityNotificationSuspender();
			bool exists = data.GetToggle(id);

			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl))
			{
				// Easter egg: Ignore rules if holding both down
				if (exists) data.ToggleData.Toggles.Remove(id);
				else data.ToggleData.Toggles.Add(id);
				return;
			}

			if (exists)
			{
				// Early return if there must be one toggle of this type
				foreach (var group in id.Groups)
				{
					if (group.MustHaveOne)
					{
						bool anotherExists = data.ToggleData.Toggles.Any(other => other != id && other.Groups.Contains(group));
						if (!anotherExists) return;
					}
				}
				data.ToggleData.Toggles.Remove(id);
			}
			else
			{
				data.ToggleData.Toggles.Add(id);

				foreach (var group in id.Groups)
				{
					var togglesToRemove = data.ToggleData.Toggles
						.Where(toggle => toggle != id && toggle.Groups.Contains(group))
						.ToList();
					foreach (var toggleToRemove in togglesToRemove)
					{
						data.ToggleData.Toggles.Remove(toggleToRemove);
					}
				}
			}

			// For some colors, we want to default to another color in the same group if it has been specified
			foreach (var mixTexture in id.AddedTextures)
			{
				var recolorId = mixTexture.ReColorId;

				// Only if we have the special property set
				if (!recolorId.ColorGroup) return;
				if (!recolorId.ColorGroup.AutoColorWithGroup) return;

				// If this already has an explicit color, don't do anything
				if (data.ColorData.ColorizeValues.Any(kvp => kvp.Key == recolorId)) continue;

				var kvpToCopyFrom = data.ColorData.ColorizeValues.Where(kvp => kvp.Key.ColorGroup == recolorId.ColorGroup).FirstOrDefault();

				// If there's no other color to copy from, don't do anything
				if (kvpToCopyFrom.Value == null) continue;

				data.ColorData.ColorizeValues[recolorId] = new(kvpToCopyFrom.Value.Val);
			}
		}

		public static int GetInt(this ObservableCustomizationNumberData data, CharacterIntId id)
		{
			if (data.IntValues.TryGetValue(id, out Observable<int> value))
			{
				return value.Val;
			}
			return 0;
		}
	}
}
