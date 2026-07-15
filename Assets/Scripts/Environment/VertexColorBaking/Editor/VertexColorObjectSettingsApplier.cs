using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Ensures that all objects are ready for baking based on the settings on individual objects
/// Settings are applied for the lifetime of this object
/// </summary>
public class VertexColorObjectSettingsApplier : IDisposable
{
	private readonly IEnumerable<MeshRenderer> _disabledMeshRenderers;

	public VertexColorObjectSettingsApplier(IEnumerable<Transform> objects)
	{

		var dontObfuscate = objects
			.Select(o => o.GetComponent<VertexColorObject_DontObfuscate>())
			.Where(c => c != null)
			.ToArray();

		var disabledMeshRenders = new List<MeshRenderer>();
		foreach (var setting in dontObfuscate)
		{
			var meshRender = setting.GetComponent<MeshRenderer>();
			if (meshRender == null)
			{
				Debug.LogWarning("Settings called for no obfuscation, but the object didn't even have a MeshRenderer to begin with", setting.gameObject);
				continue;
			}
			if (meshRender.enabled == false)
			{
				Debug.LogWarning("Settings called for no obfuscation, but the object didn't even have its mesh render enabled to begin with", setting.gameObject);
				continue;
			}
			meshRender.enabled = false;
			disabledMeshRenders.Add(meshRender);
		}
		_disabledMeshRenderers = disabledMeshRenders;

	}

	public void Dispose()
	{
		foreach (var meshRender in _disabledMeshRenderers)
		{
			meshRender.enabled = true;
		}
	}
}
