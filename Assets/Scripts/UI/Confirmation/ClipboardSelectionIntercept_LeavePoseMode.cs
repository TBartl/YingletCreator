using Character.Creator.UI;
using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class used to potentially interrupt clipboard selection
/// Currently used for showing a confirmation when exiting pose mode
/// </summary>
public interface IClipboardSelectionIntercept
{
	void OnSelect(ClipboardSelectionType from, ClipboardSelectionType to, Action execute);
}

public class ClipboardSelectionIntercept_LeavePoseMode : MonoBehaviour, IClipboardSelectionIntercept
{
	[SerializeField] ClipboardSelectionType _selection;
	private IConfirmationManager _confirmationManager;
	private IPoseData _poseData;

	private void Start()
	{
		_confirmationManager = Singletons.GetSingleton<IConfirmationManager>();
		_poseData = this.GetComponentInParent<IPoseData>();
	}

	public void OnSelect(ClipboardSelectionType from, ClipboardSelectionType to, Action execute)
	{
		// Only if we're leaving pose mode and we've selected some number of yinglets
		if (from == _selection && to != _selection && _poseData.Data.Any())
		{
			_confirmationManager.OpenConfirmation(new(
				"Are you sure you want to leave pose mode?\n\nThis will reset the scene.",
				"Leave Pose Mode",
				"leave-pose-mode",
				execute));
			return;
		}
		execute();
	}
}
