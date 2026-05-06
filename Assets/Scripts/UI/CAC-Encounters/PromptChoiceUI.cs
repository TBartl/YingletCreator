using Encounters.Runtime;
using Reactivity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IPromptChoiceUI
{
	void SetChoice(IEncounterInstance encounter, ChoiceBlockNode choice);
}
public class PromptChoiceUI : ReactiveBehaviour, IPromptChoiceUI, IUIInteractable
{
	[SerializeField] Sprite _normalState;
	[SerializeField] Sprite _hoveredState;

	Image _backgroundImage;
	TMP_Text _text;
	private IHoverable _hoverable;
	private Button _button;
	private IEncounterInstance _encounter;
	private ChoiceBlockNode _choice;
	private Observable<bool> _canAfford = new Observable<bool>(); // We don't want to actually keep this reflective, but the interface demands it.

	public IReadOnlyObservable<bool> Interactable => _canAfford;

	public void SetChoice(IEncounterInstance encounter, ChoiceBlockNode choice)
	{
		_encounter = encounter;
		_choice = choice;
		_backgroundImage = this.GetComponentSafe<Image>();
		_text = this.GetComponentInChildrenSafe<TMP_Text>();
		_hoverable = this.GetComponentSafe<IHoverable>();
		_button = this.GetComponentSafe<Button>();

		_text.text = GetText();

		_button.onClick.AddListener(OnClicked);
		SetCanAfford();
		AddReflector(ReflectHovering);
	}

	void SetCanAfford()
	{
		var currentEnergy = _encounter.Character.GetComponentInChildrenSafe<ICharacterResources>().GetResource(CharacterResourceType.Energy);
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
		_choice.Run(_encounter);
	}

	private void ReflectHovering()
	{
		bool hovering = _hoverable.Hovered.Val;
		_backgroundImage.sprite = hovering ? _hoveredState : _normalState;
	}
}
