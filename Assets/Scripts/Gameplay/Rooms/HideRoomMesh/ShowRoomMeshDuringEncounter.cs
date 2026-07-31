using Reactivity;
using UnityEngine;

[RequireComponent(typeof(RoomMeshVisibilityTransitioner))]
public class ShowRoomMeshDuringEncounter : ReactiveBehaviour
{
	private IRoomMeshVisibilityConditions _roomVisibilityConditions;
	private IRoomMeshVisibilityTransitioner _visibilityTransitioner;

	private void Start()
	{
		_roomVisibilityConditions = this.GetComponentInParentSafe<IRoomMeshVisibilityConditions>();

		_visibilityTransitioner = this.GetComponentSafe<IRoomMeshVisibilityTransitioner>();

		var show = _roomVisibilityConditions.ShowDuringEncounter;
		show.OnChanged += Show_OnChanged;

		_visibilityTransitioner.ForceTo(show.Val);
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		if (_roomVisibilityConditions != null)
		{
			_roomVisibilityConditions.ShowDuringEncounter.OnChanged -= Show_OnChanged;
		}
	}

	private void Show_OnChanged(bool from, bool to)
	{
		_visibilityTransitioner.TransitionTo(to);
	}
}
