using Reactivity;
using UnityEngine;

namespace Character.Creator
{
	public sealed class CharacterCreatorStateSnapshot
	{
		public CharacterCreatorStateSnapshot(string action, CachedYingletReference selected, SerializableCustomizationData serializedData)
		{
			Action = action;
			Selected = selected;
			SerializedData = serializedData;
		}
		public string Action { get; }
		public CachedYingletReference Selected { get; }
		public SerializableCustomizationData SerializedData { get; }
	}

	public interface ICharacterCreatorStateSnapshotter
	{
		CharacterCreatorStateSnapshot GetStateSnapshot(string action);
		void RestoreStateSnapshot(CharacterCreatorStateSnapshot snapshot);
	}


	internal class CharacterCreatorStateSnapshotter : MonoBehaviour, ICharacterCreatorStateSnapshotter
	{
		private ICustomizationSelection _selection;
		private ICustomizationSelectedDataRepository _dataRepository;

		private void Awake()
		{
			_selection = Singletons.GetSingleton<ICustomizationSelection>();
			_dataRepository = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
		}

		public CharacterCreatorStateSnapshot GetStateSnapshot(string action)
		{
			var cachedData = new SerializableCustomizationData(_dataRepository.CustomizationData);
			return new CharacterCreatorStateSnapshot(action, _selection.Selected.Val, cachedData);
		}

		public void RestoreStateSnapshot(CharacterCreatorStateSnapshot snapshot)
		{
			using var suspender = new ReactivityNotificationSuspender();
			_selection.SetSelected(snapshot.Selected);
			_dataRepository.ForceCustomizationData(snapshot.SerializedData);
		}
	}
}
