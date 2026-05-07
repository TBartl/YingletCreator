using UnityEngine;

namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class ChangeCharacterResourceNode : SingleOutputNode
	{
		[SerializeField] private AssetReferenceT<CharacterResourceId> _resource;
		[SerializeField] private int _delta;

		public CharacterResourceId Resource => _resource.LoadSync();
		public int Delta => _delta;

		public ChangeCharacterResourceNode(CharacterResourceId resource, int delta)
		{
			if (resource != null)
			{
				_resource = new AssetReferenceT<CharacterResourceId>(resource.UniqueAssetID);
			}
			_delta = delta;
		}

		public override void Run(IEncounterInstance encounterInstance)
		{
			var characterResources = encounterInstance.Character.GetComponentInChildrenSafe<ICharacterResources>();
			var currentResources = characterResources.GetResource(Resource);

			characterResources.SetResource(Resource, Mathf.Max(currentResources + Delta, 0));

			encounterInstance.ProgressToNode(_next);
		}
	}
}