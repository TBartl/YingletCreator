using Reactivity;
using UnityEngine.UI;

public class SwitchToPartyHUDMemberOnClick : ReactiveBehaviour
{
	private IActiveCharacterProvider _activeCharacterProvider;
	private Computed<IExpeditionCharacterManager> _expeditionCharacterManager;
	private IPartyMemberHUDReference _hudReference;
	private Button _button;

	private void Start()
	{
		_expeditionCharacterManager = this.CreateExpeditionComputed<IExpeditionCharacterManager>(Singletons.GetSingleton<IExpeditionManager>());
		_hudReference = this.GetComponentInParentSafe<IPartyMemberHUDReference>();

		_button = this.GetComponentInParentSafe<Button>();
		_button.onClick.AddListener(Button_OnClick);
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		_button?.onClick.RemoveListener(Button_OnClick);
	}

	private void Button_OnClick()
	{
		_expeditionCharacterManager.Val?.SetActiveCharacter(_hudReference.CharacterGameObject);
	}
}
