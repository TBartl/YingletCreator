using System.Collections.Generic;
using UnityEngine;

namespace Character.Compositor
{
	public class MeshGatherer_FromSnapshotData : MonoBehaviour, IMeshGathererMutator
	{
		private SnapshotterDataRepository _dataRepo;

		void Awake()
		{
			_dataRepo = this.GetComponentInParent<SnapshotterDataRepository>();
		}
		public void Mutate(ref ISet<MeshWithMaterial> set)
		{
			// Not used in expedition snapshotter right now
			if (_dataRepo == null) return;

			var pose = _dataRepo.Pose;
			if (pose == null) return;
			if (pose.Props == null) return;
			foreach (var prop in pose.Props)
			{
				set.Add(prop);
			}
		}
	}
}