using Character.Creator;
using Reactivity;
using System.Linq;
using UnityEngine;


/// <summary>
/// Pretty small for now; will expand this later
/// </summary>
public class EnvironmentLoader : ReactiveBehaviour
{
	private ICustomizationSelectedDataRepository _dataRepository;
	Computed<GameObject> _roomPrefab;
	GameObject _cachedRoomObject;

	void Start()
	{
		_dataRepository = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
		_roomPrefab = this.CreateComputed(ComputeRoomPrefab);
		AddReflector(ReflectRoom);
	}

	private GameObject ComputeRoomPrefab()
	{
		var toggles = _dataRepository.CustomizationData.ToggleData.Toggles;
		return toggles.FirstOrDefault(t => t.RoomPrefab != null)?.RoomPrefab;
	}

	void ReflectRoom()
	{
		if (_cachedRoomObject != null)
		{
			Destroy(_cachedRoomObject);
		}
		var roomPrefab = _roomPrefab.Val;
		if (roomPrefab != null)
		{
			_cachedRoomObject = GameObject.Instantiate(roomPrefab, this.transform);
		}
	}
}
