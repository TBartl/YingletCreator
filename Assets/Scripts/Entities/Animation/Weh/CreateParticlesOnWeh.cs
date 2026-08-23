using UnityEngine;

public class CreateParticlesOnWeh : MonoBehaviour
{
	[SerializeField] GameObject _particlePrefab;
	private IWehManager _wehManager;

	void Start()
	{
		_wehManager = this.GetCharacterRootComponent<IWehManager>();
		_wehManager.OnWeh += OnWeh;
	}
	private void OnDestroy()
	{
		_wehManager.OnWeh -= OnWeh;
	}

	private void OnWeh()
	{
		var go = Instantiate(_particlePrefab, transform.position, this.transform.rotation, transform);
	}
}
