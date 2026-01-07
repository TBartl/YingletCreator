using Reactivity;
using System.Collections;
using UnityEngine;

public interface ITooltipManager
{
	void Register(ITooltip tooltip);
	void Unregister(ITooltip tooltip);
	IReadOnlyObservable<ITooltip> CurrentTooltip { get; }
	void NotifyTextChanged(ITooltip tooltip);
}
public class TooltipManager : MonoBehaviour, ITooltipManager
{
	Observable<ITooltip> _currentTooltip = new Observable<ITooltip>(null);
	Coroutine _coroutine;
	ITooltip _nextTooltip; // The tooltip that is pending to be shown

	public IReadOnlyObservable<ITooltip> CurrentTooltip => _currentTooltip;

	public void Register(ITooltip tooltip)
	{
		this.StopAndStartCoroutine(ref _coroutine, DelayAndMakeTooltip(tooltip));
	}

	public void Unregister(ITooltip tooltip)
	{
		if (_coroutine != null && _nextTooltip == tooltip)
		{
			StopCoroutine(_coroutine);
			_coroutine = null;
			_nextTooltip = null;
		}

		if (_currentTooltip.Val != tooltip) return;
		_currentTooltip.Val = null;
	}

	public void NotifyTextChanged(ITooltip tooltip)
	{
		if (_currentTooltip.Val == tooltip)
		{
			// Force-refresh the text by re-assigning null and then the same tooltip.
			_currentTooltip.Val = null;
			_currentTooltip.Val = tooltip;
		}
	}

	IEnumerator DelayAndMakeTooltip(ITooltip tooltip)
	{
		_nextTooltip = tooltip;
		yield return new WaitForSeconds(0.3f); // Small delay
		_currentTooltip.Val = tooltip;
		_coroutine = null;
		_nextTooltip = null;
	}
}
