using UnityEngine;

namespace Encounters.Runtime
{
	public enum VfxSpawnTarget
	{
		Character,
		Encounter
	}

	[System.Serializable]
	public sealed class CreateVfxNode : SingleOutputNode
	{

		[SerializeField] private VfxSpawnTarget _spawnTarget;
		[SerializeField] private GameObject _vfxPrefab;

		public CreateVfxNode(VfxSpawnTarget spawnTarget, GameObject vfxPrefab)
		{
			_spawnTarget = spawnTarget;
			_vfxPrefab = vfxPrefab;
		}

		public override void Run(IEncounterInstance encounterInstance)
		{
			encounterInstance.ProgressToNode(_next);
			TrySpawnVfx(encounterInstance);
		}

		void TrySpawnVfx(IEncounterInstance encounterInstance)
		{
			if (_vfxPrefab == null)
			{
				Debug.LogWarning($"Trying to create VFX on encounter instance {encounterInstance} with null VFX prefab.");
				return;
			}

			Transform spawnParent = GetSpawnParent(encounterInstance);
			if (spawnParent == null)
			{
				Debug.LogWarning($"Trying to create VFX on encounter instance {encounterInstance} but could not find valid spawn target.");
				return;
			}

			GameObject.Instantiate(_vfxPrefab, spawnParent.transform.position, spawnParent.transform.rotation);
		}

		Transform GetSpawnParent(IEncounterInstance encounterInstance)
		{
			return _spawnTarget switch
			{
				VfxSpawnTarget.Character => encounterInstance.Character?.transform,
				VfxSpawnTarget.Encounter => encounterInstance.EncounterSource?.transform,
				_ => null
			};
		}
	}
}