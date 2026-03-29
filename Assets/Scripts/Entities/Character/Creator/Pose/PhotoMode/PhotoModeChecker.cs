using Reactivity;
using UnityEngine;

namespace Character.Creator.UI
{
	public interface IPhotoModeChecker
	{
		MenuType PhotoModeMenu { get; }
		IReadOnlyObservable<bool> IsInPhotoMode { get; }
	}
	public class PhotoModeChecker : ReactiveBehaviour, IPhotoModeChecker
	{
		[SerializeField] MenuType _photoModeMenu;
		private IMenuManager _menuManager;
		Computed<bool> _isInPhotoMode;
		public IReadOnlyObservable<bool> IsInPhotoMode => _isInPhotoMode;

		public MenuType PhotoModeMenu => _photoModeMenu;

		private void Awake()
		{
			_menuManager = Singletons.GetSingleton<IMenuManager>();
			_isInPhotoMode = CreateComputed(ComputeIsInPhotoMode);
		}

		private bool ComputeIsInPhotoMode()
		{
			return _menuManager.OpenMenu.Val == _photoModeMenu;
		}
	}
}