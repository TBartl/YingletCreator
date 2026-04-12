using Reactivity;
using UnityEngine;

namespace Character.Creator
{
	public interface IGameCharacterDataRepository : ICustomizationDataRepository, IForceableCustomizationDataRepository
	{
		void Increment();
	}
	public class GameCharacterDataRepository : ReactiveBehaviour, IGameCharacterDataRepository
	{
		private ICompositeResourceLoader _resourceLoader;
		private CachedYingletReference[] _yinglets;
		private Observable<ObservableCustomizationData> _data = new();

		int _index = 0;
		private IPlayerIdentity _identity;
		private IInputRestrictor _inputRestrictor;

		public ObservableCustomizationData CustomizationData => _data.Val;

		public void Increment()
		{
			OffsetIndex(1);
		}

		void OffsetIndex(int offset)
		{
			_index = (_index + offset + _yinglets.Length) % _yinglets.Length;
			_data.Val = new ObservableCustomizationData(_yinglets[_index].CachedData, _resourceLoader);
		}

		void Awake()
		{
			_resourceLoader = Singletons.GetSingleton<ICompositeResourceLoader>();
			var initialSelection = this.GetComponent<ICustomizationSelection>();
			var data = initialSelection.Selected.Val?.CachedData;
			if (data == null)
			{
				Debug.LogError("No initial selection found for GameCharacterDataRepository");
			}
			_data.Val = new ObservableCustomizationData(data, _resourceLoader);

			_identity = this.GetComponentInParent<IPlayerIdentity>();
			_inputRestrictor = Singletons.GetSingleton<IInputRestrictor>();
		}

		private void Update()
		{
			if (!_identity.IsMine) return;
			if (!_inputRestrictor.InputAllowed) return;

			//if (Input.GetKeyDown(KeyCode.Q))
			//{
			//	OffsetIndex(-1);
			//}

			//if (Input.GetKeyDown(KeyCode.E))
			//{
			//	OffsetIndex(1);
			//}
		}

		public void ForceCustomizationData(SerializableCustomizationData cachedData)
		{
			_data.Val = new ObservableCustomizationData(cachedData, _resourceLoader);
		}
	}
}