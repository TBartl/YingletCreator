using System.Linq;
using UnityEngine;



namespace Character.Creator.UI
{
	public class CharacterCreatorTogglePortraitIdGroup : MonoBehaviour
	{
		private ICompositeResourceLoader _resourceLoader;
		[SerializeField] GameObject _togglePrefab;

		private void Awake()
		{
			_resourceLoader = Singletons.GetSingleton<ICompositeResourceLoader>();
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}
		}
		private void Start()
		{
			var allPortraits = _resourceLoader.LoadAllPortraitIds()
				.OrderBy(pose => pose.OrderIndex)
				.ToArray();
			foreach (var portraitId in allPortraits)
			{
				var go = GameObject.Instantiate(_togglePrefab, this.transform);
				go.GetComponent<ICharacterCreatorTogglePortraitIdReference>().PortraitId = portraitId;
			}
		}
	}
}