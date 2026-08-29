using UnityEngine;


/// <summary>
/// Like a component for StatusId
/// </summary>
[CreateAssetMenu(fileName = "ChangeStat", menuName = "Scriptable Objects/Gameplay/StatusEffect/ChangeStat")]
public class StatusEffect_ChangeStat : StatusEffectId
{
	[SerializeField] AssetReferenceT<StatId> _stat;
	[SerializeField] int _delta;

	public StatId Stat => _stat.LoadSync();
	public int Delta => _delta;

	public override string GetTooltipText()
	{
		return TMPUtils.ColorizeLabelWithNumber($"{Stat.ShortName}: {{0}}", Delta);
	}
}
