using UnityEngine;

public class CreateParticlesOnTolled : MonoBehaviour
{
	[SerializeField] private GameObject _particlePrefab1;
	[SerializeField] private GameObject _particlePrefab2;
	private ITollEnergyOnEnterRoom _toll;

	private void Awake()
	{
		_toll = this.GetComponentInParentSafe<ITollEnergyOnEnterRoom>();
		_toll.OnEnergyTollApplied += OnTolled;
	}

	private void OnDestroy()
	{
		_toll.OnEnergyTollApplied -= OnTolled;
	}

	void OnTolled(int cost)
	{
		var prefab = cost switch
		{
			1 => _particlePrefab1,
			2 => _particlePrefab2,
			_ => null
		};
		if (prefab == null) return;
		var go = Instantiate(prefab, transform.position, Quaternion.identity);
		go.transform.parent = transform;
		var particles = go.GetComponentInChildren<ParticleSystem>();
		Destroy(go, particles.main.duration);
	}
}
