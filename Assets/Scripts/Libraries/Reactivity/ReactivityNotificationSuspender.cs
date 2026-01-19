using Reactivity.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reactivity
{
	/// <summary>
	/// Disposable object that prevents notifications from being fired while this object is alive
	/// Used to bundle numerous changes so that updates can be consolidated
	/// </summary>
	public sealed class ReactivityNotificationSuspender : IDisposable
	{
		static ReactivityNotificationSuspender _current = null;

		List<IDependent> _suspendedDependents = new();

		public ReactivityNotificationSuspender()
		{
			if (_current != null)
			{
				throw new InvalidOperationException("Tried to suspend reactivity when it was already suspended!");
			}
			_current = this;
		}

		public static bool ShouldSuspend(IDependent dependant)
		{
			if (_current == null) return false;
			_current._suspendedDependents.Add(dependant);
			return true;
		}

		public void Dispose()
		{
			_current = null;
			foreach (var dependant in _suspendedDependents.Distinct())
			{
				dependant?.SetDirty();
			}
		}
	}
}
