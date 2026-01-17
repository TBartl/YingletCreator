using Reactivity;
using UnityEngine;

namespace Character.Creator.UI
{
	public class BookmarkCreateFakeBookmarks : ReactiveBehaviour
	{
		[SerializeField] GameObject[] _visualOnlyPrefabs;

		private void Awake()
		{
			foreach (var prefab in _visualOnlyPrefabs)
			{

				var fakeBookmark = Instantiate(prefab, this.transform.parent).GetComponent<IFakeBookmark>();
				fakeBookmark.Setup(this.gameObject);
			}
		}
	}
}