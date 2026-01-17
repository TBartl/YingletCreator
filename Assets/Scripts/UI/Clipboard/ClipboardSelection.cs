using Reactivity;
using UnityEngine;

namespace Character.Creator.UI
{
	public interface IClipboardSelection
	{
		Observable<ClipboardSelectionType> Selection { get; }

		void SetSelection(ClipboardSelectionType selection);

		Transform transform { get; }
	}

	public class ClipboardSelection : MonoBehaviour, IClipboardSelection
	{
		[SerializeField] ClipboardSelectionType _initialSelection;
		private IClipboardSelectionIntercept _intercept;

		public Observable<ClipboardSelectionType> Selection { get; } = new Observable<ClipboardSelectionType>();

		void Start()
		{
			Selection.Val = _initialSelection;

			_intercept = this.GetComponent<IClipboardSelectionIntercept>();
		}

		public void SetSelection(ClipboardSelectionType selection)
		{
			if (_intercept == null)
			{
				Selection.Val = selection;
				return;
			}
			else
			{
				_intercept.OnSelect(Selection.Val, selection, () =>
				{
					Selection.Val = selection;
				});
			}
		}
	}
}