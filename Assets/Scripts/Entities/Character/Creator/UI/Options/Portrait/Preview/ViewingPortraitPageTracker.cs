using Character.Creator.UI;
using Reactivity;
using UnityEngine;

public interface IViewingPortraitPageTracker
{
	bool IsViewingPortraitPage { get; }
}

public class ViewingPortraitPageTracker : ReactiveBehaviour, IViewingPortraitPageTracker
{
	[SerializeField] ClipboardSelection _selectionReference; // Scene reference is not ideal, but w/e
	[SerializeField] ClipboardSelectionType _selectionType;

	private ICharacterCreatorTracker _characterCreatorTracker;

	Computed<bool> _isViewingPortraitPage;

	public bool IsViewingPortraitPage => _isViewingPortraitPage.Val;

	private void Awake()
	{
		_characterCreatorTracker = Singletons.GetSingleton<ICharacterCreatorTracker>();

		_isViewingPortraitPage = CreateComputed(ComputeViewingPortraitPage);
	}

	private bool ComputeViewingPortraitPage()
	{
		if (!_characterCreatorTracker.IsInCharacterCreator.Val)
		{
			return false;
		}
		return _selectionReference.Selection.Val == _selectionType;
	}
}
