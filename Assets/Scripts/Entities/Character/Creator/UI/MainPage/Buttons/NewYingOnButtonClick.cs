using UnityEngine;
using UnityEngine.UI;

namespace Character.Creator.UI
{
	public class NewYingOnButtonClick : MonoBehaviour
	{
		private Button _button;
		private ISelectedYingletDiskIO _diskIO;
		private ICharacterCreatorUndoManager _undoManager;
		private ScrollContentUpdater _scrollContentUpdater;

		private void Awake()
		{
			_button = this.GetComponent<Button>();
			_button.onClick.AddListener(Button_OnClick);

			_diskIO = Singletons.GetSingleton<ISelectedYingletDiskIO>();
			_undoManager = Singletons.GetSingleton<ICharacterCreatorUndoManager>();
			_scrollContentUpdater = new ScrollContentUpdater(this.transform);
		}

		private void OnDestroy()
		{
			_button.onClick.RemoveListener(Button_OnClick);
		}

		private void Button_OnClick()
		{
			_undoManager.RecordState("Created yinglet");
			_scrollContentUpdater.ApplyAndRestoreScrollPosition(() =>
			{
				_diskIO.DuplicateSelected();
			});
		}
	}
}