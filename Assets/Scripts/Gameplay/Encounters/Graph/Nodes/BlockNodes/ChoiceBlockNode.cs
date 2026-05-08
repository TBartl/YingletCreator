using UnityEngine;

namespace Encounters.Runtime
{
	[System.Serializable]
	public class ChoiceBlockNode : SingleOutputNode
	{

		[field: SerializeField]
		public string Text { get; private set; }

		[field: SerializeReference]
		public IChoiceRequirementNode[] Requirements { get; private set; }

		public ChoiceBlockNode(string text, IChoiceRequirementNode[] requirements)
		{
			Text = text;
			Requirements = requirements;
		}

		/// <summary>
		/// The node after this
		/// Exposed for UI purposes: We want to display the subsequent roll type when applicable
		/// </summary>
		public IEncounterNode Next => _next;

		public override void Run(IEncounterInstance encounterInstance)
		{
			// Check all requirements met
			foreach (var requirement in Requirements)
			{
				if (!requirement.RequirementsMet(encounterInstance))
				{
					Debug.LogWarning($"ChoiceBlockNode requirement not met: {requirement.GetType().Name}");
					return;
				}
			}

			// Apply any requirement costs
			foreach (var requirement in Requirements)
			{
				requirement.Apply(encounterInstance);
			}

			// Go to our next node
			encounterInstance.ProgressToNode(_next);
		}
	}
}
