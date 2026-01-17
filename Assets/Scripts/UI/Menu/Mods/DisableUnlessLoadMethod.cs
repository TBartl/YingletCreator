using UnityEngine;

public class DisableUnlessLoadMethod : MonoBehaviour
{
	[SerializeField] CompositeResourceLoadMethod _activeLoadMethod;

	private void Start()
	{
		var loadProvider = Singletons.GetSingleton<IResourceLoadMethodProvider>();
		this.gameObject.SetActive(_activeLoadMethod == loadProvider.LoadMethod);
	}
}
