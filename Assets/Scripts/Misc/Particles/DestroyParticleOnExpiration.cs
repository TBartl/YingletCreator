using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleOnExpiration : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, this.GetComponent<ParticleSystem>().main.duration);
    }
}