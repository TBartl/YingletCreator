using Encounters.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IRollUIOutcome
{
	void SetResult(RollBlockNode branch);
}

public class RollUIOutcome : MonoBehaviour, IRollUIOutcome
{
	[SerializeField] TMP_Text _numberText;
	[SerializeField] TMP_Text _descriptionText;
	[SerializeField] Image _background;

	[SerializeField] RollUISettings _settings;
	[SerializeField] SharedEaseSettings _easeSettings;

	private RollNodeVisitData _data;
	private RollClassificationColors _classificationSettings;
	private RollBlockNode _branch;
	protected Coroutine _transitionCoroutine;

	public void SetResult(RollBlockNode branch)
	{
		_branch = branch;

		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		var node = reference.Record.Node as RollNode;
		_data = reference.Record.VisitData as RollNodeVisitData;

		_classificationSettings = _settings.RollClassificationColorMap[branch.Classification];
		int minValue = CalculateMinValue(node, branch);

		_numberText.text = GetNumberText(minValue, branch.MaxValueInclusive);
		_descriptionText.text = _classificationSettings.Text;

		bool alreadyOnFinalStates = _data.State == RollState.ShowingResult || _data.State == RollState.Finished;
		if (alreadyOnFinalStates && _data.RealBranch == _branch)
		{
			_numberText.color = Color.white;
			_descriptionText.color = Color.white;
			_background.color = _classificationSettings.JuicyColor;
		}
		else
		{
			_numberText.color = _classificationSettings.TextColor;
			_descriptionText.color = _classificationSettings.TextColor;
			_background.color = _classificationSettings.BackgroundColor;
		}

		_data.StateObservable.OnChanged += OnRollStateChanged;
	}

	private void OnDestroy()
	{
		if (_data != null)
		{
			_data.StateObservable.OnChanged -= OnRollStateChanged;
		}
	}
	private void OnRollStateChanged(RollState from, RollState to)
	{
		if (to != RollState.ShowingResult) return;
		if (_data.RealBranch != _branch) return;

		Color fromTextColor = _classificationSettings.TextColor;
		Color toTextColor = Color.white;

		Color fromBackgroundColor = _classificationSettings.BackgroundColor;
		Color toBackgroundColor = _classificationSettings.JuicyColor;

		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, UpdateColors);

		void UpdateColors(float p)
		{
			Color textColor = Color.LerpUnclamped(fromTextColor, toTextColor, p);
			Color backgroundColor = Color.LerpUnclamped(fromBackgroundColor, toBackgroundColor, p);
			_numberText.color = textColor;
			_descriptionText.color = textColor;
			_background.color = backgroundColor;
		}
	}

	private int CalculateMinValue(RollNode node, RollBlockNode myBranch)
	{
		int minValue = 0;
		var branches = node.Branches;
		foreach (var branch in node.Branches)
		{
			if (branch == myBranch) break;
			minValue = branch.MaxValueInclusive + 1;
		}
		return minValue;
	}

	private string GetNumberText(int minValue, int maxValue)
	{
		if (minValue == maxValue)
		{
			return minValue.ToString();
		}
		else if (maxValue >= RollProvider.MaxRollValue)
		{
			return $"{minValue}+";
		}
		return $"{minValue}-{maxValue}";
	}
}
