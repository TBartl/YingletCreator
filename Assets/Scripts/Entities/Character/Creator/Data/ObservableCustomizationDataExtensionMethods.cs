using Character.Data;
using Reactivity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Character.Creator
{
	public static class ObservableCustomizationDataExtensionMethods
	{
		public static bool FlipToggle(this ICollection<CharacterToggleId> toggles, CharacterToggleId id, bool allowDebugOverride = false)
		{
			bool exists = toggles.Contains(id);

			if (allowDebugOverride && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl))
			{
				// Easter egg: Ignore rules if holding both down
				if (exists) toggles.Remove(id);
				else toggles.Add(id);
				return !exists;
			}

			if (exists)
			{
				// Early return if there must be one toggle of this type
				foreach (var group in id.Groups)
				{
					if (group.MustHaveOne)
					{
						bool anotherExists = toggles.Any(other => other != id && other.Groups.Contains(group));
						if (!anotherExists) return false;
					}
				}
				toggles.Remove(id);
				return false;
			}
			else
			{
				toggles.Add(id);

				foreach (var group in id.Groups)
				{
					var togglesToRemove = toggles
						.Where(toggle => toggle != id && toggle.Groups.Contains(group))
						.ToList();
					foreach (var toggleToRemove in togglesToRemove)
					{
						toggles.Remove(toggleToRemove);
					}
				}
				return true;
			}
		}


		public static bool GetToggle(this ObservableCustomizationData data, CharacterToggleId id)
		{
			return data.ToggleData.Toggles.Contains(id);
		}

		public static void FlipToggle(this ObservableCustomizationData data, CharacterToggleId id)
		{

			using var suspender = new ReactivityNotificationSuspender();
			data.ToggleData.Toggles.FlipToggle(id, allowDebugOverride: true);

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