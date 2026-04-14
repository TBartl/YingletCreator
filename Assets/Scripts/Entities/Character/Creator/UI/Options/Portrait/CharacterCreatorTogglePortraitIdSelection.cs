using Reactivity;

namespace Character.Creator.UI
{
	public class CharacterCreatorTogglePortraitIdSelection : ReactiveBehaviour, ISelectable
	{
		Computed<bool> _selected;
		private ICustomizationSelectedDataRepository _dataRepo;
		private ICharacterCreatorTogglePortraitIdReference _reference;

		public IReadOnlyObservable<bool> Selected => _selected;

		void Awake()
		{
			_dataRepo = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
			_reference = this.GetComponent<ICharacterCreatorTogglePortraitIdReference>();
		}

		void Start()
		{
			_selected = CreateComputed(ComputeSelected);
		}

		private bool ComputeSelected()
		{
			if (_dataRepo.CustomizationData == null) return false;

			var toggleVal = _dataRepo.CustomizationData.PortraitData.PortraitId.Val == _reference.PortraitId;
			return toggleVal;
		}
	}
}