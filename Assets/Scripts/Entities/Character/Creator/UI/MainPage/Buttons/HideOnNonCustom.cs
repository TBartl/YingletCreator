using Reactivity;

namespace Character.Creator.UI
{
    public class HideOnNonCustom : ReactiveBehaviour
    {
        private ICustomizationSelection _selection;

        private void Awake()
        {
            _selection = this.GetComponentInParent<ICustomizationSelection>();
        }

        private void Start()
        {
            AddReflector(Reflect);
        }

        private void Reflect()
        {
            this.gameObject.SetActive(IsCustomSelected());
        }

        bool IsCustomSelected()
        {

            if (_selection.Selected == null)
            {
                return false;
            }
            return _selection.Selected.Group == LocalYingletGroup.Custom;
        }
    }
}