using Reactivity;
using UnityEngine;

namespace Character.Creator.UI
{
    /// <summary>
    /// Various elements on a clipboard need to know if they're enabled or not:
    /// - The bookmark
    /// - The page
    /// - The fake bookmark
    /// This provides a common mechanism for them all to know this information
    /// </summary>
    internal interface IClipboardElementSelection : ISelectable
    {
        void SetSelected();
        ClipboardSelectionType Type { get; set; }
    }

    public class ClipboardElementSelection : ReactiveBehaviour, IClipboardElementSelection
    {
        [SerializeField] ClipboardSelectionType _initialType;
        private IClipboardSelection _clipboardSelection;
        private Computed<bool> _isSelected;

        public IReadOnlyObservable<bool> Selected => _isSelected;

        Observable<ClipboardSelectionType> _type = new Observable<ClipboardSelectionType>();
        public ClipboardSelectionType Type
        {
            get => _type.Val;
            set => _type.Val = value;
        }

        public void SetSelected()
        {
            _clipboardSelection.SetSelection(_initialType);
        }

        private void Awake()
        {
            _clipboardSelection = this.GetComponentInParent<IClipboardSelection>();
            _type.Val = _initialType;
            _isSelected = CreateComputed<bool>(ComputeIsSelected);
        }

        private bool ComputeIsSelected()
        {
            return _clipboardSelection.Selection.Val == _type.Val;
        }
    }
}