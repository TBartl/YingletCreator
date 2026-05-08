using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum StatusSentiment
{
	Negative,
	Neutral,
	Positive,
}

/// <summary>
/// Contains some number of status effects
/// Also contains an icon and name to be diplayed in the UI
/// </summary>
[CreateAssetMenu(fileName = "Status", menuName = "Scriptable Objects/Gameplay/Status")]
public class StatusId : ScriptableObject, IHasUniqueAssetId
{
	[SerializeField, HideInInspector] string _uniqueAssetId;
	public string UniqueAssetID { get => _uniqueAssetId; set => _uniqueAssetId = value; }

	[field: SerializeField] public Sprite Icon { get; private set; }
	[SerializeField] string _overrideDisplayName;
	public string DisplayName => string.IsNullOrEmpty(_overrideDisplayName) ? name : _overrideDisplayName;
	[field: SerializeField] public StatusSentiment Sentiment { get; private set; } = StatusSentiment.Neutral;

	[SerializeField] AssetReferenceT<StatusEffectId>[] _statusEffects;
	public IEnumerable<StatusEffectId> StatusEffects => _statusEffects.Select(status => status.LoadSync());

}
