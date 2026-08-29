using Character.Data;
using UnityEngine;


[CreateAssetMenu(fileName = "AddCharacterToggle", menuName = "Scriptable Objects/Gameplay/StatusEffect/AddCharacterToggle")]
public class StatusEffect_AddCharacterToggle : StatusEffectId
{
	[SerializeField] AssetReferenceT<CharacterToggleId> _toggle;
	public CharacterToggleId Toggle => _toggle.LoadSync();

	public override string GetTooltipText() => null;
}
