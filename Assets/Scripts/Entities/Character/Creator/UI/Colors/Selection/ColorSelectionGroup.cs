using Character.Compositor;
using Character.Data;
using Reactivity;
using System.Linq;
using UnityEngine;

namespace Character.Creator.UI
{
	public class ColorSelectionGroup : ReactiveBehaviour
	{
		private ICharacterSpawner _characterSpawner;
		private IColorSelectionSorter _sorter;
		private Computed<ITextureGatherer> _currentGatherer;
		[SerializeField] GameObject _colorSelectionPrefab;

		EnumerableDictReflector<ReColorId, GameObject> _enumerableReflector;

		private void Awake()
		{
			_characterSpawner = Singletons.GetSingleton<ICharacterSpawner>();
			_sorter = this.GetComponentInParent<IColorSelectionSorter>();
			_currentGatherer = CreateComputed(ComputeGatherer);

			_enumerableReflector = new(Create, Delete);
			// Clean up any dummy objects under this
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}
		}

		private ITextureGatherer ComputeGatherer()
		{
			var myCharacter = _characterSpawner.MyCharacter;
			if (myCharacter == null) return null;
			return myCharacter.GetComponentInChildren<ITextureGatherer>();
		}

		private GameObject Create(ReColorId id)
		{
			var go = Instantiate(_colorSelectionPrefab, this.transform);
			go.GetComponent<IColorSelectionReference>().Id = id;
			_sorter.PositionSorted(this.transform, go);
			return go;
		}

		private void Delete(GameObject gameObject)
		{
			Destroy(gameObject);
		}

		private void Start()
		{
			AddReflector(ReflectColors);
		}

		void ReflectColors()
		{
			var gatherer = _currentGatherer.Val;
			if (gatherer == null)
			{
				return;
			}

			var recolorIds = gatherer.AllRelevantTextures
				.ToArray()
				.Select(t => t.ReColorId)
				.Where(i => i != null)
				.ToHashSet();
			_enumerableReflector.Enumerate(recolorIds);
		}

	}
}