using Reactivity;
using System.Linq;

namespace Character.Creator.UI
{
	public class PoseYingPortraitReference : ReactiveBehaviour, IPortraitReference
	{
		private IPoseData _poseData;
		private Computed<bool> _selected;
		private CachedYingletReference _reference;

		public CachedYingletReference Reference => _reference;
		public IReadOnlyObservable<bool> Selected => _selected;

		void Start()
		{
			_poseData = this.GetComponentInParent<IPoseData>();
			_selected = CreateComputed(ComputeSelected);
		}

		public void Setup(CachedYingletReference reference)
		{
			_reference = reference;
		}

		bool ComputeSelected()
		{
			return _poseData.Data.Keys.Contains(_reference);
		}
	}
}