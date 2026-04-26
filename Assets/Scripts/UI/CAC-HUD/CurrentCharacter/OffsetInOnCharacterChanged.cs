using UnityEngine;

public class OffsetInOnCharacterChanged : MonoBehaviour
{
	[SerializeField] SharedEaseSettings _easeSettings;
	[SerializeField] Vector2 _offset;

	private Coroutine _transitionCoroutine;
	private IPartyMemberHUDReference _reference;
	private RectTransform _rectTransform;
	private Vector2 _originalAnchoredPosition;

	private void Start()
	{
		_rectTransform = this.GetComponent<RectTransform>();
		_originalAnchoredPosition = _rectTransform.anchoredPosition;
		_reference = this.GetComponentInParentSafe<IPartyMemberHUDReference>();
		_reference.CharacterGameObjectObservable.OnChanged += CharacterGameObject_OnChanged;
	}
	private void OnDestroy()
	{
		if (_reference != null)
		{
			_reference.CharacterGameObjectObservable.OnChanged -= CharacterGameObject_OnChanged;
		}
	}

	private void CharacterGameObject_OnChanged(GameObject from, GameObject to)
	{
		Vector2 fromPos = _rectTransform.anchoredPosition + _offset;
		Vector2 toPos = _originalAnchoredPosition;
		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => _rectTransform.anchoredPosition = Vector2.LerpUnclamped(fromPos, toPos, p));
	}
}
