using Encounters.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Encounters.Editor
{

	[ScriptedImporter(1, EncounterGraph.ASSET_EXTENSION)]
	internal class EncounterGraphImporter : ScriptedImporter
	{
		public override void OnImportAsset(AssetImportContext ctx)
		{
			var graph = GraphDatabase.LoadGraphForImporter<EncounterGraph>(ctx.assetPath);


			if (graph == null)
			{
				Debug.LogError($"Failed to load Encounter graph asset: {ctx.assetPath}");
				return;
			}

			var startNodeModel = graph.GetNodes().OfType<StartNode>().FirstOrDefault();
			if (startNodeModel == null)
			{
				// No need to log an error here, as the EncounterGraph is already logging an error in the console
				return;
			}

			// Build the runtime asset by walking the graph and adding the relevant nodes.
			var runtimeAsset = ScriptableObject.CreateInstance<Runtime.EncounterGraph>();

			BuildRuntimeGraph(startNodeModel, runtimeAsset);

			// Add the runtime object to the graph asset and set it to be the main asset.
			ctx.AddObjectToAsset("RuntimeAsset", runtimeAsset);
			ctx.SetMainObject(runtimeAsset);
		}

		static void BuildRuntimeGraph(INode startNode, Runtime.EncounterGraph runtimeAsset)
		{
			(List<INode> allConnectedNodes, Dictionary<INode, IList<INode>> nodeConnections) = TraverseRawEditorNodes(startNode);

			List<IEncounterNode> runtimeNodes = CreateRuntimeNodesFromEditorNodes(allConnectedNodes);

			ConnectRuntimeNodes(runtimeNodes, allConnectedNodes, nodeConnections);

			var firstNode = runtimeNodes.FirstOrDefault();
			if (firstNode == null)
			{
				throw new InvalidOperationException("No runtime nodes were created from the editor nodes.");
			}
			runtimeAsset.EditorSetData((Runtime.StartNode)firstNode);
		}

		static (List<INode>, Dictionary<INode, IList<INode>>) TraverseRawEditorNodes(INode startNode)
		{
			List<INode> allConnectedNodes = new List<INode>();
			Dictionary<INode, IList<INode>> nodeConnections = new Dictionary<INode, IList<INode>>();

			allConnectedNodes.Add(startNode);
			RecurseNode(startNode);

			return (allConnectedNodes, nodeConnections);

			void RecurseNode(INode node)
			{
				// Most nodes have a singular "ExecutionPort" node. Add it if available
				var connectedExecutionNode = GetExecutionOutputNode(node);
				AddConnectedNode(node, connectedExecutionNode);

				// Nodes with multiple outcomes will have "block nodes". Add those as well.
				var blockNodes = GetBlockNodes(node);
				foreach (var blockNode in blockNodes)
				{
					AddConnectedNode(node, blockNode);
				}
			}

			void AddConnectedNode(INode fromNode, INode toNode)
			{
				if (toNode == null) return;
				if (nodeConnections.TryGetValue(fromNode, out var existingList))
				{
					existingList.Add(toNode);
				}
				else
				{
					nodeConnections[fromNode] = new List<INode> { toNode };
				}

				// Recurse through the connected node if needed
				if (allConnectedNodes.Contains(toNode)) return; // We've already visited this node
				allConnectedNodes.Add(toNode);
				RecurseNode(toNode);
			}
		}

		static List<IEncounterNode> CreateRuntimeNodesFromEditorNodes(List<INode> editorNodes)
		{
			var runtimeNodes = new List<IEncounterNode>();
			foreach (var node in editorNodes)
			{
				var editorNode = node as IEditorNode;
				if (editorNode == null)
				{
					throw new ArgumentException($"Node {node} does not implement IEditorNode and cannot be converted to a runtime node.");
				}
				var runtimeNode = editorNode.CreateRuntimeNode();
				runtimeNodes.Add(runtimeNode);
			}
			return runtimeNodes;
		}

		static INode GetOutputNode(INode currentNode, string portName)
		{
			var outputPort = currentNode.GetOutputPortByName(portName);
			var nextNodePort = outputPort?.FirstConnectedPort;
			return nextNodePort?.GetNode();
		}

		static INode GetExecutionOutputNode(INode currentNode)
		{
			return GetOutputNode(currentNode, EditorNodeUtils.EXECUTION_PORT_NAME);
		}

		static IEnumerable<INode> GetBlockNodes(INode currentNode)
		{
			var contextNode = currentNode as ContextNode;
			if (contextNode == null)
			{
				return Enumerable.Empty<INode>();
			}

			return contextNode.BlockNodes.ToArray();
		}

		static void ConnectRuntimeNodes(IList<IEncounterNode> runtimeNodes, IList<INode> editorNodes, Dictionary<INode, IList<INode>> editorNodeConnections)
		{
			if (runtimeNodes.Count != editorNodes.Count)
			{
				throw new ArgumentException("The number of runtime nodes must match the number of editor nodes to establish connections.");
			}


			Dictionary<INode, int> editorNodeIndices = new Dictionary<INode, int>();
			for (int i = 0; i < editorNodes.Count; i++)
			{
				editorNodeIndices[editorNodes[i]] = i;
			}

			// Convert editor node connections to int-based connections
			Dictionary<int, IList<int>> nodeConnections = new Dictionary<int, IList<int>>();
			foreach (var kvp in editorNodeConnections)
			{
				int sourceIndex = editorNodeIndices[kvp.Key];
				var targetIndices = new List<int>();
				foreach (var connectedNode in kvp.Value)
				{
					int targetIndex = editorNodeIndices[connectedNode];
					targetIndices.Add(targetIndex);
				}
				if (targetIndices.Count > 0)
				{
					nodeConnections[sourceIndex] = targetIndices;
				}
			}

			for (int i = 0; i < runtimeNodes.Count; i++)
			{
				var runtimeNode = runtimeNodes[i];
				var editorNode = editorNodes[i];

				if (nodeConnections.TryGetValue(i, out var connectedIndices))
				{
					var connectedRuntimeNodes = connectedIndices.Select(index => runtimeNodes[index]).ToList();
					runtimeNode.EditorSetConnections(connectedRuntimeNodes);
				}
			}
		}
	}
}
