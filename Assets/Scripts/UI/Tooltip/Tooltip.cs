using UnityEngine;
using UnityEngine.EventSystems;

public interface ITooltip
{
	string Text { get; }
	Vector2 Position { get; }
	Vector2 SizeDelta { get; }
}


public abstract class Tooltip : MonoBehaviour, ITooltip, IPointerEnterHandler, IPointerExitHandler
{
	private ITooltipProvider_UI _tooltipManager;
	private RectTransform _rectTransform;

	public abstract string Text { get; }

	public Vector2 Position => _rectTransform.position;
	public Vector2 SizeDelta => _rectTransform.sizeDelta;

	protected virtual void Awake()
	{
		_tooltipManager = Singletons.GetSingleton<ITooltipProvider_UI>();
		_rectTransform = GetComponent<RectTransform>();
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
