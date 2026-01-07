using UnityEngine;
using UnityEngine.EventSystems;

public interface ITooltip
{
	string Text { get; }
	RectTransform RectTransform { get; }
}


public abstract class Tooltip : MonoBehaviour, ITooltip, IPointerEnterHandler, IPointerExitHandler
{
	private ITooltipManager _tooltipManager;

	public abstract string Text { get; }

	public RectTransform RectTransform { get; private set; }

	protected virtual void Awake()
	{
		RectTransform = GetComponent<RectTransform>();
		_tooltipManager = Singletons.GetSingleton<ITooltipManager>();
	}

	private void OnDestroy()
	{
		_tooltipManager.Unregister(this);
	}

	private void OnDisable()
	{
		_tooltipManager.Unregister(this);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		_tooltipManager.Register(this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_tooltipManager.Unregister(this);
	}
}
