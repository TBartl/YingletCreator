using UnityEngine;

public class EyeExpressionsMutator_Booped : MonoBehaviour, ICurrentEyeExpressionMutator, IInitializable
{
	[SerializeField] EyeExpressionSequence _sequence;

	private IBoopManager _boopManager;
	private IEyeExpressionSequencePlayer _sequencePlayer;

	public void Initialize()
	{
		_boopManager = this.GetComponentInParent<IBoopManager>();
		_boopManager.OnBoop += OnBooped;
		_sequencePlayer = _sequence.CreatePlayer();
	}

	void OnDestroy()
	{
		_boopManager.OnBoop -= OnBooped;
	}
	private void OnEnable()
	{
		_sequencePlayer?.Stop();
	}

	private void OnBooped()
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
