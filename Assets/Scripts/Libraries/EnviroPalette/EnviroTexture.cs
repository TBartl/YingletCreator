using UnityEngine;



[CreateAssetMenu(fileName = "EnviroTexture", menuName = "Scriptable Objects/Enviro/Palette")]
public class EnviroTexture : ScriptableObject
{
	public Texture2D Texture;
	public EnviroRamp Ramp;
	public float HueInfluence = 1;

	[SerializeField] public EnviroColorRange ColorRange;

	[SerializeField] public EnviroMaskLayer[] MaskLayers;

	[SerializeField] public EnviroAdvancedSettings AdvancedSettings;

	// Just used for preview
	[HideInInspector] public Texture2D Generated;
}

[System.Serializable]
public class EnviroColorRange
{
	public Color MinColor = Color.black;
	public Color MidColor = Color.gray;
	public Color MaxColor = Color.white;
}

[System.Serializable]
public class EnviroAdvancedSettings
{
	public bool AllowBeyondRange = false;
	public float BelowRangeMultiplier = 1;
	public float AboveRangeMultiplier = 1;
}

[System.Serializable]
public class EnviroMaskLayer
{
	public Texture2D Texture;
	public EnviroRamp Ramp;
}