using UnityEngine;

[CreateAssetMenu(fileName = "EnviroTexture", menuName = "Scriptable Objects/Enviro/Palette")]
public class EnviroTexture : ScriptableObject
{
	public Texture2D Texture;
	public EnviroRamp Ramp;
	public float Hue = 0;
	public float HueInfluence = 1;

	// Just used for preview
	[HideInInspector] public Texture2D Generated;
}
