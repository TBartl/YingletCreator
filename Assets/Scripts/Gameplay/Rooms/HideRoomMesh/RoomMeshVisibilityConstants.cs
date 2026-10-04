using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomMeshVisibilityConstants : MonoBehaviour, IInitializable
{
	[SerializeField] public List<CutoffReplacementShaderPair> _cutoffReplacementShaders;
	Dictionary<Shader, Shader> _cutoffReplacementShaderLookup;

	[field: SerializeField] public SharedEaseSettings EaseSettings { get; private set; }
	[field: SerializeField] public Vector2 PassageRange { get; private set; }

	public int Y_CUTOFF_PROPERTY_ID { get; private set; }

	public void Initialize()
	{
		Y_CUTOFF_PROPERTY_ID = Shader.PropertyToID("_YCutoff");
		_cutoffReplacementShaderLookup = _cutoffReplacementShaders.ToDictionary(pair => pair.Original, pair => pair.Replacement);
	}

	public Shader GetReplacementShader(Shader original)
	{
		if (_cutoffReplacementShaderLookup.TryGetValue(original, out var replacement))
		{
			return replacement;
		}
		Debug.LogWarning($"No replacement shader found for original shader: {original.name}. Returning original shader.");
		return original;
	}
}

[System.Serializable]
public sealed class CutoffReplacementShaderPair
{
	public Shader Original;
	public Shader Replacement;
}
