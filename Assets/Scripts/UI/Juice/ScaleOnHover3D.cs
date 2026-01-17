using Reactivity;
using UnityEngine;

public class ScaleOnHover3D : ReactiveBehaviour
{
	[SerializeField] Vector3 _hoverScale;
	[SerializeField] SharedEaseSettings _easeSettings;
	private IHoverable _hoverable;
	private Coroutine _transitionCoroutine;

	private void Awake()
	{
		_hoverable = this.GetComponentInParent<IHoverable>();
	}

	void Start()
	{
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		Vector3 from = this.transform.localScale;
		Vector3 to = _hoverable.Hovered.Val ? _hoverScale : Vector3.one;
		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => this.transform.localScale = Vector3.LerpUnclamped(from, to, p));
	}
}
