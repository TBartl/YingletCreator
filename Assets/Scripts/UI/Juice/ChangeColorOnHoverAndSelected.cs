using Reactivity;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColorOnHoverAndSelected : ReactiveBehaviour
{
	[SerializeField] Color _hoverColor;
	[SerializeField] Color _selectColor;
	[SerializeField] Graphic[] _targets;
	[SerializeField] SharedEaseSettings _easeSettings;
	private IHoverable _hoverable;
	private ISelectable _selectable;
	private Color _originalColor;
	private Coroutine _transitionCoroutine;

	private void Awake()
	{
		_hoverable = this.GetComponentInParent<IHoverable>();
		_selectable = this.GetComponentInParent<ISelectable>();
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
		Color to = GetTargetColor();
		if (this.isActiveAndEnabled)
		{
			this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => UpdateColors(Color.LerpUnclamped(from, to, p)));
		}
		else
		{
			UpdateColors(to);
		}
	}

	Color GetTargetColor()
	{
		if (_selectable.Selected.Val)
		{
			return _selectColor;
		}
		if (_hoverable.Hovered.Val)
		{
			return _hoverColor;
		}
		return _originalColor;
	}

	void UpdateColors(Color c)
	{
		foreach (var target in _targets)
		{
			target.color = c;
		}
	}
}

