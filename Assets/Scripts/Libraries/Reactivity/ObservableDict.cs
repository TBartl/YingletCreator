using Reactivity.Implementation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Reactivity
{

	public class ObservableDict<K, V> : IObservableEnumerable<KeyValuePair<K, V>>, IDictionary<K, V>, IReadOnlyDictionary<K, V>
	{
		class DictValue
		{
			// If we want TryGetValue to react when a value was added, we need a notifier to exist even when a value isn't set for a key
			// Alternatively, TryGetValue could just rely on the enumerableNotifier.
			// But this trades memory for some speed
			public bool Exists { get; set; }
			public V Value { get; set; }

			public Notifier Notifier { get; } = new Notifier();
		}

		readonly Dictionary<K, DictValue> dict = new Dictionary<K, DictValue>();
		readonly Notifier enumerableNotifier = new Notifier();

		#region Dictionary

		public void Add(K key, V value)
		{
			if (!dict.TryGetValue(key, out var dictValue))
			{
				dictValue = new DictValue();
				dict.Add(key, dictValue);
			}
			if (dictValue.Exists) throw new ArgumentException();
			dictValue.Exists = true;
			dictValue.Value = value;
			dictValue.Notifier.Dirty();

			enumerableNotifier.Dirty();
		}

		public bool ContainsKey(K key)
		{
			if (!dict.TryGetValue(key, out DictValue dictValue))
			{
				dictValue = new DictValue();
				dict.Add(key, dictValue);
			}
			dictValue.Notifier.Track();

			return dictValue.Exists;
		}

		public bool Remove(K key)
		{
			bool existed = false;
			if (dict.TryGetValue(key, out var dictValue))
			{
				existed = dictValue.Exists;
				dictValue.Exists = false;
				dictValue.Notifier.Dirty();
			}
			enumerableNotifier.Dirty();
			return existed;
		}

		public bool TryGetValue(K key, out V value)
		{
			if (!dict.TryGetValue(key, out DictValue dictValue))
			{
				dictValue = new DictValue();
				dict.Add(key, dictValue);
			}
			dictValue.Notifier.Track();

			value = dictValue.Value;
			return dictValue.Exists;
		}

		public void Add(KeyValuePair<K, V> item)
		{
			throw new System.NotImplementedException();
		}

		public void Clear()
		{
			dict.Clear();
			enumerableNotifier.Dirty();
		}

		public bool Contains(KeyValuePair<K, V> item)
		{
			throw new System.NotImplementedException();
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			enumerableNotifier.Track();
			foreach (var kvp in dict.Where(kvp => kvp.Value.Exists).Select(kvp => new KeyValuePair<K, V>(kvp.Key, kvp.Value.Value)))
			{
				if (arrayIndex >= array.Length) throw new ArgumentException("Array is too small");
				array[arrayIndex++] = kvp;
			}
		}

		public bool Remove(KeyValuePair<K, V> item)
		{
			throw new System.NotImplementedException();
		}

		public ICollection<K> Keys =>
		throw new System.NotImplementedException();

		public ICollection<V> Values =>
		throw new System.NotImplementedException();

		public int Count
		{
			get
			{
				enumerableNotifier.Track();
				return dict.Count(kvp => kvp.Value.Exists);
			}
		}

		public bool IsReadOnly =>
		throw new System.NotImplementedException();

		IEnumerable<K> IReadOnlyDictionary<K, V>.Keys
		{
			get
			{
				enumerableNotifier.Track();
				return dict
					.Where(kvp => kvp.Value.Exists)
					.Select(kvp => kvp.Key)
					.ToArray();
			}
		}

		IEnumerable<V> IReadOnlyDictionary<K, V>.Values =>
		throw new System.NotImplementedException();

		public V this[K key]
		{
			get
			{
				if (!dict.TryGetValue(key, out var dictValue) || !dictValue.Exists)
				{
					throw new KeyNotFoundException();
				}
				dictValue.Notifier.Track();
				return dictValue.Value;
			}
			set
			{
				if (!dict.TryGetValue(key, out var dictValue))
				{
					dictValue = new DictValue();
					dict.Add(key, dictValue);
				}
				dictValue.Exists = true;
				if (EqualityComparer<V>.Default.Equals(dictValue.Value, value)) return;
				dictValue.Value = value;
				dictValue.Notifier.Dirty();

				enumerableNotifier.Dirty();
			}
		}

		#endregion

		#region IEnumerable
		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			enumerableNotifier.Track();

			return dict
				.Where(kvp => kvp.Value.Exists)
				.Select(kvp => new KeyValuePair<K, V>(kvp.Key, kvp.Value.Value))
				.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}