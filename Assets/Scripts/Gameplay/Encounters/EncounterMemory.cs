using System.Collections.Generic;

public interface IEncounterMemory
{
	void Write(string key, int value);
	int Read(string key);
}

internal sealed class EncounterMemory : IEncounterMemory
{
	Dictionary<string, int> _memory = new Dictionary<string, int>();
	public void Write(string key, int value)
	{
		_memory[key] = value;
	}
	public int Read(string key)
	{
		return _memory.TryGetValue(key, out var value) ? value : 0;
	}
}
