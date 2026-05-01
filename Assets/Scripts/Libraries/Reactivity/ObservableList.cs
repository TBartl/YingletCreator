using Reactivity.Implementation;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Reactivity
{

	public class ObservableList<T> : IObservableEnumerable<T>, IList<T>
	{
		readonly IList<T> list;

		// Not using individual notifiers for vals since that will get a bit muddy as things are inserted / removed
		readonly Notifier notifier = new Notifier();

		public ObservableList()
		{
			list = new List<T>();
		}


		#region IList
		public T this[int index]
		{
			get
			{
				notifier.Track();
				return list[index];
			}
			set => throw new NotImplementedException();
		}

		public int Count
		{
			get
			{
				notifier.Track();
				return list.Count;
			}
		}

		public bool IsReadOnly => throw new NotImplementedException();

		public void Add(T item)
		{
			list.Add(item);
			notifier.Dirty();
		}

		public void Clear()
		{
			list.Clear();
			notifier.Dirty();
		}

		public bool Contains(T item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			notifier.Track();
			list.CopyTo(array, arrayIndex);
		}

		public int IndexOf(T item)
		{
			notifier.Track();
			return list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			list.Insert(index, item);
			notifier.Dirty();
		}

		public bool Remove(T item)
		{
			bool success = list.Remove(item);
			if (success)
			{
				notifier.Dirty();
			}
			return success;
		}

		public void RemoveAt(int index)
		{
			list.RemoveAt(index);
			notifier.Dirty();
		}
		#endregion

		#region IEnumerable
		public IEnumerator<T> GetEnumerator()
		{
			notifier.Track();

			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}

}