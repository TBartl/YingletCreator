using Reactivity;

namespace Character.Creator.UI
{
	public class CharacterCreatorToggleIdSelection : ReactiveBehaviour, ISelectable
	{
		Computed<bool> _selected;
		private ICustomizationSelectedDataRepository _dataRepo;
		private ICharacterCreatorToggleIdReference _reference;

		public IReadOnlyObservable<bool> Selected => _selected;

		void Awake()
		{
			_dataRepo = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
			_reference = this.GetComponent<ICharacterCreatorToggleIdReference>();
		}

		void Start()
		{
			_selected = CreateComputed(ComputeSelected);
		}

		private bool ComputeSelected()
		{
			var toggleVal = _dataRepo.GetToggle(_reference.ToggleId);
			return toggleVal;
		}
	}
}