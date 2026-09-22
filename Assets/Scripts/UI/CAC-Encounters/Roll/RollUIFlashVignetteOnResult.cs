using Encounters.Runtime;
using UnityEngine;

public class RollUIFlashVignetteOnResult : MonoBehaviour
{
	[SerializeField] private RollUISettings _settings;
	private IUIVignetteManager _vignetteManager;
	private RollNodeVisitData _data;

	void Start()
	{

		_vignetteManager = Singletons.GetSingleton<IUIVignetteManager>();

		var reference = this.GetComponentInParentSafe<IEncounterNodeReferenceUI>(true);
		_data = reference.Record.VisitData as RollNodeVisitData;

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
		var color = _settings.RollClassificationColorMap[_data.RealBranch.Classification].JuicyColor;
		_vignetteManager.FlashVignette(color);
	}
}
