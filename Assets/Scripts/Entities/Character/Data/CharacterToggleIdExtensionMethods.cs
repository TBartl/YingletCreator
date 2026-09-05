using Character.Data;
using System.Collections.Generic;
using System.Linq;

public static class CharacterToggleIdExtensionMethods
{
	public static T GetLastComponentOrDefault<T>(this IEnumerable<CharacterToggleId> toggles)
	{
		foreach (var toggle in toggles.Reverse())
		{
			foreach (var component in toggle.Components)
			{
				if (component is T typedComponent)
				{
					return typedComponent;
				}
			}
		}
		return default;
	}

	public static T GetComponent<T>(this CharacterToggleId toggle) where T : class
	{
		foreach (var component in toggle.Components)
		{
			if (component is T typedComponent)
			{
				return typedComponent;
			}
		}
		return null;
	}
}
