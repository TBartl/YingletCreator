using Encounters.Runtime;
using Reactivity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IPromptChoiceUI
{
	void SetChoice(IEncounterInstance encounter, ChoiceBlockNode choice, int choiceIndex);
}
public class PromptChoiceUI : ReactiveBehaviour, IPromptChoiceUI, IUIInteractable
{
	[SerializeField] Sprite _normalState;
	[SerializeField] Sprite _hoveredState;

	Image _backgroundImage;
	TMP_Text _text;
	private IHoverable _hoverable;
	private Button _button;
	private IEncounterNodeReferenceUI _reference;
	private IEncounterInstance _encounter;
	private ChoiceBlockNode _choice;
	private int _choiceIndex;
	private ICommonGameplayAssets _assets;
	private Observable<bool> _canAfford = new Observable<bool>(); // We don't want to actually keep this reflective, but the interface demands it.

	public IReadOnlyObservable<bool> Interactable => _canAfford;
	Computed<bool> _showAsSelected;

	public void SetChoice(IEncounterInstance encounter, ChoiceBlockNode choice, int choiceIndex)
	{
		_encounter = encounter;
		_choice = choice;
		_choiceIndex = choiceIndex;
		_assets = Singletons.GetSingleton<ICommonGameplayAssets>();
		_backgroundImage = this.GetComponentSafe<Image>();
		_text = this.GetComponentInChildrenSafe<TMP_Text>();
		_hoverable = this.GetComponentSafe<IHoverable>();
		_button = this.GetComponentSafe<Button>();
		_reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);

		SetCanAfford();
		_text.text = GetText();

		_button.onClick.AddListener(OnClicked);
		_showAsSelected = CreateComputed(ComputeShowAsSelected);
		AddReflector(ReflectHovering);
	}

	void SetCanAfford()
	{
		var currentEnergy = _encounter.Character.GetComponentInChildrenSafe<ICharacterResources>().GetResource(_assets.ResourceEnergy);
		_canAfford.Val = currentEnergy >= _choice.EnergyCost;
	}

	private string GetText()
	{

		var sb = new System.Text.StringBuilder();
		if (_choice.EnergyCost > 0)
		{
			if (!_canAfford.Val)
			{
				sb.Append($"<color={TMPUtils.TooltipRed}>");
			}

			sb.Append($"[");
			for (int i = 0; i < _choice.EnergyCost; i++)
			{
				sb.Append(TMPUtils.EnergySprite);
			}
			sb.Append("] ");
			if (!_canAfford.Val)
			{
				sb.Append($"</color>");
			}
		}

		if (_choice.Next is RollNode rollNode)
		{
			// Next node we're rolling, so let's display that in this text
			sb.Append($"[{TMPUtils.DiceSprite}{rollNode.RollType.ToString().ToUpper()}] ");
		}

		sb.Append(_choice.Text);
		return sb.ToString();
	}


	private new void OnDestroy()
	{
		base.OnDestroy();
		if (_button != null)
		{
			_button.onClick.RemoveListener(OnClicked);
		}
	}

	private void OnClicked()
	{
		_encounter.Networking.SendMessage_SelectChoice(_choiceIndex);
	}

	private bool ComputeShowAsSelected()
	{
		bool hovering = _hoverable.Hovered.Val;
		if (hovering)
		{
			return true;
		}
		int indexInHistory = _reference.IndexInHistory;
		var history = _reference.EncounterInstance.NodeHistory;
		if (indexInHistory + 2 < history.Count)
		{
			// In this conditional, we know we're not the latest node
			var selectedBlockNode = history[indexInHistory + 1];
			if (selectedBlockNode == _choice)
			{
				return true;
			}
		}

		return false;
	}

	private void ReflectHovering()
	{
		bool showAsSelected = _showAsSelected.Val;
		_backgroundImage.sprite = showAsSelected ? _hoveredState : _normalState;
	}
}
