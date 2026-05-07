using UnityEngine;

/// <summary>
/// Not to be confused with resource loading
/// This is a resource the character may have some amount of
/// This can go up and down, but should never be negative
/// </summary>
[CreateAssetMenu(fileName = "Resource", menuName = "Scriptable Objects/Gameplay/CharacterResource")]
public class CharacterResourceId : ScriptableObject, IHasUniqueAssetId
{
	[SerializeField, HideInInspector] string _uniqueAssetId;
	public string UniqueAssetID { get => _uniqueAssetId; set => _uniqueAssetId = value; }

	[SerializeField] string _textIconName = "Energy";
	public string TextIconName => _textIconName;

}
