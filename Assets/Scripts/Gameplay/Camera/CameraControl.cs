using Reactivity;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A provider might be something like:
///  - Following the player
///  - Fixed position showing the character in the character creator
///  - Showing an event
/// </summary>
public interface ICameraControlProvider
{
	bool WantsControl { get; }
	(Vector3, Quaternion) CalculateTransform();
}

public interface ICameraControl
{
	ICameraControlProvider CurrentProvider { get; }
}

public class CameraControl : ReactiveBehaviour, ICameraControl
{
	[SerializeField] SharedEaseSettings _swapBetweenProvidersEase;


	IEnumerable<ICameraControlProvider> _providers;

	Computed<ICameraControlProvider> _bestProvider;
	private ICameraControlProvider _lastProvider;
	private Coroutine _easeCoroutine;
	private float _providerInfluence;

	public ICameraControlProvider CurrentProvider => _bestProvider.Val;

	private void Start()
	{
		_providers = this.GetComponentsInChildrenSafe<ICameraControlProvider>();
		_bestProvider = CreateComputed(ComputeBestProvider);
		_bestProvider.OnChanged += OnBestProviderChanged;
	}

	private void OnBestProviderChanged(ICameraControlProvider from, ICameraControlProvider to)
	{
		_lastProvider = from;
		this.StartEaseCoroutine(ref _easeCoroutine, _swapBetweenProvidersEase, p => _providerInfluence = p, () => { _lastProvider = null; });
	}

	private ICameraControlProvider ComputeBestProvider()
	{
		foreach (var provider in _providers)
		{
			if (provider.WantsControl)
			{
				return provider;
			}
		}
		return null;
	}

	void LateUpdate() // Late so we can act only after other things have moved and settled
	{
		var providerTransform = _bestProvider.Val?.CalculateTransform() ?? (this.transform.position, this.transform.rotation);

		if (_providerInfluence < .999f)
		{
			// We're still transitioning to this camera provider, blend
			var lastTransform = _lastProvider?.CalculateTransform() ?? (this.transform.position, this.transform.rotation);
			this.transform.position = Vector3.LerpUnclamped(lastTransform.Item1, providerTransform.Item1, _providerInfluence);
			this.transform.rotation = Quaternion.SlerpUnclamped(lastTransform.Item2, providerTransform.Item2, _providerInfluence);
		}
		else
		{
			// We have full control, just use directly
			this.transform.position = providerTransform.Item1;
			this.transform.rotation = providerTransform.Item2;
		}
	}
}
