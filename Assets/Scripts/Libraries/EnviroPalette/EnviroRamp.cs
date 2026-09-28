using UnityEngine;

[CreateAssetMenu(fileName = "EnviroRamp", menuName = "Scriptable Objects/Enviro/Ramp")]
public class EnviroRamp : ScriptableObject
{
	public Gradient Gradient;
	[HideInInspector] public Texture2D Generated;
}
