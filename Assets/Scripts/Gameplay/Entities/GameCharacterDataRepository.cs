using Reactivity;
using System.Linq;

namespace Character.Creator
{

	public class GameCharacterDataRepository : ReactiveBehaviour, ICustomizationSelectedDataRepository
	{
		private ICompositeResourceLoader _resourceLoader;
		private ILocalYingletRepository _yingletRepository;
		private Observable<ObservableCustomizationData> _data = new();

		public ObservableCustomizationData CustomizationData => _data.Val;

		void Awake()
		{
			_resourceLoader = Singletons.GetSingleton<ICompositeResourceLoader>();
			_yingletRepository = Singletons.GetSingleton<ILocalYingletRepository>();
			var firstCharacterData = _yingletRepository.GetYinglets(LocalYingletGroup.Preset).First().CachedData;
			_data.Val = new ObservableCustomizationData(firstCharacterData, _resourceLoader);
		}
	}
}