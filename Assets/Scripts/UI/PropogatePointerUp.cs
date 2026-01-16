using UnityEngine;
using UnityEngine.EventSystems;

public class PropogatePointerUp : MonoBehaviour, IPointerUpHandler
{
	private IPointerUpHandler _target;

	private void Awake()
	{
		_target = this.transform.parent.GetComponentInParent<IPointerUpHandler>();
	}
	public void OnPointerUp(PointerEventData eventData)
	{
		_target.OnPointerUp(eventData);
	}
}
