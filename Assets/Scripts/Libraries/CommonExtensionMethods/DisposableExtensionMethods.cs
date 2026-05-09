using System;

public static class DisposableExtensions
{
	public static IDisposable Toggle(
		this IDisposable current,
		bool enabled,
		Func<IDisposable> acquire)
	{
		if (enabled)
		{
			current ??= acquire();
		}
		else
		{
			current?.Dispose();
			current = null;
		}

		return current;
	}
}