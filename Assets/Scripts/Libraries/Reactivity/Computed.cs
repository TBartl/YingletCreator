using System;

namespace Reactivity
{
	// This could probably be optimized so it only calculates if something's actually relying on it
	public class Computed<T> : IDestroyable, IReadOnlyObservable<T>
	{
		Observable<T> cachedVal;
		Reflector reflector;

		public Computed(Func<T> func)
		{
			cachedVal = new Observable<T>();
			reflector = new Reflector(() =>
			{
				cachedVal.Val = func();
			});
			cachedVal.OnChanged += CachedVal_OnChanged;
		}

		public void Destroy()
		{
			reflector.Destroy();
			cachedVal.OnChanged -= CachedVal_OnChanged;
		}

		public T Val => cachedVal.Val;

		private void CachedVal_OnChanged(T t1, T t2)
		{
			OnChanged(t1, t2);
		}

		public OnObservableChanged<T> OnChanged { get; set; } = delegate { };
	}
}