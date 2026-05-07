using Reactivity;
using UnityEngine;

public class ShakeOnResourceCountChanged : ReactiveBehaviour
{
	[SerializeField] AssetReferenceT<CharacterResourceId> _resource;
	[SerializeField] Vector3 _offsetAmount;
	[SerializeField] SharedEaseSettings _easeSettings;
	private RectTransform _rectTransform;
	private Coroutine _transitionCoroutine;
	private Vector3 _originalPos;

	private IPartyMemberHUDReference _reference;
	Computed<ICharacterResources> _characterResources;
	private Computed<int> _resourceCount;
	private ICharacterRoot _lastCharacter;

	void Start()
	{
		_rectTransform = this.GetComponent<RectTransform>();
		_originalPos = _rectTransform.anchoredPosition3D;
		_reference = this.GetComponentInParentSafe<IPartyMemberHUDReference>();
		_characterResources = CreateComputed(ComputeCharacterResources);
		_resourceCount = CreateComputed(() => _characterResources.Val?.GetResource(_resource.LoadSync()) ?? 0);
		_resourceCount.OnChanged += OnResourceCountChanged;
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		if (_resourceCount != null)
			_resourceCount.OnChanged -= OnResourceCountChanged;
	}

	private void OnResourceCountChanged(int fromVal, int toVal)
	{
		var lastCharacter = _lastCharacter;
		var thisCharacter = _reference.Character;
		_lastCharacter = thisCharacter;
		if (lastCharacter != null && thisCharacter != lastCharacter)
		{
			return;
		}

		Vector3 from = _originalPos;
		Vector3 to = _originalPos + _offsetAmount;
		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => _rectTransform.anchoredPosition3D = Vector3.LerpUnclamped(from, to, p));
	}

	private ICharacterResources ComputeCharacterResources()
	{
		var characterGameObject = _reference.Character;
		if (characterGameObject == null) return null;
		return characterGameObject.GetComponentInChildrenSafe<ICharacterResources>();
	}
}
