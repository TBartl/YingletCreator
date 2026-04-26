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
		_reference.CharacterGameObjectObservable.OnChanged += CharacterGameObject_OnChanged;
	}
	private void OnDestroy()
	{
		if (_reference.CharacterGameObjectObservable != null)
		{
			_reference.CharacterGameObjectObservable.OnChanged -= CharacterGameObject_OnChanged;
		}
	}

	private void CharacterGameObject_OnChanged(GameObject from, GameObject to)
	{
		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => _canvasGroup.alpha = Mathf.Lerp(0, 1, p));
	}
}
