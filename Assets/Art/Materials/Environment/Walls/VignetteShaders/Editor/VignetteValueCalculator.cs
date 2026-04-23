using UnityEngine;

[CreateAssetMenu(fileName = "VignetteValueCalculator", menuName = "Scriptable Objects/Editor/Vignette Value Calculator")]
public class VignetteValueCalculator : ScriptableObject
{
	[field: SerializeField] public float DistanceVisible { get; private set; }
	[field: SerializeField] public float DistanceBlackStart { get; private set; }
	[field: SerializeField] public float DistanceBlackEnd { get; private set; }
}