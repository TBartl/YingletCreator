using UnityEngine;

public class CreateModChildrenUI : MonoBehaviour
{
	[SerializeField] GameObject _prefab;
	void Start()
	{
		var modLoader = Singletons.GetSingleton<IModLoader>();
		foreach (var mod in modLoader.AllMods)
		{
			var go = GameObject.Instantiate(_prefab, this.transform);
			go.GetComponent<IndividualModUI>().Setup(mod);
		}
	}
}
