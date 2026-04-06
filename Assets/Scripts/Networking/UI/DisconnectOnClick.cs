using UnityEngine;
using UnityEngine.UI;

public class DisconnectOnClick : MonoBehaviour
{
	private INetStateWriter _netStateWriter;
	private Button _button;

	void Start()
	{
		_netStateWriter = Singletons.GetSingleton<INetStateWriter>();
		_button = this.GetComponent<Button>();
		_button.onClick.AddListener(Button_OnClick);
	}

	private void OnDestroy()
	{
		_button?.onClick.RemoveListener(Button_OnClick);
	}

	private void Button_OnClick()
	{
		_netStateWriter.Disconnect();
	}
}