using Reactivity;
using UnityEngine;

public interface ICharacterStatuses
{
	void AddStatus(StatusId status);
	IObservableEnumerable<StatusId> Statuses { get; }
}

public class CharacterStatuses : MonoBehaviour, ICharacterStatuses
{
	ObservableList<StatusId> _statuses = new ObservableList<StatusId>();
	public IObservableEnumerable<StatusId> Statuses => _statuses;

	public void AddStatus(StatusId status)
	{
		Debug.Log($"Adding status {status.DisplayName} to {gameObject.name}");
		_statuses.Add(status);
	}
}
