using Reactivity;

namespace Character.Creator.UI
{
	public interface IPortraitReference
	{
		public CachedYingletReference Reference { get; }
	}

	public interface IWriteablePortraitReference : IPortraitReference
	{
		void Setup(CachedYingletReference reference);
	}

	public class YingPortraitReference : ReactiveBehaviour, IWriteablePortraitReference, ICachedYingletReference, ISelectable
	{
		private ICustomizationSelection _selection;
		private Computed<bool> _selected;
		private CachedYingletReference _reference;

		public CachedYingletReference Reference => _reference;
		public SerializableCustomizationData CachedData => _reference.CachedData;
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