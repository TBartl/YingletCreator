using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Generates a camera and render texture for the lifetime of this object which can be used to sample ambient occlusion values
/// </summary>
public sealed class AmbientOcclusionSamplerCamera : IDisposable
{
	private readonly RenderTexture _renderTexture;
	private readonly GameObject _cameraGO;
	private readonly Camera _camera;
	private readonly IAmbientOcclusionSamplerCameraOffsetter _offsetter;
	private readonly Texture2D _texture2D;

	public AmbientOcclusionSamplerCamera(VertexColorBakingSettings settings, IAmbientOcclusionSamplerCameraOffsetter offsetter)
	{
		_offsetter = offsetter;

		_texture2D = new Texture2D(VertexColorBakingSettings.SamplingTextureSize, VertexColorBakingSettings.SamplingTextureSize, TextureFormat.RGBA64, false);
		_renderTexture = new RenderTexture(VertexColorBakingSettings.SamplingTextureSize, VertexColorBakingSettings.SamplingTextureSize, 16, RenderTextureFormat.ARGB64);

		_cameraGO = new GameObject("VertexColorBakingCamera");
		_camera = _cameraGO.AddComponent<Camera>();
		_camera.targetTexture = _renderTexture;
		_camera.aspect = 1.0f;
		_camera.nearClipPlane = VertexColorBakingSettings.SamplingBias;
		_camera.farClipPlane = settings.SamplingDistance;
		_camera.fieldOfView = Mathf.Clamp(settings.SamplingCameraFOV, 5, 170);
		_camera.clearFlags = CameraClearFlags.SolidColor;
		_camera.backgroundColor = Color.white;
		var mask = Tools.visibleLayers;
		mask &= ~(1 << VertexColorBakingSettings.NoRenderLayer);
		_camera.cullingMask = mask;
		_camera.enabled = false; // We render with .Render
		var shader = Shader.Find("Hidden/VertexColorBakingAmbientOcclusion");
		_camera.SetReplacementShader(shader, "");

	}
	public void Dispose()
	{
		_camera.targetTexture = null;
		GameObject.DestroyImmediate(_texture2D);
		GameObject.DestroyImmediate(_renderTexture);
		GameObject.DestroyImmediate(_cameraGO);
	}

	public float SampleAO(Vector3 position, Vector3 normal)
	{
		_cameraGO.transform.position = position + normal * VertexColorBakingSettings.SamplingBias;
		_cameraGO.transform.LookAt(position + normal);
		_offsetter.AdjustPosition(_cameraGO.transform);
		_camera.Render();

		RenderTexture.active = _renderTexture;

		_texture2D.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
		_texture2D.Apply();
		var averageValue = GetAverageValue(_texture2D);

		RenderTexture.active = null;
		return averageValue;
	}

	static float GetAverageValue(Texture2D texture)
	{
		// Get all pixels of the texture
		Color[] pixels = texture.GetPixels();

		float totalR = 0f;

		foreach (Color pixel in pixels)
		{
			totalR += pixel.r;
		}

		return totalR / pixels.Length;
	}
}

public interface IAmbientOcclusionSamplerCameraOffsetter
{
	public void AdjustPosition(Transform camera);
}