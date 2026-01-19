using Reactivity.Implementation;
using System;

namespace Reactivity
{
	/// <summary>
	/// Disposable object that prevents any reactivity from occurring while the object is alive
	/// This may be useful if you're writing to your own data within a reactive context
	/// i.e.: the auto-upgrade logic reads from itself
	/// </summary>
	public sealed class ReactivityTrackingSuspender : IDisposable
	{
		private readonly IDependent _dependentToRestore;

		public ReactivityTrackingSuspender()
		{
			_dependentToRestore = Statics.CurrentDependent;
			Statics.CurrentDependent = null;
		}

		public void Dispose()
		{
			Statics.CurrentDependent = _dependentToRestore;
		}
	}
}
