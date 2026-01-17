using Reactivity;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColorOnHover : ReactiveBehaviour
{
	[SerializeField] Color _targetColor;
	[SerializeField] Graphic[] _targets;
	[SerializeField] SharedEaseSettings _easeSettings;
	private IHoverable _hoverable;
	private Color _originalColor;
	private Coroutine _transitionCoroutine;

	private void Awake()
	{
		_hoverable = this.GetComponentInParent<IHoverable>();
		_originalColor = _targets.First().color;
		UpdateColors(_originalColor);
	}

	void Start()
	{
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		Color from = _targets.First().color;
		Color to = _hoverable.Hovered.Val ? _targetColor : _originalColor;
		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => UpdateColors(Color.LerpUnclamped(from, to, p)));
	}

	void UpdateColors(Color c)
	{
		foreach (var target in _targets)
		{
			target.color = c;
		}
	}
}

