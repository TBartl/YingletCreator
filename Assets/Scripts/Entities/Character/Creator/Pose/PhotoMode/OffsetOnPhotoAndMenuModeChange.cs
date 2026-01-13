using Reactivity;
using System.Linq;
using UnityEngine;

namespace Character.Creator.UI
{

	enum PhotoAndMenuState
	{
		None,
		Photo,
		Menu
	}

	public class OffsetOnPhotoAndMenuModeChange : ReactiveBehaviour
	{
		[SerializeField] SharedEaseSettings _easeSettings;
		[SerializeField] Vector3 _offset;
		[SerializeField] Vector3 _photoOffset;
		[SerializeField] Vector3 _menuOffset;

		[SerializeField] MenuType[] _hideOnMenus;

		private IPhotoModeState _photoModeState;
		private Vector3 _originalPos;
		private IMenuManager _menuManager;
		private Coroutine _transitionCoroutine;

		Computed<PhotoAndMenuState> _photoAndMenuState;


		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			_photoModeState = this.GetComponentInParent<IPhotoModeState>();
			_originalPos = this.transform.localPosition;

			_menuManager = Singletons.GetSingleton<IMenuManager>();
			_photoAndMenuState = CreateComputed(ComputePhotoAndMenuState);

			_photoAndMenuState.OnChanged += OnPhotoAndMenuStateChanged;
			this.transform.localPosition = GetToPos(_photoAndMenuState.Val);
		}

		private PhotoAndMenuState ComputePhotoAndMenuState()
		{
			if (_menuManager.OpenMenu.Val != null)
			{
				if (_hideOnMenus.Contains(_menuManager.OpenMenu.Val))
				{
					// Effectively special case logic for screensaver
					return PhotoAndMenuState.Photo;
				}
				return PhotoAndMenuState.Menu;
			}
			if (_photoModeState.IsInPhotoMode.Val)
			{
				return PhotoAndMenuState.Photo;
			}
			return PhotoAndMenuState.None;
		}

		private void OnPhotoAndMenuStateChanged(PhotoAndMenuState from, PhotoAndMenuState to)
		{
			var fromPos = this.transform.localPosition;
			var toPos = GetToPos(to);
			this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => this.transform.localPosition = Vector3.LerpUnclamped(fromPos, toPos, p));
		}

		Vector3 GetToPos(PhotoAndMenuState to)
		{
			return to switch
			{
				PhotoAndMenuState.None => _originalPos,
				PhotoAndMenuState.Photo => _originalPos + _photoOffset,
				PhotoAndMenuState.Menu => _originalPos + _menuOffset,
				_ => _originalPos
			};
		}
	}
}
