using Reactivity;

public static class ExpeditionExtensionMethods
{
	public static Computed<T> CreateExpeditionComputed<T>(this ReactiveBehaviour mb, IExpeditionManager expeditionManager)
	{
		return mb.CreateComputed<T>(() =>
		{
			var rootObject = expeditionManager.RootObject;

			if (rootObject == null) return default(T);

			return rootObject.GetComponentInChildren<T>(true);
		});
	}
}
