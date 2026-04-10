using Reactivity.Implementation;
using System.Collections.Generic;

namespace Reactivity
{
	public delegate void OnObservableChanged<T>(T from, T to);
	public interface IReadOnlyObservable<T>
	{
		T Val { get; }
		public OnObservableChanged<T> OnChanged { get; set; }
	}

	public class Observable<T> : IReadOnlyObservable<T>
	{
		T val;

		readonly Notifier notifier = new Notifier();

		public Observable() { }
		public Observable(T val)
		{
			this.val = val;
		}

		public T Val
		{
			get
			{
				notifier.Track();
				return val;
			}
			set
			{
				// If the value is staying the same, don't do anything
				bool areSame = EqualityComparer<T>.Default.Equals(val, value);
				if (areSame) return;

				// If an object is Destroyed from another location (as is common in unity)
				// we need to set our value to null
				// We do this by adding our ClearVal() function to the Destroyed callback
				var destroyableElement = value as IDestroyableObservableElement;
				if (destroyableElement != null)
				{
					// Might need to make this disposable and remove all the events at some point
					destroyableElement.Destroyed += ClearVal;
				}
				var previousDestroyableElement = val as IDestroyableObservableElement;
				if (previousDestroyableElement != null)
				{
					previousDestroyableElement.Destroyed -= ClearVal;
				}

				var from = val;

				val = value;

				OnChanged(from, val);

				notifier.Dirty();

			}
		}

		public OnObservableChanged<T> OnChanged { get; set; } = delegate { };

		void ClearVal()
		{
			Val = default(T);
		}
	}

}