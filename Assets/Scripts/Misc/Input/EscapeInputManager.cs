using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EscapeInputPriority
{
	CloseMenu,
	ClosePopupModal
}

public interface IEscapeInputConsumer
{
	/// <summary>
	/// Return true if the escape key was handled
	/// </summary>
	bool OnEscape();

	EscapeInputPriority EscapeInputPriority { get; }
}

public interface IEscapeInputManager
{
	void Register(IEscapeInputConsumer escapeInputConsumer);
	void Unregister(IEscapeInputConsumer escapeInputConsumer);
}

public class EscapeInputManager : MonoBehaviour, IEscapeInputManager
{
	List<IEscapeInputConsumer> _escapeInputConsumers = new List<IEscapeInputConsumer>();

	void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Escape)) return;

		var sortedConsumers = _escapeInputConsumers.OrderByDescending(c => c.EscapeInputPriority).ToList();
		foreach (var escapeInputConsumer in sortedConsumers)
		{
			if (escapeInputConsumer.OnEscape())
			{
				break;
			}
		}
	}

	public void Register(IEscapeInputConsumer escapeInputConsumer)
	{
		if (!_escapeInputConsumers.Contains(escapeInputConsumer))
		{
			_escapeInputConsumers.Add(escapeInputConsumer);
		}
	}

	public void Unregister(IEscapeInputConsumer escapeInputConsumer)
	{
		_escapeInputConsumers.Remove(escapeInputConsumer);
	}
}
