using Reactivity;
using UnityEngine;

public class HidePassageTopWhenObstructing : ReactiveBehaviour
{
	[SerializeField] float _minYCutoff = 1.2f;
	[SerializeField] float _maxYCutoff = 1.97f;
	[SerializeField] SharedEaseSettings _easeSettings;
	private ICurrentRoomProvider _currentRoomProvider;
	Shader CUTOFF_FROM_TOP_SHADER;
	static readonly int Y_CUTOFF_PROPERTY_ID = Shader.PropertyToID("_YCutoff");

	private Computed<bool> _show;
	private IRoom _roomToObstructIn;
	private float _currentYCutoff;
	private Coroutine _transitionCoroutine;
	private MeshRenderer _mr;
	private Material _originalMaterial;
	private Material _cutOffMaterial;

	private void Start()
	{
		// See if we're a horizontal passage. If so, this will never be applicable
		var room = this.GetComponentInParentSafe<IRoom>();
		var localPositionRelativeToRoom = this.transform.position - room.WorldPosition;
		if (Mathf.Abs(localPositionRelativeToRoom.x) > 1)
		{
			Destroy(this);
			return;
		}


		CUTOFF_FROM_TOP_SHADER = Shader.Find("Shader Graphs/CutOffFromTop");
		var roomManager = this.GetExpeditionComponent<IRoomManager>();
		_currentRoomProvider = Singletons.GetSingleton<ICurrentRoomProvider>();

		_mr = this.GetComponentSafe<MeshRenderer>();
		_originalMaterial = _mr.sharedMaterial;
		_cutOffMaterial = new Material(_originalMaterial);
		_cutOffMaterial.shader = CUTOFF_FROM_TOP_SHADER;

		// Calculate the room
		var expectedRoomPos = Room.GetRoomPosFromWorldPos(this.transform.position + new Vector3(0, 0, RoomManager.ROOM_SIZE / 2));
		_roomToObstructIn = roomManager.GetRoom(expectedRoomPos);

		// Compute show and act off of it
		_show = CreateComputed(ComputeShow);
		_show.OnChanged += Show_OnChanged;

		_currentYCutoff = _show.Val ? _maxYCutoff : _minYCutoff;
		if (!_show.Val)
		{
			this.gameObject.SetActive(false);
		}
	}

	private bool ComputeShow()
	{
		return _currentRoomProvider.CurrentRoom.Val != _roomToObstructIn;
	}
	private void Show_OnChanged(bool _, bool show)
	{
		var fromY = _currentYCutoff;
		var toY = show ? _maxYCutoff : _minYCutoff;
		this.gameObject.SetActive(true);
		_mr.sharedMaterial = _cutOffMaterial;
		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, Apply, OnComplete);


		void Apply(float p)
		{
			_currentYCutoff = Mathf.Lerp(fromY, toY, p);
			_cutOffMaterial.SetFloat(Y_CUTOFF_PROPERTY_ID, _currentYCutoff);
		}
		void OnComplete()
		{
			_mr.sharedMaterial = _originalMaterial;
			this.gameObject.SetActive(show);
		}
	}

}
