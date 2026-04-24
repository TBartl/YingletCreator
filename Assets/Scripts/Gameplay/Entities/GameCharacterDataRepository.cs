using Networking;
using Reactivity;
using UnityEngine;

namespace Character.Creator
{
	public interface IGameCharacterDataRepository : ICustomizationDataRepository, IForceableCustomizationDataRepository
	{
	}
	public class GameCharacterDataRepository : ReactiveBehaviour, IGameCharacterDataRepository
	{
		private ICompositeResourceLoader _resourceLoader;
		private Observable<ObservableCustomizationData> _data = new();

		private ICharacterIdentity _identity;
		private IInputRestrictor _inputRestrictor;

		public ObservableCustomizationData CustomizationData => _data.Val;

		void Awake()
		{
			_resourceLoader = Singletons.GetSingleton<ICompositeResourceLoader>();
			var initialSelection = this.GetComponent<ICustomizationSelection>();
			_identity = this.GetComponentInParentSafe<ICharacterIdentity>();
			_inputRestrictor = Singletons.GetSingleton<IInputRestrictor>();

			if (_data.Val == null)
			{
				var data = initialSelection.Selected.Val?.CachedData;
				if (data == null)
				{
					Debug.LogError("No initial selection found for GameCharacterDataRepository");
				}
				_data.Val = new ObservableCustomizationData(data, _resourceLoader);
			}
		}

		private void Update()
		{
			if (!_identity.IsActive) return;
			if (!_inputRestrictor.InputAllowed) return;
		}

		public void ForceCustomizationData(SerializableCustomizationData cachedData)
		{
			// Not ideal but unless I get a proper DI setup w/e
			// EDIT: We have the lazy safe IInitializable now but I haven't wired it up here yet
			if (_resourceLoader == null)
			{
				_resourceLoader = Singletons.GetSingleton<ICompositeResourceLoader>();
			}


			_data.Val = new ObservableCustomizationData(cachedData, _resourceLoader);
		}
	}
}