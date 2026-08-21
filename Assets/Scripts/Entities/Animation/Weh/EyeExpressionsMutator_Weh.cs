using UnityEngine;

public class EyeExpressionsMutator_Weh : MonoBehaviour, ICurrentEyeExpressionMutator, IInitializable
{
	[SerializeField] EyeExpressionSequence _sequence;

	private IWehManager _wehManager;
	private IEyeExpressionSequencePlayer _sequencePlayer;

	public void Initialize()
	{
		_wehManager = this.GetComponentInParent<IWehManager>();
		_wehManager.OnWeh += OnWehed;
		_sequencePlayer = _sequence.CreatePlayer();
	}

	void OnDestroy()
	{
		_wehManager.OnWeh -= OnWehed;
	}

	private void OnEnable()
	{
		_sequencePlayer?.Stop();
	}

	private void OnWehed()
	{
		if (!this.isActiveAndEnabled) return;
		_sequencePlayer.Play();
	}

	public EyeExpression Mutate(EyeExpression input)
	{
		if (!_sequencePlayer.IsActive) return input;
		return _sequencePlayer.Expression;
	}
}