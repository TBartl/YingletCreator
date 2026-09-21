using Encounters;
using Encounters.Runtime;
using UnityEngine;

public class PlaySoundOnEnterRollUIState : MonoBehaviour
{
	[SerializeField] private SoundEffect _rollDice;
	[SerializeField] private SoundEffect _resultSuccess;
	[SerializeField] private SoundEffect _resultFailure;
	[SerializeField] private SoundEffect _resultNeutral;
	[SerializeField] private SoundEffect _moveOn;
	private RollNodeVisitData _data;
	private IAudioPlayer _audioPlayer;

	void Start()
	{
		_audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		_data = reference.Record.VisitData as RollNodeVisitData;

		_data.StateObservable.OnChanged += OnRollStateChanged;
	}

	private void OnRollStateChanged(RollState from, RollState to)
	{
		if (to == RollState.Rolling)
		{
			_audioPlayer.Play(_rollDice);
		}
		else if (to == RollState.ShowingResult)
		{
			var classification = _data.RealBranch.Classification;
			if (classification == RollClassification.CriticalSuccess || classification == RollClassification.Success)
			{
				_audioPlayer.Play(_resultSuccess);
			}
			else if (classification == RollClassification.CriticalFailure || classification == RollClassification.Failure)
			{
				_audioPlayer.Play(_resultFailure);
			}
			else
			{
				_audioPlayer.Play(_resultNeutral);
			}
		}
		else if (to == RollState.Finished)
		{
			_audioPlayer.Play(_moveOn);
		}

	}
}
