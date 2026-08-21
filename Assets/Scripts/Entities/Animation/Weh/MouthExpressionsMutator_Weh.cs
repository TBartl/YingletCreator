using UnityEngine;

public class MouthExpressionsMutator_Weh : MonoBehaviour, IMouthExpressionsMutator, IInitializable
{
	[SerializeField] MouthExpressionSequence _sequence;

	private IWehManager _wehManager;
	private IMouthExpressionSequencePlayer _sequencePlayer;

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

	public void Mutate(ref MouthExpression expression, ref MouthOpenAmount openAmount)
	{
		if (!_sequencePlayer.IsActive) return;

		expression = _sequencePlayer.Expression;
		openAmount = _sequencePlayer.OpenAmount;
	}
}