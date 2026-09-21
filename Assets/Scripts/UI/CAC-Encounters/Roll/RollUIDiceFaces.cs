using Encounters.Runtime;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RollUIDiceFaces : MonoBehaviour
{
	[SerializeField] Sprite[] _diceFaceSprites;
	[SerializeField] Image[] _dice;
	[SerializeField] float _rollDuration = .7f;
	[SerializeField] float _faceChangeInterval = .1f;
	private RollNodeVisitData _data;

	private void Start()
	{
		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		_data = reference.Record.VisitData as RollNodeVisitData;
		_data.StateObservable.OnChanged += OnStateChanged;

		// We've already rolled
		if (_data.State == RollState.ShowingResult || _data.State == RollState.Finished)
		{
			SetFinalDiceFace();
		}
	}

	void SetFinalDiceFace()
	{
		for (int i = 0; i < _dice.Length; i++)
		{
			_dice[i].sprite = _diceFaceSprites[_data.DiceRolls[i] - 1];
		}
	}

	private void OnStateChanged(RollState from, RollState to)
	{
		if (to == RollState.Rolling)
		{
			StartCoroutine(RollDice());
		}
		else if (to == RollState.ShowingResult || to == RollState.Finished)
		{
			SetFinalDiceFace();
		}
	}

	IEnumerator RollDice()
	{
		int numFramesToShow = Mathf.RoundToInt(_rollDuration / _faceChangeInterval);

		int[][] facesToShow = _data.DiceRolls
			.Select(finalFaceValue => GetFacesToShowForDie(numFramesToShow, (int)finalFaceValue))
			.ToArray();

		for (int t = 0; t < numFramesToShow; t++)
		{
			for (int i = 0; i < _dice.Length; i++)
			{
				int faceIndex = facesToShow[i][t];
				_dice[i].sprite = _diceFaceSprites[faceIndex - 1];
			}
			yield return new WaitForSeconds(_faceChangeInterval);
		}
	}

	int[] GetFacesToShowForDie(int numFramesToShow, int finalFaceValue)
	{
		int[] faces = new int[numFramesToShow];
		// Fill the array backwards, ensuring no consecutive repeats and the last face is the final face value
		faces[numFramesToShow - 1] = finalFaceValue;
		for (int i = numFramesToShow - 2; i >= 0; i--)
		{
			int face;

			do
			{
				face = Random.Range(1, 7);
			}
			while (face == faces[i + 1]);

			faces[i] = face;
		}

		return faces;
	}

	private void OnDestroy()
	{
		if (_data != null)
		{

			_data.StateObservable.OnChanged -= OnStateChanged;
		}
	}
}
