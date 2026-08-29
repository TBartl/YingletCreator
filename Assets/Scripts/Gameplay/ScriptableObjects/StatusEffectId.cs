using UnityEngine;

public interface IStatusEffectId
{
	string GetTooltipText();
}

public abstract class StatusEffectId : ScriptableObject, IHasUniqueAssetId, IStatusEffectId
{
	[SerializeField, HideInInspector] string _uniqueAssetId;
	public string UniqueAssetID { get => _uniqueAssetId; set => _uniqueAssetId = value; }

	public abstract string GetTooltipText();
}