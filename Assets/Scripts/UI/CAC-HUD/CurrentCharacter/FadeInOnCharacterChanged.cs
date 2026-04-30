using UnityEngine;

public class FadeInOnCharacterChanged : MonoBehaviour
{
	[SerializeField] SharedEaseSettings _easeSettings;

	private Coroutine _transitionCoroutine;
	private CanvasGroup _canvasGroup;
	private IPartyMemberHUDReference _reference;

	private void Start()
	{
		_canvasGroup = this.GetComponentSafe<CanvasGroup>();
		_reference = this.GetComponentInParentSafe<IPartyMemberHUDReference>();
		_reference.CharacterObservable.OnChanged += CharacterGameObject_OnChanged;
	}
	private void OnDestroy()
	{
		if (_reference != null)
		{
			_reference.CharacterObservable.OnChanged -= CharacterGameObject_OnChanged;
		}
	}

	private void CharacterGameObject_OnChanged(ICharacterRoot from, ICharacterRoot to)
	{
		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => _canvasGroup.alpha = Mathf.Lerp(0, 1, p));
	}
}
