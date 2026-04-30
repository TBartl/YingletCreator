using Reactivity;
using System;
using System.Linq;
using UnityEngine;

public class ChangeActiveExpeditionCharacterOnTab : ReactiveBehaviour
{
	private INetStateReader _netState;
	private IExpeditionCharacterManager _expeditionCharacterManager;

	private void Start()
	{
		_netState = Singletons.GetSingleton<INetStateReader>();
		_expeditionCharacterManager = this.GetComponentInParentSafe<IExpeditionCharacterManager>();
	}
	private void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Tab)) return;
		var myId = _netState.LocalClientID;
		var possibleCharacters = _expeditionCharacterManager.Characters.ToArray();
		var currentCharacter = _expeditionCharacterManager.ActiveCharacter.Val;

		int currentCharacterIndex = Array.IndexOf(possibleCharacters, currentCharacter);
		for (int i = 1; i <= possibleCharacters.Length; i++)
		{
			var nextCharacter = possibleCharacters[(currentCharacterIndex + i) % possibleCharacters.Length];
			if (nextCharacter.ClientId == myId)
			{
				_expeditionCharacterManager.SetActiveCharacter(nextCharacter.Root);
				break;
			}
		}
	}
}
