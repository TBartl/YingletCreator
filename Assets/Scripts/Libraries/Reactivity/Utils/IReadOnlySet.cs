using System.Collections.Generic;

/// <summary>
/// We're not on .NET 5 yet, so implement this ourself
/// </summary>
public interface IReadOnlySet<T> : IEnumerable<T>
{
	bool Contains(T item);
}