using System;
using UnityEngine;

/// <summary>
/// This class doesn't really _do_ anything beyond forward events
/// The expectation is that the 
/// </summary>
public interface IToastManager
{
	void Show(string message);

	event Action<string> Toasted;
}

public class ToastManager : MonoBehaviour, IToastManager
{
	public event Action<string> Toasted;

	public void Show(string message)
	{
		Toasted?.Invoke(message);
	}
}
