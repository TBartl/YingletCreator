using Reactivity;
using UnityEngine;

public class ScaleOnHover : ReactiveBehaviour
{
	[SerializeField] float _hoverScale;
	[SerializeField] SharedEaseSettings _easeSettings;
	private IHoverable _hoverable;
	private Vector3 _originalScale;
	private Coroutine _transitionCoroutine;

	private void Awake()
	{
		_hoverable = this.GetComponentInParent<IHoverable>();
		_originalScale = this.transform.localScale;
	}

	void Start()
	{
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		Vector3 from = this.transform.localScale;
		Vector3 to = _hoverable.Hovered.Val ? _originalScale * _hoverScale : _originalScale;
		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => this.transform.localScale = Vector3.LerpUnclamped(from, to, p));
	}
}
