using Reactivity;
using UnityEngine;

public interface IMainMenuState
{
	public bool OnMainMenu { get; }

	public void EnterGame();
}

public class MainMenuState : MonoBehaviour, IMainMenuState
{
	Observable<bool> _onMainMenu = new Observable<bool>(true);

	public bool OnMainMenu => _onMainMenu.Val;

	public void EnterGame()
	{
		_onMainMenu.Val = false;
	}
}
