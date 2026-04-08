using Reactivity;
using System.Collections;
using UnityEngine;

public interface ITooltipProvider_UI : ITooltipProvider
{
	void Register(ITooltip tooltip);
	void Unregister(ITooltip tooltip);
}
public class TooltipProvider_UI : MonoBehaviour, ITooltipProvider_UI
{
	Observable<ITooltip> _desiredTooltip = new Observable<ITooltip>(null);
	Coroutine _coroutine;
	ITooltip _nextTooltip; // The tooltip that is pending to be shown

	public IReadOnlyObservable<ITooltip> DesiredTooltip => _desiredTooltip;

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

		if (_desiredTooltip.Val != tooltip) return;
		_desiredTooltip.Val = null;
	}

	public void NotifyTextChanged(ITooltip tooltip)
	{
		if (_desiredTooltip.Val == tooltip)
		{
			// Force-refresh the text by re-assigning null and then the same tooltip.
			_desiredTooltip.Val = null;
			_desiredTooltip.Val = tooltip;
		}
	}

	IEnumerator DelayAndMakeTooltip(ITooltip tooltip)
	{
		_nextTooltip = tooltip;
		yield return new WaitForSeconds(0.3f); // Small delay
		_desiredTooltip.Val = tooltip;
		_coroutine = null;
		_nextTooltip = null;
	}
}
