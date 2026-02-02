using Reactivity;

namespace Character.Creator.UI
{
	public class YingletSelectionPortraitReference : ReactiveBehaviour, IYingPortraitReference
	{
		private IMainMenuYingletSelection _yingletSelection;
		private Computed<bool> _selected;
		private CachedYingletReference _reference;

		public CachedYingletReference Reference => _reference;
		public IReadOnlyObservable<bool> Selected => _selected;

		void Start()
		{
			_yingletSelection = Singletons.GetSingleton<IMainMenuYingletSelection>();
			_selected = CreateComputed(ComputeSelected);
		}

		public void Setup(CachedYingletReference reference)
		{
			_reference = reference;
		}

		bool ComputeSelected()
		{
			return _yingletSelection.Selected == _reference;
		}
	}
}