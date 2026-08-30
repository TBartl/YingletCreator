using UnityEngine;

namespace Snapshotter
{
	public interface ISnapshotterReferences
	{
		GameObject YingletDataPrefab { get; }
		GameObject YingletExpeditionPrefab { get; }
		int LayerIndex { get; }
		int LayerMask { get; }

		/// <summary>
		/// Always a 1:1 ratio for now
		/// </summary>
		int SizeInPixels { get; }

		public float YScaleToCamOffset { get; }

		/// <summary>
		/// Mostly used for testing
		/// </summary>
		bool CleanupObjects { get; }
	}

	[CreateAssetMenu(fileName = "SnapshotterReferences", menuName = "Scriptable Objects/Misc/SnapshotterReferences")]
	public sealed class SnapshotterReferences : ScriptableObject, ISnapshotterReferences
	{
		[SerializeField] GameObject _yingletDataPrefab;
		[SerializeField] GameObject _yingletExpeditionPrefab;
		[SerializeField] int _layerIndex;
		[SerializeField] int _sizeInPixels;
		[SerializeField] bool _cleanupObject;
		[SerializeField] float _yScaleToCamOffset = 1;

		public GameObject YingletDataPrefab => _yingletDataPrefab;
		public GameObject YingletExpeditionPrefab => _yingletExpeditionPrefab;
		public int LayerIndex => _layerIndex;
		public int LayerMask => 1 << _layerIndex;
		public int SizeInPixels => _sizeInPixels;
		public float YScaleToCamOffset => _yScaleToCamOffset;
		public bool CleanupObjects => _cleanupObject;
	}
}
