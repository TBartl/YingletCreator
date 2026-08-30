using Snapshotter;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SnapshotterCharacterLayerSwapper : MonoBehaviour, ISnapshottableComponent
{
	public SnapshotOrder SnapshotOrder => SnapshotOrder.AfterAnimate;

	public Action PrepareForSnapshot(ISnapshotterReferences references)
	{
		var layer = references.LayerIndex;
		var originalLayers = new Dictionary<Transform, int>();

		// Store original layers and set to snapshot layer
		SetLayerRecursive(this.transform, layer, originalLayers);

		// Return action to restore original layers
		return () =>
		{
			foreach (var kvp in originalLayers)
			{
				if (kvp.Key != null)
				{
					kvp.Key.gameObject.layer = kvp.Value;
				}
			}
		};
	}

	private void SetLayerRecursive(Transform transform, int layer, Dictionary<Transform, int> originalLayers)
	{
		originalLayers[transform] = transform.gameObject.layer;
		transform.gameObject.layer = layer;

		foreach (Transform child in transform)
		{
			SetLayerRecursive(child, layer, originalLayers);
		}
	}
}
