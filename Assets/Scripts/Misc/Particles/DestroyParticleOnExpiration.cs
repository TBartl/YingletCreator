using UnityEngine;

public class DestroyParticleOnExpiration : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		Destroy(this.gameObject, this.GetComponentInChildren<ParticleSystem>().main.duration);
	}
}