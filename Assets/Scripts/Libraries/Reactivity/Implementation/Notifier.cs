using System.Collections.Generic;

namespace Reactivity.Implementation
{
	public class Notifier
	{
		readonly IList<IDependent> dependents;

		public Notifier()
		{
			dependents = new List<IDependent>();
		}

		public void Track()
		{
			if (Statics.CurrentDependent != null && !dependents.Contains(Statics.CurrentDependent))
			{
				dependents.Add(Statics.CurrentDependent);
				Statics.CurrentDependent.AddNotifier(this);
			}
		}

		public void Dirty()
		{

			// As we set dirty, the original list will likely be updated, so clone it first
			var clonedDependents = new List<IDependent>(dependents);
			dependents.Clear();
			foreach (var dependent in clonedDependents)
			{
				if (ReactivityNotificationSuspender.ShouldSuspend(dependent)) continue;

				dependent.SetDirty();
			}
		}

		public void ClearDependent(IDependent dependent)
		{
			dependents.Remove(dependent);
		}
	}
}