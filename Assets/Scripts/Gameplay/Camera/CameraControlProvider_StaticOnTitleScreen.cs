using Reactivity;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Camera
{
	internal sealed class CameraControlProvider_StaticOnTitleScreen : ReactiveBehaviour, ICameraControlProvider
	{
		[SerializeField] MenuType _titleScreenMenu;

		private IMenuManager _menuManager;
		Computed<bool> _wantsControl;
		private Vector3 _originalPos;
		private Quaternion _originalRot;

		public bool WantsControl => _wantsControl.Val;

		private void Awake()
		{
			_menuManager = Singletons.GetSingleton<IMenuManager>();
			_originalPos = this.transform.position;
			_originalRot = this.transform.rotation;
			_wantsControl = CreateComputed(ComputeWantsControl);
		}

		private bool ComputeWantsControl()
		{
			return _menuManager.OpenMenu.Val == _titleScreenMenu;
		}
		public (Vector3, Quaternion) CalculateTransform()
		{
			return (_originalPos, _originalRot);
		}
	}
}