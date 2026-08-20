using UnityEngine;

public class MouthExpressionsMutator_Booped : MonoBehaviour, IMouthExpressionsMutator, IInitializable
{
	[SerializeField] MouthExpressionSequence _sequence;

	private IBoopManager _boopManager;
	private IMouthExpressionSequencePlayer _sequencePlayer;

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

	public void Mutate(ref MouthExpression expression, ref MouthOpenAmount openAmount)
	{
		if (!_sequencePlayer.IsActive) return;

		expression = _sequencePlayer.Expression;
		openAmount = _sequencePlayer.OpenAmount;
	}
}
