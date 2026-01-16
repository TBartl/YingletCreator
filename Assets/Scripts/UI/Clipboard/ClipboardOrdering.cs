using UnityEngine;


namespace Character.Creator.UI
{
    public enum ClipboardLayer
    {
        /// <summary>
        /// Normal area. Only active object here should be the unselected bookmarks
        /// </summary>
        Back,

        /// <summary>
        /// The selected page should live here
        /// </summary>
        ActivePage,

        /// <summary>
        /// The topmost bookmark should live here
        /// </summary>
        ActiveBookmark,

        /// <summary>
        /// Anything actively freefalling should live here
        /// </summary>
        Freefall
    }
    public interface IClipboardOrdering
    {
        void SendToLayer(Transform transform, ClipboardLayer layer);
    }
    public class ClipboardOrdering : MonoBehaviour, IClipboardOrdering
    {
        [SerializeField] RectTransform _back;
        [SerializeField] RectTransform _activePage;
        [SerializeField] RectTransform _activeBookmark;
        [SerializeField] RectTransform _freeFall;


        public void SendToLayer(Transform transform, ClipboardLayer layer)
        {
            var parent = layer switch
            {
                ClipboardLayer.Back => _back,
                ClipboardLayer.ActivePage => _activePage,
                ClipboardLayer.ActiveBookmark => _activeBookmark,
                ClipboardLayer.Freefall => _freeFall,
                _ => null
            };

            transform.SetParent(parent);
            transform.SetSiblingIndex(parent.childCount - 1);
        }
    }
}