using UnityEngine;

/// <summary>
/// Controls the preview portrait generated for characters
/// </summary>
[CreateAssetMenu(fileName = "Stat", menuName = "Scriptable Objects/Gameplay/Stat")]
public class StatId : ScriptableObject, IHasUniqueAssetId, IOrderableScriptableObject
{
	[SerializeField, HideInInspector] string _uniqueAssetId;
	public string UniqueAssetID { get => _uniqueAssetId; set => _uniqueAssetId = value; }

	[SerializeField] int _orderIndex;
	public int OrderIndex => _orderIndex;
}
