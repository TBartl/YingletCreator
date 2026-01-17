using Reactivity;

namespace Character.Creator.UI
{
	public class BookmarkSelfLayerer : ReactiveBehaviour
	{

		private ISelectable _elementSelection;
		private IClipboardOrdering _clipboardOrdering;


		private void Awake()
		{
			_elementSelection = this.GetComponent<ISelectable>();
			_clipboardOrdering = this.GetComponentInParent<IClipboardOrdering>();
		}

		private void Start()
		{
			this.AddReflector(ReflectSelected);
		}

		private void ReflectSelected()
		{
			bool isSelected = _elementSelection.Selected.Val;
			_clipboardOrdering.SendToLayer(this.transform, isSelected ? ClipboardLayer.ActiveBookmark : ClipboardLayer.Back);
		}
	}
}