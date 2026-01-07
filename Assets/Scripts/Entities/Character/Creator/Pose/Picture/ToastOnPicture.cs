using UnityEngine;

public class ToastOnPicture : MonoBehaviour
{
	private IToastManager _toastDisplay;
	private ITakePictureEvents _picEvents;

	private void Awake()
	{
		_toastDisplay = Singletons.GetSingleton<IToastManager>();
		_picEvents = this.GetComponent<ITakePictureEvents>();
		_picEvents.PictureTaken += OnPictureTaken;
	}

	private void OnDestroy()
	{
		_picEvents.PictureTaken -= OnPictureTaken;
	}

	private void OnPictureTaken(string fileName)
	{
		_toastDisplay.Show("Picture taken: " + fileName);
	}
}
