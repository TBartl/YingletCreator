using System;
using System.Collections.Generic;
using UnityEngine;

public enum SurfaceSoundType
{
	Footstep,
	Landing
}
public interface ISurfaceSoundProvider
{
	SoundEffect GetSound(PhysicsMaterial material, SurfaceSoundType soundType);
}

[Serializable]
public class SurfaceSoundEntry
{
	public PhysicsMaterial Material;
	public SoundEffect Footstep;
	public SoundEffect Landing;
}

public class SurfaceSoundProvider : MonoBehaviour, ISurfaceSoundProvider
{
	[SerializeField] private SurfaceSoundEntry _defaultEntry;
	[SerializeField] private SurfaceSoundEntry[] _entries;

	private Dictionary<PhysicsMaterial, SurfaceSoundEntry> _lookup = new Dictionary<PhysicsMaterial, SurfaceSoundEntry>();

	void Awake()
	{
		int count = _entries.Length;

		foreach (var entry in _entries)
		{
			_lookup[entry.Material] = entry;
		}
	}

	public SoundEffect GetSound(PhysicsMaterial material, SurfaceSoundType soundType)
	{
		var entry = GetEntry(material);

		return soundType switch
		{
			SurfaceSoundType.Footstep => entry.Footstep,
			SurfaceSoundType.Landing => entry.Landing,
			_ => throw new Exception($"Unsupported SurfaceSoundType: {soundType}")
		};
	}

	SurfaceSoundEntry GetEntry(PhysicsMaterial material)
	{
		if (material == null) return _defaultEntry;
		if (_lookup.TryGetValue(material, out var entry))
		{
			return entry;
		}
		return _defaultEntry;
	}
}