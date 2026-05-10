using Reactivity;
using UnityEngine;

public static class ExpeditionExtensionMethods
{

	public static Computed<T> CreateExpeditionComputed<T>(this ReactiveBehaviour mb)
	{
		var expeditionManager = Singletons.GetSingleton<IExpeditionManager>();
		return CreateExpeditionComputed<T>(mb, expeditionManager);
	}

	public static Computed<T> CreateExpeditionComputed<T>(this ReactiveBehaviour mb, IExpeditionManager expeditionManager)
	{
		return mb.CreateComputed<T>(() =>
		{
			var rootObject = expeditionManager.RootObject;

			if (rootObject == null) return default(T);

			return rootObject.GetComponentInChildrenSafe<T>(true);
		});
	}

	/// <summary>
	/// To be called only by objects under the expedition root
	/// </summary>
	public static T GetExpeditionComponent<T>(this MonoBehaviour mb)
	{
		/// <summary>
		/// Returns the first component under the parent composited yinglet root
		/// </summary>
		var type = typeof(T);
		var root = mb.GetComponentInParentSafe<ExpeditionRoot>(true);
		if (root == null)
		{
			Debug.LogWarning($"Failed to get expedition component of type {type}; could not find expedition root");
			return default(T);
		}

		var component = root.GetComponentInChildrenSafe<T>(true);
		if (component == null)
		{
			Debug.LogWarning($"Failed to get expedition component of type {type}; could not find a component");
			return default(T);
		}
		return component;
	}
}
