using UnityEngine;

public interface IRoomMeshVisibilityTransitioner
{
	void ForceTo(bool active);
	void TransitionTo(bool active);
}

internal class RoomMeshVisibilityTransitioner : MonoBehaviour, IRoomMeshVisibilityTransitioner, IInitializable
{
	private RoomMeshVisibilityConstants _constants;
	private float _currentYCutoff;
	private Coroutine _transitionCoroutine;
	private MeshRenderer _mr;
	private Material _originalMaterial;
	private Material _cutOffMaterial;

	public void Initialize()
	{
		_constants = Singletons.GetSingleton<RoomMeshVisibilityConstants>();


		_mr = this.GetComponentSafe<MeshRenderer>();
		_originalMaterial = _mr.sharedMaterial;
		_cutOffMaterial = new Material(_originalMaterial);
		_cutOffMaterial.shader = _constants.CUTOFF_FROM_TOP_SHADER;
	}

	float MaxYCutoff => _constants.PassageRange.y;
	float MinYCutoff => _constants.PassageRange.x;

	public void ForceTo(bool active)
	{
		_currentYCutoff = active ? MaxYCutoff : MinYCutoff;
		if (_transitionCoroutine != null) StopCoroutine(_transitionCoroutine);
		this.gameObject.SetActive(active);
	}

	public void TransitionTo(bool active)
	{
		var fromY = _currentYCutoff;
		var toY = active ? MaxYCutoff : MinYCutoff;
		this.gameObject.SetActive(true);
		_mr.sharedMaterial = _cutOffMaterial;
		this.StartEaseCoroutine(ref _transitionCoroutine, _constants.EaseSettings, Apply, OnComplete);


		void Apply(float p)
		{
			_currentYCutoff = Mathf.Lerp(fromY, toY, p);
			_cutOffMaterial.SetFloat(_constants.Y_CUTOFF_PROPERTY_ID, _currentYCutoff);
		}
		void OnComplete()
		{
			_mr.sharedMaterial = _originalMaterial;
			this.gameObject.SetActive(active);
		}
	}
}
