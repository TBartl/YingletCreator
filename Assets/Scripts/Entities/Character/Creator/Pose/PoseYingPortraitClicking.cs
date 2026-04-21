using Reactivity;
using UnityEngine.UI;

namespace Character.Creator.UI
{
	public class PoseYingPortraitClicking : ReactiveBehaviour
	{
		private IPoseData _poseData;
		private IPortraitReference _reference;
		private Button _button;

		private void Awake()
		{
			_poseData = this.GetComponentInParent<IPoseData>();
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
			_poseData.ToggleYing(_reference.Reference);
		}
	}
}
