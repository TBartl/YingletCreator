using Reactivity;
using UnityEngine;
using UnityEngine.UI;

namespace Character.Creator.UI
{
	public interface IBookmarkImageControl
	{
		void CopyValuesFrom(IBookmarkImageControl source);
	}

	public class BookmarkImageControl : ReactiveBehaviour, IBookmarkImageControl
	{
		[SerializeField] Image _fill;
		[SerializeField] Image _icon;
		[SerializeField] Color _selectedIconColor = Color.white;
		[SerializeField] EaseSettings _iconTransitionEaseSettings;
		private Color _unselectedColor;
		private ISelectable _selection;
		private Coroutine _iconTransitionCoroutine;

		private void Awake()
		{
			_unselectedColor = _icon.color;
			_selection = this.GetComponent<ISelectable>();
		}

		void Start()
		{
			AddReflector(ReflectSelected);
		}

		void ReflectSelected()
		{
			bool selected = _selection.Selected.Val;
			var fromColor = _icon.color;
			var toColor = selected ? _selectedIconColor : _unselectedColor;

			if (!this.gameObject.activeInHierarchy)
			{
				_icon.color = toColor;
			}
			else
			{
				this.StartEaseCoroutine(ref _iconTransitionCoroutine, _iconTransitionEaseSettings, p => _icon.color = Color.Lerp(fromColor, toColor, p));
			}
		}

		public void CopyValuesFrom(IBookmarkImageControl source)
		{
			var sourceImpl = (BookmarkImageControl)source;
			_unselectedColor = sourceImpl._unselectedColor;
			_fill.color = sourceImpl._fill.color;
			_icon.color = sourceImpl._icon.color;
			_icon.sprite = sourceImpl._icon.sprite;
		}
	}
}