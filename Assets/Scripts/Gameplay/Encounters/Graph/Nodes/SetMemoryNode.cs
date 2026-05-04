using UnityEngine;

namespace Encounters.Runtime
{

	[System.Serializable]
	public sealed class SetMemoryNode : SingleOutputNode
	{
		[SerializeField] private string _key;
		[SerializeField] private int _value;

		public SetMemoryNode(string key, int value)
		{
			_key = key;
			_value = value;
		}

		public override void Run(IEncounterInstance encounterInstance)
		{
			encounterInstance.Memory.Write(_key, _value);
			encounterInstance.ProgressToNode(_next);
		}
	}
}