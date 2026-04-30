using Reactivity;
using System.Collections;
using UnityEngine;

namespace Character.Creator.UI
{
	/// <summary>
	/// We want more than just a Vector3: we want to know if this item is considered on-screen or not so we can optimize
	/// </summary>
	[System.Serializable]
	public sealed class ReactiveOffsetValues
	{
		[SerializeField] bool _onScreen;
		[SerializeField] Vector3 _offset;

		/// <summary>
		/// True if the this item is considered "onscreen" with this offset
		/// If false, we will disable it for optimization when we've reached it
		/// </summary>
		public bool OnScreen => _onScreen;
		public Vector3 Offset => _offset;
	}

	public interface IReactiveOffsetMutator
	{
		ReactiveOffsetValues MutateOffset(ReactiveOffsetValues currentOffset);
	}

	public class ReactiveOffset : ReactiveBehaviour
	{
		[SerializeField] SharedEaseSettings _easeSettings;
		[SerializeField] ReactiveOffsetValues _baseOffset;

		// Some things, like the Yinglet Creator clipboard, have some work to do right away while we're idling on the title screen
		// Get that going immediately
		[SerializeField] bool _forceEnabledOnStart = false;

		private IReactiveOffsetMutator[] _mutators;
		private Vector3 _originalPos;
		private Coroutine _transitionCoroutine;

		Computed<ReactiveOffsetValues> _offsetTarget;


		// Start is called once before the first execution of Update after the MonoBehaviour is created
		IEnumerator Start()
		{
			_mutators = this.GetComponentsSafe<IReactiveOffsetMutator>();

			_originalPos = this.transform.localPosition;

			_offsetTarget = CreateComputed(ComputeOffset);

			_offsetTarget.OnChanged += OnOffsetTargetChanged;
			this.transform.localPosition = _originalPos + _offsetTarget.Val.Offset;
			if (_offsetTarget.Val.OnScreen == false)
			{
				if (_forceEnabledOnStart)
				{
					yield return null;
					yield return null;
				}
				this.gameObject.SetActive(false);
			}
		}

		private ReactiveOffsetValues ComputeOffset()
		{
			var offset = _baseOffset;
			foreach (var mutator in _mutators)
			{
				offset = mutator.MutateOffset(offset);
			}
			return offset;
		}

		private void OnOffsetTargetChanged(ReactiveOffsetValues fromOffset, ReactiveOffsetValues toOffset)
		{
			if (toOffset.OnScreen)
			{
				this.gameObject.SetActive(true);
			}

			var fromPos = this.transform.localPosition;
			var toPos = _originalPos + toOffset.Offset;
			this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => this.transform.localPosition = Vector3.LerpUnclamped(fromPos, toPos, p), OnComplete);

			void OnComplete()
			{
				if (toOffset.OnScreen == false)
				{
					this.gameObject.SetActive(false);
				}
			}
		}
	}
}
