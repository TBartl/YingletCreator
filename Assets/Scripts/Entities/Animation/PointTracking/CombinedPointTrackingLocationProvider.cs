using Reactivity;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Entities.Animation.PointTracking
{
	internal class CombinedPointTrackingLocationProvider : ReactiveBehaviour, IPointTrackingLocationProvider, IInitializable
	{
		public IReadOnlyObservable<bool> Active => _active;

		public Vector3 Position => _activeProvider.Val?.Position ?? Vector3.zero;
		public bool MoveUpperBody => _activeProvider.Val?.MoveUpperBody ?? true;

		private IPointTrackingLocationProvider[] _allProviders;
		Computed<IPointTrackingLocationProvider> _activeProvider;
		private Computed<bool> _active;

		public void Initialize()
		{
			_allProviders = this.GetComponentsSafe<IPointTrackingLocationProvider>().Where(provider => provider != (IPointTrackingLocationProvider)this).ToArray();
			_activeProvider = CreateComputed(ComputeActiveProvider);
			_active = CreateComputed(ComputeActive);
		}

		private IPointTrackingLocationProvider ComputeActiveProvider()
		{
			foreach (var provider in _allProviders)
			{
				if (provider.Active.Val)
				{
					return provider;
				}
			}
			return null;
		}

		private bool ComputeActive()
		{
			return _activeProvider.Val != null;
		}
	}
}
