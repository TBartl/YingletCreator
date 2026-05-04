using UnityEngine;

namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class SetEncounterSpriteNode : SingleOutputNode
	{
		[SerializeField] private Sprite _sprite;

		public SetEncounterSpriteNode(Sprite sprite)
		{
			_sprite = sprite;
		}

		public override void Run(IEncounterInstance encounterInstance)
		{
			encounterInstance.ProgressToNode(_next);
			TrySetSprite(encounterInstance);
		}

		void TrySetSprite(IEncounterInstance encounterInstance)
		{
			var source = encounterInstance.EncounterSource;
			if (source == null)
			{
				Debug.LogWarning($"Trying to set encounter sprite on encounter instance {encounterInstance} with null source.");
				return;
			}
			var spriteRenderer = source.GetComponentInChildren<SpriteRenderer>();
			if (spriteRenderer == null)
			{
				Debug.LogWarning($"Trying to set encounter sprite on encounter instance {encounterInstance} with source {source} that has no SpriteRenderer.");
				return;
			}
			spriteRenderer.sprite = _sprite;

		}
	}
}