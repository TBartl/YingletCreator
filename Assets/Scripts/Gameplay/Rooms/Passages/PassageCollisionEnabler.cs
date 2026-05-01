using Reactivity;
using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Enables or disables the passage's collision with the active character
/// </summary>
public class PassageCollisionEnabler : ReactiveBehaviour
{
	private IActiveRoomProvider _activeRoomProvider;
	private IPassage _passage;
	private Collider _collider;
	private EnumerableDictReflector<ICharacterRoot, PassageCollisionHandler> _characterToHandler;
	private IExpeditionCharacterManager _expeditionCharacterManager;

	private void Start()
	{
		_expeditionCharacterManager = this.GetComponentInParentSafe<IExpeditionCharacterManager>();

		_passage = this.GetComponentInParentSafe<IPassage>();
		_collider = this.GetComponentSafe<Collider>();

		_characterToHandler = new EnumerableDictReflector<ICharacterRoot, PassageCollisionHandler>(Added, Deleted);

		AddReflector(Reflect);
	}

	private PassageCollisionHandler Added(ICharacterRoot root)
	{
		return new PassageCollisionHandler(_passage, _collider, root);
	}

	private void Deleted(PassageCollisionHandler handler)
	{
		handler.Dispose();
	}

	void Reflect()
	{
		_characterToHandler.Enumerate(_expeditionCharacterManager.Characters.Select(c => c.Root));
	}


	sealed class PassageCollisionHandler : IDisposable
	{
		private IPassage _passage;
		private Collider _passageCollider;
		private ICharacterRoot _character;
		private ITollEnergyOnEnterRoom _characterToll;
		private Computed<bool> _canEnterRoom;
		private Reflector _reflector;
		private Collider _characterCollider;

		public PassageCollisionHandler(IPassage passage, Collider passageCollider, ICharacterRoot character)
		{
			_passage = passage;
			_passageCollider = passageCollider;
			_character = character;
			_characterToll = _character.GetComponentInChildrenSafe<ITollEnergyOnEnterRoom>();
			_characterCollider = _character.GetComponentInChildrenSafe<Collider>();

			_canEnterRoom = new Computed<bool>(ComputeCanEnterRoom);
			_reflector = new Reflector(Reflect);
		}

		private bool ComputeCanEnterRoom()
		{
			// Rather than figuring out the rooms we're in, let's just see the cost to enter either and take the max of that.
			var costA = _characterToll.GetCostToEnterRoom(_passage.RoomA);
			var costB = _characterToll.GetCostToEnterRoom(_passage.RoomB);
			var maxCost = Mathf.Max(costA, costB);
			return _characterToll.CanAffordEntry(maxCost);

		}

		void Reflect()
		{
			var canEnterRoom = _canEnterRoom.Val;
			Physics.IgnoreCollision(_passageCollider, _characterCollider, canEnterRoom);
		}

		public void Dispose()
		{
			_canEnterRoom.Destroy();
			_reflector.Destroy();
		}
	}
}
