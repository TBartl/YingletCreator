using Reactivity.Implementation;
using System.Collections;
using System.Collections.Generic;

namespace Reactivity
{
	/// <summary>
	/// Observable read-only hashset, where the contents can only be updated all at once
	/// We're not on .NET 5 yet, so we don't have IReadOnlySet
	/// </summary>
	public class ObservableUpdateableSet<T> : IObservableEnumerable<T>, IReadOnlySet<T>
	{
		HashSet<T> _set = new HashSet<T>();
		readonly Notifier notifier = new Notifier();

		public IEnumerator<T> GetEnumerator()
		{
			notifier.Track();
			return _set.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Contains(T item)
		{
			notifier.Track();
			return _set.Contains(item);
		}

		public void Update(HashSet<T> _newSet)
		{
			if (_newSet.SetEquals(_set)) return;

			_set = _newSet;
			notifier.Dirty();
		}
	}
}
