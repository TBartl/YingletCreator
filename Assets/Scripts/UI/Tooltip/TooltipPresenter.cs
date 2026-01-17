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
		this.transform.position = PositionTooltip(_childRT.sizeDelta, currentTooltip);
	}

	static Vector2 PositionTooltip(Vector2 tooltipSize, ITooltip target)
	{
		var targetCenter = target.RectTransform;
		var centerToCenter = (tooltipSize + targetCenter.sizeDelta) / 2f;
		Vector2[] candidateOffsets = new Vector2[]
		{
			new Vector2(0, centerToCenter.y),    // Above
			new Vector2(centerToCenter.x, 0),    // Right
			new Vector2(0, -centerToCenter.y),   // Below
			new Vector2(-centerToCenter.x, 0)    // Left
		};
		foreach (var offset in candidateOffsets)
		{
			var candidatePosition = (Vector2)targetCenter.position + offset;
			var tooltipRect = new Rect(candidatePosition, tooltipSize);
			if (FitsOnScreen(tooltipRect))
			{
				return candidatePosition;
			}
		}

		Debug.LogWarning("No position available for tooltip, defaulting to above."); // Should never happen unless the tooltip is massive
		return candidateOffsets[0];
	}

	static bool FitsOnScreen(Rect rect)
	{
		var screenRect = new Rect(0, 0, Screen.width, Screen.height);
		return screenRect.Overlaps(rect);
	}
}
