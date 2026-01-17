using Reactivity;
using UnityEngine;

namespace Character.Creator.UI
{
	/// <summary>
	/// Creates and controls the "VisualOnly" version of this bookmark
	/// This is used to fall off with the page during transitions
	/// </summary>
	public class BookmarkCreateFakeBookmark : ReactiveBehaviour
	{
		[SerializeField] GameObject _visualOnlyPrefab;

		private void Awake()
		{
			var fakeBookmark = Instantiate(_visualOnlyPrefab, this.transform.parent).GetComponent<IFakeBookmark>();
			fakeBookmark.Setup(this.gameObject);
		}
	}
}