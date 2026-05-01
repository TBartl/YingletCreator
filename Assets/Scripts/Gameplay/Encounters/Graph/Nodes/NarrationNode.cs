using UnityEngine;

namespace Encounters.Runtime
{
	[System.Serializable]
	public sealed class NarrationNode : SingleOutputNode
	{
		[field: SerializeField]
		public string Text { get; }

		public NarrationNode(string text)
		{
			Text = text;
		}
	}
}