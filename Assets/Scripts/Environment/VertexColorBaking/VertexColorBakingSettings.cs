using UnityEngine;

[System.Serializable]
public class VertexColorBakingSettings
{
	public const int SamplingTextureSize = 16;

	public const float SamplingBias = 0.001f;
	[Range(0.01f, 100.0f)]
	public float SamplingDistance = 10.0f;
	[Range(45.0f, 360.0f)]
	public float SamplingCameraFOV = 135.0f;

	[ColorUsage(showAlpha: true, hdr: true)]
	public Color SkyColor = Color.white;
	public Color ShadowColor = Color.black;

	public AnimationCurve RawBakeValToP = AnimationCurve.Linear(0, 1, 0, 1);
	public AnimationCurve PointLightFalloff = AnimationCurve.Linear(0, 1, 1, 0);

	public const int NoRenderLayer = 29; // Chosen at random; should probably be something that's free
	public static Vector3 InstantiatedPrefabOffset => new Vector3(2000, 0, 0); // chosen at random; should be far from the center of the scene. See VertexColorPrefabInstantiator for details
}
