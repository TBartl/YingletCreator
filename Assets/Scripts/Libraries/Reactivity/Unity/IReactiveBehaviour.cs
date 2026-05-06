using System;
using UnityEngine;

namespace Reactivity
{
	public interface IReactiveBehaviour : IDestroyableObservableElement
	{
		void AddReflector(Action action);
		Computed<T> CreateComputed<T>(Func<T> func);
		// void Watch<T>(Observable<T> observable, Action<T,T> action);

		void OnDestroy();

		GameObject gameObject { get; }

		/// <summary>
		/// True during AddReflector and CreateComputed calls
		/// Useful for not firing off animations or sound effects during the setup of these
		/// </summary>
		bool IsInitializing { get; }
	}
}