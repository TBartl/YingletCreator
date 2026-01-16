using Reactivity;
using UnityEngine;

namespace Character.Creator.UI
{
	public interface IReactiveOffsetMutator
	{
		Vector3 MutateOffset(Vector3 currentOffset);
	}


	public class ReactiveOffset : ReactiveBehaviour
	{
		[SerializeField] SharedEaseSettings _easeSettings;
		private IReactiveOffsetMutator[] _mutators;
		private Vector3 _originalPos;
		private Coroutine _transitionCoroutine;

		Computed<Vector3> _offsetTarget;


		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			_mutators = this.GetComponents<IReactiveOffsetMutator>();

			_originalPos = this.transform.localPosition;

			_offsetTarget = CreateComputed(ComputePhotoAndMenuState);

			_offsetTarget.OnChanged += OnOffsetTargetChanged;
			this.transform.localPosition = _originalPos + _offsetTarget.Val;
		}

		private Vector3 ComputePhotoAndMenuState()
		{
			var offset = Vector3.zero;
			foreach (var mutator in _mutators)
			{
				offset = mutator.MutateOffset(offset);
			}
			return offset;
		}

		private void OnOffsetTargetChanged(Vector3 fromOffset, Vector3 toOffset)
		{
			var fromPos = this.transform.localPosition;
			var toPos = _originalPos + toOffset;
			this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => this.transform.localPosition = Vector3.LerpUnclamped(fromPos, toPos, p));
		}
	}
}
