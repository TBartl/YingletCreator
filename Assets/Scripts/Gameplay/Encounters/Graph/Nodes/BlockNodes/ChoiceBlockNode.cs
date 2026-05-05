using UnityEngine;

namespace Encounters
{
	public enum ChoiceBlockRollType
	{
		None,
		RollAny,
		RollBody,
		RollMind
	}
}

namespace Encounters.Runtime
{
	[System.Serializable]
	public class ChoiceBlockNode : SingleOutputNode
	{
		[field: SerializeField]
		public int EnergyCost { get; private set; }

		[field: SerializeField]
		public string Text { get; private set; }

		public ChoiceBlockNode(int energyCost, string text)
		{
			EnergyCost = energyCost;
			Text = text;
		}

		/// <summary>
		/// The node after this
		/// Exposed for UI purposes: We want to display the subsequent roll type when applicable
		/// </summary>
		public IEncounterNode Next => _next;

		public override void Run(IEncounterInstance encounterInstance)
		{
			// Charge the player
			if (EnergyCost > 0)
			{
				var characterResources = encounterInstance.Character.GetComponentInChildrenSafe<ICharacterResources>();
				int currentEnergy = characterResources.GetResource(CharacterResourceType.Energy);
				if (currentEnergy < EnergyCost)
				{
					Debug.LogWarning($"Not enough energy to run ChoiceBlockNode. Required: {EnergyCost}, Current: {currentEnergy}");
				}
				characterResources.SetResource(CharacterResourceType.Energy, Mathf.Max(currentEnergy - EnergyCost, 0));
			}

			// Go to our next node
			encounterInstance.ProgressToNode(_next);
		}
	}
}
