using UnityEngine;

namespace Character.Creator.UI
{
	public class ColorCopyPastingToasts : MonoBehaviour
	{
		private IToastManager _toastDisplay;
		private IColorCopyPasting _copyPasting;

		private void Awake()
		{
			_toastDisplay = Singletons.GetSingleton<IToastManager>();
			_copyPasting = this.GetComponent<IColorCopyPasting>();
			_copyPasting.Copied += CopyPasting_Copied;
			_copyPasting.Pasted += CopyPasting_Pasted;
			_copyPasting.PasteFailedInvalidFormat += CopyPasting_PasteFailedInvalidFormat;
		}

		private void OnDestroy()
		{
			_copyPasting.Copied -= CopyPasting_Copied;
			_copyPasting.Pasted -= CopyPasting_Pasted;
			_copyPasting.PasteFailedInvalidFormat -= CopyPasting_PasteFailedInvalidFormat;
		}
		private void CopyPasting_Copied()
		{
			_toastDisplay.Show("Hex color copied to clipboard");
		}
		private void CopyPasting_Pasted()
		{
			_toastDisplay.Show("Hex color pasted from clipboard");
		}
		private void CopyPasting_PasteFailedInvalidFormat()
		{
			_toastDisplay.Show("Paste failed; no valid hex color on clipboard");
		}
	}
}