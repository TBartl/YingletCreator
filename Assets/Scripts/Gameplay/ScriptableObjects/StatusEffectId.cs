using System.Text;
using UnityEngine;

public interface IStatusEffectId
{
	void AppendTooltipText(StringBuilder sb);
}

public abstract class StatusEffectId : ScriptableObject, IHasUniqueAssetId, IStatusEffectId
{
	[SerializeField, HideInInspector] string _uniqueAssetId;
	public string UniqueAssetID { get => _uniqueAssetId; set => _uniqueAssetId = value; }

	public abstract void AppendTooltipText(StringBuilder sb);
}