using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// When content is added or removed from a scroll view, this class will update the scroll view's content size and position accordingly.
/// </summary>
public class ScrollContentUpdater
{
	private ScrollRect _scrollView;

	public ScrollContentUpdater(Transform parentTransform)
	{
		_scrollView = parentTransform.GetComponentInParentSafe<ScrollRect>();
	}

	public void ApplyAndRestoreScrollPosition(Action action)
	{
		var originalPos = _scrollView.normalizedPosition.y; // For some reason, doing this on both axis causes the x to shift
		action();
		//LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollView.content); I couldn't get this to work unfortunately, so I'm using a pretty heavy handed solution instead
		Canvas.ForceUpdateCanvases();

		_scrollView.normalizedPosition = new Vector2(_scrollView.normalizedPosition.x, originalPos);
	}
}
