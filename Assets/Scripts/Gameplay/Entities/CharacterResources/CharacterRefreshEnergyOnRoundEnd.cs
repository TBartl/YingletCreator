using UnityEngine;

public class CharacterRefreshEnergyOnRoundEnd : MonoBehaviour
{
	private ICommonGameplayAssets _assets;
	private ICharacterResources _resources;
	private IExpeditionRoundManager _roundManager;

	void Start()
	{
		_assets = Singletons.GetSingleton<ICommonGameplayAssets>();
		_resources = this.GetCharacterRootComponent<ICharacterResources>();
		_roundManager = this.GetExpeditionComponent<IExpeditionRoundManager>();

		_roundManager.CurrentRound.OnChanged += CurrentRound_OnChanged;
	}

	private void OnDestroy()
	{
		if (_roundManager != null)
		{
			_roundManager.CurrentRound.OnChanged -= CurrentRound_OnChanged;
		}
	}

	private void CurrentRound_OnChanged(int from, int to)
	{
		_resources.SetResource(_assets.ResourceEnergy, CharacterResources.MAX_ENERGY);
	}
}
