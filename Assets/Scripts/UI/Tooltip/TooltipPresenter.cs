using Reactivity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipPresenter : ReactiveBehaviour, ISelectable
{
	private TMP_Text _text;
	private ITooltipManager _tooltipManager;

	Computed<bool> _selected;
	private RectTransform _childRT;

	public IReadOnlyObservable<bool> Selected => _selected;

	private void Awake()
	{
		_text = this.GetComponentInChildren<TMPro.TMP_Text>();
		_tooltipManager = Singletons.GetSingleton<ITooltipManager>();
		_selected = CreateComputed(ComputeSelected);
		AddReflector(Reflect);
		_childRT = GetComponentInChildren<Image>().rectTransform;
	}
	private void LateUpdate()
	{
		UpdateToTooltipPosition(_tooltipManager.CurrentTooltip.Val);
	}

	private bool ComputeSelected()
	{
		return _tooltipManager.CurrentTooltip.Val != null;
	}

	void Reflect()
	{
		var currentTooltip = _tooltipManager.CurrentTooltip.Val;
		if (currentTooltip == null) return;
		_text.text = currentTooltip.Text;
		LayoutRebuilder.ForceRebuildLayoutImmediate(_childRT); // Ensure the size is updated with the new text
		UpdateToTooltipPosition(currentTooltip);
	}

	Vector2 _lastPos = Vector2.zero;
	void UpdateToTooltipPosition(ITooltip tooltip)
	{
		if (tooltip == null) return;

		var newPos = tooltip.Position;
		if (Vector2.Distance(newPos, _lastPos) < 0.1f) return; // Avoid unnecessary updates if the position hasn't changed significantly
		_lastPos = newPos;

		this.transform.position = PositionTooltip(_childRT.sizeDelta, tooltip);

	}

	static Vector2 PositionTooltip(Vector2 tooltipSize, ITooltip target)
	{
		var centerToCenter = (tooltipSize + target.SizeDelta) / 2f;
		Vector2[] candidateOffsets = new Vector2[]
		{
			new Vector2(0, centerToCenter.y),    // Above
			new Vector2(centerToCenter.x, 0),    // Right
			new Vector2(0, -centerToCenter.y),   // Below
			new Vector2(-centerToCenter.x, 0)    // Left
		};
		foreach (var offset in candidateOffsets)
		{
			var candidatePosition = (Vector2)target.Position + offset;
			var tooltipRect = new Rect(candidatePosition - tooltipSize / 2f, tooltipSize);
			if (FitsOnScreen(tooltipRect))
			{
				return candidatePosition;
			}
		}

		// While this can't happen for UI tooltips, it can for world tooltips
		//Debug.LogWarning("No position available for tooltip, defaulting to above."); 
		return candidateOffsets[0];
	}

	static bool FitsOnScreen(Rect rect)
	{
		var screenRect = new Rect(0, 0, Screen.width, Screen.height);
		return rect.xMin >= screenRect.xMin && rect.xMax <= screenRect.xMax &&
			   rect.yMin >= screenRect.yMin && rect.yMax <= screenRect.yMax;
	}
}
