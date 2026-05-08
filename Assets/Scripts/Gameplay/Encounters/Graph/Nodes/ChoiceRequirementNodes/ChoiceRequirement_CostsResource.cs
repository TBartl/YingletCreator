using System.Text;
using UnityEngine;

namespace Encounters.Runtime
{
	[System.Serializable]
	public class ChoiceRequirement_CostsResource : IChoiceRequirementNode
	{
		[SerializeField] AssetReferenceT<CharacterResourceId> _resource;
		[SerializeField] int _amount;

		public ChoiceRequirement_CostsResource(CharacterResourceId resource, int amount)
		{
			_resource = new AssetReferenceT<CharacterResourceId>(resource.UniqueAssetID);
			_amount = amount;
		}

		public void Apply(IEncounterInstance encounter)
		{
			var resources = encounter.Character.GetComponentInChildrenSafe<ICharacterResources>();
			var current = resources.GetResource(_resource.LoadSync());
			resources.SetResource(_resource.LoadSync(), Mathf.Max(0, current - _amount));
		}

		public void AppendDisplayText(StringBuilder sb)
		{
			for (int i = 0; i < _amount; i++)
			{
				sb.Append(_resource.LoadSync().TMPIcon);
			}
		}

		public bool RequirementsMet(IEncounterInstance encounter)
		{
			var resources = encounter.Character.GetComponentInChildrenSafe<ICharacterResources>();
			var current = resources.GetResource(_resource.LoadSync());
			return current >= _amount;
		}
	}
}
