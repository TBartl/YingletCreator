using Reactivity;
using UnityEngine;

namespace Character.Creator.UI
{
	public class YingPortraitGrid : ReactiveBehaviour
	{
		[SerializeField] GameObject _yingPortraitPrefab;
		[SerializeField] LocalYingletGroup _group;
		private ILocalYingletRepository _yingletRepository;
		private int _initialChildren;
		EnumerableDictReflector<CachedYingletReference, GameObject> _yingEnumerableReflector;

		private void Awake()
		{
			_yingletRepository = Singletons.GetSingleton<ILocalYingletRepository>();
			_initialChildren = this.transform.childCount;
		}
		private void Start()
		{
			_yingEnumerableReflector = new(CreatePortrait, RemovePortrait);
			AddReflector(ReflectYings);
		}

		private GameObject CreatePortrait(CachedYingletReference yingReference)
		{
			var go = Instantiate(_yingPortraitPrefab, this.transform);

			// Place it right before the pre-existing "Create New" button
			if (_initialChildren != 0)
			{
				int siblingCount = this.transform.childCount;
				int targetIndex = Mathf.Max(0, siblingCount - 1 - _initialChildren);
				go.transform.SetSiblingIndex(targetIndex);
			}
			go.GetComponentInChildren<IWriteablePortraitReference>().Setup(yingReference);

			return go;
		}
		private void RemovePortrait(GameObject obj)
		{
			Destroy(obj);
		}

		void ReflectYings()
		{
			_yingEnumerableReflector.Enumerate(_yingletRepository.GetYinglets(_group));
		}
	}
}