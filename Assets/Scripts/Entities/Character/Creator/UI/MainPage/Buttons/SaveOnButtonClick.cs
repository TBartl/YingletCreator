using UnityEngine;
using UnityEngine.UI;

namespace Character.Creator.UI
{
	public class SaveOnButtonClick : MonoBehaviour
	{
		private Button _button;
		private ISelectedYingletDiskIO _diskIO;

		private void Awake()
		{
			_button = this.GetComponent<Button>();
			_button.onClick.AddListener(Button_OnClick);

			_diskIO = this.GetComponentInParent<ISelectedYingletDiskIO>();
		}

		private void OnDestroy()
		{
			_button.onClick.RemoveListener(Button_OnClick);
		}

		private void Button_OnClick()
		{
			_diskIO.SaveSelected();
		}
	}
}