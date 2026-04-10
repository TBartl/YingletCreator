using Reactivity;

namespace Character.Creator.UI
{
	public interface IYingPortraitReference : ISelectable
	{
		void Setup(CachedYingletReference reference);
		public CachedYingletReference Reference { get; }
	}

	public class YingPortraitReference : ReactiveBehaviour, IYingPortraitReference
	{
		private ICustomizationSelection _selection;
		private Computed<bool> _selected;
		private CachedYingletReference _reference;

		public CachedYingletReference Reference => _reference;
		public IReadOnlyObservable<bool> Selected => _selected;

		void Start()
		{
			_selection = Singletons.GetSingleton<ICustomizationSelection>();
			_selected = CreateComputed(ComputeSelected);
		}

		public void Setup(CachedYingletReference reference)
		{
			_reference = reference;
		}

		bool ComputeSelected()
		{
			return _reference == _selection.Selected.Val;
		}
	}
}