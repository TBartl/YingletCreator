using System;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor;
namespace Encounters.Editor
{
	[Serializable]
	[Graph(ASSET_EXTENSION)]
	public class EncounterGraph : Graph
	{
		internal const string GRAPH_NAME = "Encounter";
		internal const string ASSET_EXTENSION = "encountergraph";

		[MenuItem("Assets/Create/Scriptable Objects/Gameplay/Encounter")]
		static void CreateAssetFile()
		{
			GraphDatabase.PromptInProjectBrowserToCreateNewAsset<EncounterGraph>(GRAPH_NAME);
		}

		public override void OnGraphChanged(GraphLogger infos)
		{
			base.OnGraphChanged(infos);

			CheckGraphErrors(infos);
		}

		void CheckGraphErrors(GraphLogger infos)
		{
			var startNodes = GetNodes().OfType<StartNode>().ToList();

			switch (startNodes.Count)
			{
				case 0:
					infos.LogError("Add a StartNode in your Encounter graph.", this);
					break;
				case >= 1:
					{
						foreach (var startNode in startNodes.Skip(1))
						{
							infos.LogWarning($"EncounterGraph only supports one StartNode per graph. Only the first created one will be used.", startNode);
						}
						break;
					}
			}
		}
	}
}