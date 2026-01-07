using UnityEngine;

public class ToastDisplay : MonoBehaviour
{
	[SerializeField] GameObject _toastPrefab;
	private IToastManager _toastManager;

	private void Awake()
	{
		_toastManager = Singletons.GetSingleton<IToastManager>();
		_toastManager.Toasted += Show;
	}

	private void OnDestroy()
	{
		_toastManager.Toasted -= Show;
	}

	public void Show(string message)
	{
		var go = GameObject.Instantiate(_toastPrefab, this.transform);

		go.GetComponentInChildren<TMPro.TMP_Text>().text = message;
	}
}
