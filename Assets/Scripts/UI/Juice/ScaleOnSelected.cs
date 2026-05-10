using Reactivity;
using UnityEngine;

public class ScaleOnSelected : ReactiveBehaviour
{
	[SerializeField] Vector3 _scale;
	[SerializeField] SharedEaseSettings _easeSettings;
	private Vector3 _originalScale;
	private ISelectable _selectable;
	private Coroutine _transitionCoroutine;

	void Start()
	{
		_selectable = this.GetComponentInParentSafe<ISelectable>();
		_originalScale = this.transform.localScale;
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		Vector3 from = this.transform.localScale;
		Vector3 to = _selectable.Selected.Val ? _scale : _originalScale;
		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => this.transform.localScale = Vector3.LerpUnclamped(from, to, p));
	}
}

