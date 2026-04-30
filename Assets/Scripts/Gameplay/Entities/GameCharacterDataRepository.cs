using Reactivity;
using UnityEngine;

namespace Character.Creator
{
	public interface IGameCharacterDataRepository : ICustomizationDataRepository, IForceableCustomizationDataRepository
	{
		/// <summary>
		/// The last serialized data used to drive this
		/// This is not guaranteed to be up-to-date with Customization Data
		/// This is useful for things like portraits where we only occasionally want updated data
		/// </summary>
		public IReadOnlyObservable<SerializableCustomizationData> LastSerializedData { get; }
	}
	public class GameCharacterDataRepository : ReactiveBehaviour, IGameCharacterDataRepository
	{
		private ICompositeResourceLoader _resourceLoader;
		private Observable<SerializableCustomizationData> _lastSerializedData = new();
		private Observable<ObservableCustomizationData> _data = new();

		public IReadOnlyObservable<SerializableCustomizationData> LastSerializedData => _lastSerializedData;
		public ObservableCustomizationData CustomizationData => _data.Val;

		void Awake()
		{
			_resourceLoader = Singletons.GetSingleton<ICompositeResourceLoader>();

			if (_data.Val == null)
			{
				var initialSelection = this.GetComponentSafe<ICustomizationSelection>();
				var data = initialSelection.Selected.Val?.CachedData;
				if (data == null)
				{
					Debug.LogError("No initial selection found for GameCharacterDataRepository");
				}
				_lastSerializedData.Val = data;
				_data.Val = new ObservableCustomizationData(data, _resourceLoader);
			}
		}

		public void ForceCustomizationData(SerializableCustomizationData cachedData)
		{
			// Not ideal but unless I get a proper DI setup w/e
			// EDIT: We have the lazy safe IInitializable now but I haven't wired it up here yet
			if (_resourceLoader == null)
			{
				_resourceLoader = Singletons.GetSingleton<ICompositeResourceLoader>();
			}

			_lastSerializedData.Val = cachedData;
			_data.Val = new ObservableCustomizationData(cachedData, _resourceLoader);
		}
	}
}