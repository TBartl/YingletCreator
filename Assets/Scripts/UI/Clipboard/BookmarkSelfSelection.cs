using Reactivity;
using UnityEngine.EventSystems;

namespace Character.Creator.UI
{
    public class BookmarkSelfSelection : ReactiveBehaviour, IPointerClickHandler
    {
        private IClipboardElementSelection _elementSelection;

        void Awake()
        {
            _elementSelection = this.GetComponent<IClipboardElementSelection>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _elementSelection.SetSelected();
        }
    }
}