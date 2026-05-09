using Reactivity;
using UnityEngine;

public class ChangeActiveExpeditionCharacterOnTab : ReactiveBehaviour
{
	private IExpeditionCharacterManager _expeditionCharacterManager;

	private void Start()
	{
		_expeditionCharacterManager = this.GetComponentInParentSafe<IExpeditionCharacterManager>();
	}
	private void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Tab)) return;

		_expeditionCharacterManager.TryTabToNextCharacter();
	}
}
