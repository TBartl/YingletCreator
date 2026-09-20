using Reactivity;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class ChangeColorOnBool : ReactiveBehaviour
{
	[SerializeField] private Graphic[] _targets;
	[SerializeField] private SharedEaseSettings _easeSettings;
	protected Color _originalColor;
	protected Coroutine _transitionCoroutine;

	protected virtual void Awake()
	{
		_originalColor = _targets.First().color;
		UpdateColors(_originalColor);
	}

	protected void Start()
	{
		AddReflector(Reflect);
	}

	protected abstract Color GetTargetColor();

	private void Reflect()
	{
		Color from = _targets.First().color;
		Color to = GetTargetColor();
		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => UpdateColors(Color.LerpUnclamped(from, to, p)));
	}

	protected void UpdateColors(Color c)
	{
		foreach (var target in _targets)
		{
			target.color = c;
		}
	}
}