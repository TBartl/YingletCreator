using UnityEngine;

namespace Character.Creator.UI
{
	/// <summary>
	/// When the user clicks off the page, this will move with the page
	/// </summary>
	public interface IFakeBookmark
	{
		void Setup(GameObject realBookmark);
	}

	public class FakeBookmark : MonoBehaviour, IFakeBookmark
	{
		private IBookmarkImageControl _imageControl;
		private IClipboardOrdering _clipboardOrdering;
		private IClipboardFreeFallManager _freeFallManager;
		private ISelectable _selectable;
		private IClipboardElementSelection _elementSelection;
		private Vector3 _originalPos;
		private Quaternion _originalRot;

		void Awake()
		{
			_imageControl = this.GetComponent<IBookmarkImageControl>();
			_clipboardOrdering = this.GetComponentInParent<IClipboardOrdering>();
			_freeFallManager = this.GetComponentInParent<IClipboardFreeFallManager>();
			_selectable = this.GetComponentSafe<ISelectable>();
			_elementSelection = this.GetComponent<IClipboardElementSelection>();
		}

		void Start()
		{
			_selectable.Selected.OnChanged += Selected_OnChanged;
			this.gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			_selectable.Selected.OnChanged -= Selected_OnChanged;
		}

		public void Setup(GameObject realBookmark)
		{
			_imageControl.CopyValuesFrom(realBookmark.GetComponent<IBookmarkImageControl>());
			this.transform.position = realBookmark.transform.position;
			_originalPos = this.transform.localPosition;
			_originalRot = this.transform.localRotation;

			var selectiontype = realBookmark.GetComponent<IClipboardElementSelection>().Type;
			_elementSelection.Type = selectiontype;

			_clipboardOrdering.SendToLayer(this.transform, ClipboardLayer.Back);
		}

		private void Selected_OnChanged(bool wasSelected, bool isSelected)
		{
			if (wasSelected)
			{
				this.gameObject.SetActive(true);
				_clipboardOrdering.SendToLayer(this.transform, ClipboardLayer.Back);
				ResetTransform();
				_freeFallManager.FreeFall(this.transform);
			}
		}

		void ResetTransform()
		{
			this.transform.localPosition = _originalPos;
			this.transform.localRotation = _originalRot;
		}
	}
}