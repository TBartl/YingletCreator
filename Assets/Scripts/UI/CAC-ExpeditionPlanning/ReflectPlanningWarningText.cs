using Reactivity;
using TMPro;
using UnityEngine;

public class ReflectPlanningWarningText : ReactiveBehaviour
{
	[SerializeField] Color _errorColor;
	[SerializeField] Color _warningColor;
	[SerializeField] Color _infoColor;

	private IExpeditionPlanningManager _planning;
	private IExpeditionManager _expeditionManager;
	private TMP_Text _text;

	void Start()
	{
		_planning = Singletons.GetSingleton<IExpeditionPlanningManager>();
		_expeditionManager = Singletons.GetSingleton<IExpeditionManager>();
		_text = this.GetComponentInChildren<TMP_Text>();
		AddReflector(ReflectText);
	}

	private void ReflectText()
	{
		var partyCount = _planning.CurrentParty.Count;
		if (_expeditionManager.State.Val == ExpeditionState.Running)
		{
			_text.text = "<sprite tint=\"1\" name=\"Warning\"> Expedition is already running";
			_text.color = _errorColor;
			this.gameObject.SetActive(true);
		}
		else if (_expeditionManager.State.Val == ExpeditionState.Starting)
		{
			_text.text = "Expedition is starting...";
			_text.color = _infoColor;
			this.gameObject.SetActive(true);
		}
		else if (partyCount == ExpeditionPlanningManager.MAX_CHARACTERS)
		{
			this.gameObject.SetActive(false);
		}
		else if (partyCount == 0)
		{

			_text.text = "<sprite tint=\"1\" name=\"Warning\"> Must have at least 1 character";
			_text.color = _errorColor;
			this.gameObject.SetActive(true);
		}
		else
		{
			_text.text = $"<sprite tint=\"1\" name=\"Warning\"> {ExpeditionPlanningManager.MAX_CHARACTERS} characters recommended";
			_text.color = _warningColor;
			this.gameObject.SetActive(true);
		}
	}
}
