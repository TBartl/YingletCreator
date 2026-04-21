using Character.Creator;
using Reactivity;
using System.Collections.Generic;
using UnityEngine;

public interface IExpeditionPlanningManager
{
	IList<CachedYingletReference> CurrentParty { get; }

	void AddToParty(CachedYingletReference character);
}


public class ExpeditionPlanningManager : MonoBehaviour, IExpeditionPlanningManager
{
	public const int MAX_CHARACTERS = 4;

	private ObservableList<CachedYingletReference> _currentParty = new ObservableList<CachedYingletReference>();

	public IList<CachedYingletReference> CurrentParty => _currentParty;

	public void AddToParty(CachedYingletReference character)
	{
		if (_currentParty.Count < MAX_CHARACTERS)
		{
			_currentParty.Add(character);
		}
		else
		{
			Debug.LogWarning("Max party size reached");
		}
	}
}
