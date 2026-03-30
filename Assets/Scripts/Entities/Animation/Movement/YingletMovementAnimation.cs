using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class YingletMovementAnimation : MonoBehaviour
{
	[SerializeField] float SPEED_THRESHOLD = 0.1f;
	static string[] IDLE_LAYER_NAMES = new string[] { "TailWagging", "LookAround", "EarWiggle" };

	private Rigidbody _rigidBody;
	private Animator _animator;
	private IEnumerable<YingLayer> _layers;



	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_rigidBody = this.GetComponentInParent<Rigidbody>();
		_animator = this.GetComponent<Animator>();
		_layers = IDLE_LAYER_NAMES.Select(layerName => new YingLayer(layerName, _animator)).ToArray();
	}

	// Update is called once per frame
	void LateUpdate()
	{
		float speed = _rigidBody.linearVelocity.magnitude;
		bool moving = speed > SPEED_THRESHOLD;

		foreach (var layer in _layers)
		{
			_animator.SetLayerWeight(layer.LayerIndex, moving ? 0 : layer.OriginalWeight);
		}
	}


	class YingLayer
	{
		public YingLayer(string name, Animator animator)
		{
			LayerIndex = animator.GetLayerIndex(name);
			OriginalWeight = animator.GetLayerWeight(LayerIndex);
		}
		public int LayerIndex { get; }
		public float OriginalWeight { get; }
	}
}
