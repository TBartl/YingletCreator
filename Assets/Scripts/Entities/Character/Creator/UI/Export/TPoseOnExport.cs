using UnityEngine;

public class TPoseOnExport : MonoBehaviour
{
	[SerializeField] private Animator _animator;
	[SerializeField] private EyeControl _eyeControl;
	[SerializeField] private MenuType _exportMenu;
	[SerializeField] private GameObject[] _needsDisable;
	[SerializeField] private Transform[] _needsUnrotation;

	private IMenuManager _menuManager;
	private Quaternion[] _savedRotations;
	private double[] _savedLayerWeights;

	private void Awake()
	{
		_menuManager = Singletons.GetSingleton<IMenuManager>();
		_menuManager.OpenMenu.OnChanged += Menu_OnOpenChanged;
	}

	private void OnDestroy()
	{
		_menuManager.OpenMenu.OnChanged -= Menu_OnOpenChanged;
	}

	private void Menu_OnOpenChanged(MenuType fromMenu, MenuType toMenu)
	{
		if (toMenu == _exportMenu)
		{
			// Save the rotations and set them to identity.
			if (_savedRotations is null || _savedRotations.Length < _needsUnrotation.Length)
			{
				_savedRotations = new Quaternion[_needsUnrotation.Length];
			}
			for (int i = 0; i < _needsUnrotation.Length; i++)
			{
				_savedRotations[i] = _needsUnrotation[i].localRotation;
				_needsUnrotation[i].localRotation = Quaternion.identity;
			}
			// Save the weights of other layers and set them to zero.
			if (_savedLayerWeights is null || _savedLayerWeights.Length < _animator.layerCount)
			{
				_savedLayerWeights = new double[_animator.layerCount];
			}
			for (int i = 0; i < _animator.layerCount; i++)
			{
				_savedLayerWeights[i] = _animator.GetLayerWeight(i);
				_animator.SetLayerWeight(i, 0.0f);
			}
			for (int i = 0; i < _needsDisable.Length; i++)
			{
				_needsDisable[i].SetActive(false);
			}
			// Enable the TPose layer.
			_animator.SetLayerWeight(_animator.GetLayerIndex("TPose"), 1.0f);
			_eyeControl.IdleEyeMovementEnabled = false;
		}
		else
		{
			// Restore the rotations, if any were saved.
			if (_savedRotations != null && _savedRotations.Length >= _needsUnrotation.Length)
			{
				for (int i = 0; i < _needsUnrotation.Length; i++)
				{
					_needsUnrotation[i].localRotation = _savedRotations[i];
				}
			}
			// Restore the saved weights of other layers, if any were saved.
			if (_savedLayerWeights != null && _savedLayerWeights.Length >= _animator.layerCount)
			{
				for (int i = 0; i < _animator.layerCount; i++)
				{
					_animator.SetLayerWeight(i, (float)_savedLayerWeights[i]);
				}
			}
			for (int i = 0; i < _needsDisable.Length; i++)
			{
				_needsDisable[i].SetActive(true);
			}
			// Disable the TPose layer.
			_animator.SetLayerWeight(_animator.GetLayerIndex("TPose"), 0.0f);
			_eyeControl.IdleEyeMovementEnabled = true;
		}
	}
}
