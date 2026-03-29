using Character.Creator.UI;
using Reactivity;

public class SmartDisableOnNoUi : ReactiveBehaviour
{
	private IPhotoModeChecker _photoModeState;
	private ISmartDisabler _smartDisabler;

	private void Start()
	{
		_photoModeState = this.GetComponentInParent<IPhotoModeChecker>();
		_smartDisabler = this.GetComponent<ISmartDisabler>();
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		bool inPhotoMode = _photoModeState.IsInPhotoMode.Val;
		_smartDisabler.SetActive(!inPhotoMode, this);
	}
}
