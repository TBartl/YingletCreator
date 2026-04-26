using Reactivity;
using UnityEngine;

namespace Character.Creator.UI
{
	public class ColorSelectionLeftAlignOnSelected : ReactiveBehaviour
	{
		[SerializeField] SharedEaseSettings _easeSettings;
		private ISelectable _selectable;
		private Coroutine _transitionCoroutine;
		private RectTransform _rt;
		private Vector2 _initialOffset;

		private void Awake()
		{
			_rt = this.GetComponent<RectTransform>();
			_initialOffset = _rt.offsetMin;
			_selectable = this.GetComponentInParentSafe<ISelectable>();
		}

		void Start()
		{
			AddReflector(Reflect);
		}

		private void Reflect()
		{
			Vector2 from = _rt.offsetMin;
			Vector2 to = _selectable.Selected.Val ? Vector2.zero : _initialOffset;
			this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => _rt.offsetMin = Vector2.LerpUnclamped(from, to, p));
		}
	}
}