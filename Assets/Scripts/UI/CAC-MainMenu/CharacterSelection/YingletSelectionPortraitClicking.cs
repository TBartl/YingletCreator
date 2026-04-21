using Reactivity;
using UnityEngine.UI;

namespace Character.Creator.UI
{
	public class YingletSelectionPortraitClicking : ReactiveBehaviour
	{
		private IMainMenuYingletSelection _yingletSelection;
		private IPortraitReference _reference;
		private Button _button;

		private void Awake()
		{
			_yingletSelection = Singletons.GetSingleton<IMainMenuYingletSelection>();
			_reference = this.GetComponent<IPortraitReference>();
			_button = this.GetComponent<Button>();
			_button.onClick.AddListener(Button_OnClick);

		}

		private new void OnDestroy()
		{
			base.OnDestroy();
			_button.onClick.RemoveListener(Button_OnClick);
		}

		private void Button_OnClick()
		{
			_yingletSelection.Selected = _reference.Reference;
		}
	}
}
