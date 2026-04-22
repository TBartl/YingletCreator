using Character.Creator;
using Reactivity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ExpeditionPartyMember
{
	public ExpeditionPartyMember(uint id, SerializableCustomizationData customizationData)
	{
		Id = id;
		CustomizationData = customizationData;
		// ClassId Class
	}

	public SerializableCustomizationData CustomizationData { get; }
	public uint Id { get; }
}

public interface IExpeditionPlanningManager
{
	IList<ExpeditionPartyMember> CurrentParty { get; }

	void AddToParty(SerializableCustomizationData customizationData);
	void RemoveFromParty(uint id);
}


public class ExpeditionPlanningManager : MonoBehaviour, IExpeditionPlanningManager
{
	public const int MAX_CHARACTERS = 4;
	uint _currentId = 0;

	private ObservableList<ExpeditionPartyMember> _currentParty = new ObservableList<ExpeditionPartyMember>();
	private INetStateReader _netState;

	public IList<ExpeditionPartyMember> CurrentParty => _currentParty;

	private void Awake()
	{
		_netState = Singletons.GetSingleton<INetStateReader>();

		_netState.OnLocalDisconnected += NetState_OnLocalDisconnected;
	}

	private void NetState_OnLocalDisconnected()
	{
		_currentParty.Clear();
		_currentId = 0;
	}

	public void AddToParty(SerializableCustomizationData customizationData)
	{
		if (_currentParty.Count < MAX_CHARACTERS)
		{
			var newMember = new ExpeditionPartyMember(_currentId++, customizationData);
			_currentParty.Add(newMember);
		}
		else
		{
			Debug.LogWarning("Max party size reached");
		}
	}

	public void RemoveFromParty(uint id)
	{
		var member = _currentParty.FirstOrDefault(m => m.Id == id);
		if (member != null)
		{
			_currentParty.Remove(member);
		}
	}
}
