using System;
using UnityEngine;

/// <summary>
/// You cannot call Camera.Render within the prefab view
/// If we attempt to bake to a prefab, we need to instantiate a copy of the objects in the scene that we can use to render the camera
/// This object creates that only if needed for the lifetime of this object
/// We create this at some offset far from the center to avoid any existing geometry already in the scene
/// </summary>
public class VertexColorPrefabInstantiator : IDisposable, IAmbientOcclusionSamplerCameraOffsetter
{
	private readonly GameObject _instantiatedObject;

	public VertexColorPrefabInstantiator(Transform root)
	{
		if (!IsInPrefabView(root.gameObject)) return;

		// If we made it this far, we need to insantiate a copy of this in the scene
		_instantiatedObject = GameObject.Instantiate(root.gameObject);
		_instantiatedObject.transform.position += VertexColorBakingSettings.InstantiatedPrefabOffset;
	}

	public void Dispose()
	{
		if (_instantiatedObject == null) return;

		GameObject.DestroyImmediate(_instantiatedObject);
	}

	public static bool IsInPrefabView(GameObject obj)
	{
		var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
		if (prefabStage == null) return false;

		return prefabStage.prefabContentsRoot == obj || obj.transform.IsChildOf(prefabStage.prefabContentsRoot.transform);
	}

	public void AdjustPosition(Transform camera)
	{
		if (_instantiatedObject == null) return;
		camera.transform.position += VertexColorBakingSettings.InstantiatedPrefabOffset;
	}
}
