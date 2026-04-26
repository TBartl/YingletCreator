using Character.Creator;
using System.Linq;
using UnityEngine;

public class DebugQuickExpedition : MonoBehaviour
{
	private IExpeditionPlanningManager _expeditionPlanner;
	private IExpeditionManager _expeditionManager;
	private ILocalYingletRepository _yingletRepository;

	private void Start()
	{
		_expeditionPlanner = Singletons.GetSingleton<IExpeditionPlanningManager>();
		_expeditionManager = Singletons.GetSingleton<IExpeditionManager>();
		_yingletRepository = Singletons.GetSingleton<ILocalYingletRepository>();

	}

	private void Update()
	{
		if (_expeditionManager.State.Val != ExpeditionState.Planning) return;
		if (_expeditionPlanner.CurrentParty.Count > 0) return;

		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Keypad1))
		{
			var allCharacters = _yingletRepository.GetAllYinglets().ToArray();

			for (int i = 0; i < 4; i++)
			{
				_expeditionPlanner.AddToParty(allCharacters[i].CachedData);
			}
			_expeditionManager.StartExpedition();
		}
	}
}