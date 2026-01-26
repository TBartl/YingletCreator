using Character.Creator.UI;
using UnityEngine;

/// <summary>
/// Provides the free fall juice for clipboard elements
/// In practice, this is the page and (possibly) the fake bookmark
/// </summary>
public interface IClipboardFreeFallManager
{
	/// <summary> 
	/// Plays the free fall animation on the given transform
	/// </summary>
	void FreeFall(Transform transform);
}

public class ClipboardFreeFallManager : MonoBehaviour, IClipboardFreeFallManager
{
	[SerializeField] GameObject _freeFallParent;

	private IClipboardOrdering _clipboardOrdering;

	private void Awake()
	{
		_clipboardOrdering = this.GetComponent<IClipboardOrdering>();
	}

	public void FreeFall(Transform transform)
	{
		var freeFallParentInstance = Instantiate(_freeFallParent, this.transform.position, this.transform.rotation);
		_clipboardOrdering.SendToLayer(freeFallParentInstance.transform, ClipboardLayer.Freefall);
		transform.SetParent(freeFallParentInstance.transform, true);
	}
}
