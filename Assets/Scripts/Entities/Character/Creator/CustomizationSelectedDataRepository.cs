using Reactivity;
using UnityEngine;

namespace Character.Creator
{
	/// <summary>
	/// This is made available only for undo purposes; most consumers should not need to set this
	/// </summary>
	public interface IForceableCustomizationDataRepository
	{
		void ForceCustomizationData(SerializableCustomizationData cachedData);
	}

	/// <summary>
	/// Returns observable data associated to the character currently selected for customization
	/// This is a singleton
	/// </summary>
	public interface ICustomizationSelectedDataRepository : ICustomizationDataRepository, IForceableCustomizationDataRepository { }


	public class CustomizationSelectedDataRepository : ReactiveBehaviour, ICustomizationSelectedDataRepository
	{
		private IActiveCharacterProvider _activeCharacterProvider;
		private ICustomizationSelection _selection;

		Computed<IGameCharacterDataRepository> _characterDataRepository;
		Computed<ObservableCustomizationData> _customizationData;

		public ObservableCustomizationData CustomizationData => _customizationData.Val;

		void Awake()
		{
			_activeCharacterProvider = Singletons.GetSingleton<IActiveCharacterProvider>();

			// TODO: use this
			_selection = this.GetComponentSafe<ICustomizationSelection>();
			_selection.Selected.OnChanged += Selected_OnChanged;

			_characterDataRepository = CreateComputed(ComputeCharacterDataRepository);
			_customizationData = CreateComputed(ComputeCustomizationData);
		}

		private new void OnDestroy()
		{
			base.OnDestroy();
			_selection.Selected.OnChanged -= Selected_OnChanged;
		}

		private void Selected_OnChanged(CachedYingletReference from, CachedYingletReference to)
		{
			var characterRepository = _characterDataRepository.Val;
			if (characterRepository == null)
			{
				// This can happen on startup
				return;
			}
			if (to == null)
			{
				// This seemingly happens when a client gets disconnected? Not sure why tbh
				return;
			}

			if (((Component)characterRepository).gameObject.GetComponentInParent<IClassReference>() != null)
			{
				// Super hacky code to see if this is a game character
				// I should really reconsider how all this logic works
				// Right now, switching who we're spectating is causing this to be forced
				// This should probably be forced from within the component or something
				return;
			}

			characterRepository.ForceCustomizationData(to.CachedData);
		}

		// Optimization Opportunity: Instead of using CharacterSpawner, we should consider only using whatever initiated this
		// That way we wouldn't be changing it so much when jumping between different characters eventually
		private IGameCharacterDataRepository ComputeCharacterDataRepository()
		{
			var myCharacter = _activeCharacterProvider.ActiveCharacter.Val;
			if (myCharacter == null)
			{
				return null;
			}

			return myCharacter.GetComponentInChildrenSafe<IGameCharacterDataRepository>();
		}

		private ObservableCustomizationData ComputeCustomizationData()
		{
			var dataRepo = _characterDataRepository.Val;
			if (dataRepo == null)
			{
				return null;
			}

			return dataRepo.CustomizationData;
		}

		//void ReflectCustomizationData()
		//{
		//	// This used to be a computed, but with undo we want to be able to force the customization data to a specific state
		//	var cachedData = _selection.Selected.CachedData;
		//	_data.Val = new ObservableCustomizationData(cachedData, _resourceLoader);
		//}

		public void ForceCustomizationData(SerializableCustomizationData cachedData)
		{
			if (_characterDataRepository.Val == null)
			{
				Debug.LogError("No character data repository found to force data to");
				return;
			}
			_characterDataRepository.Val.ForceCustomizationData(cachedData);
		}
	}
}