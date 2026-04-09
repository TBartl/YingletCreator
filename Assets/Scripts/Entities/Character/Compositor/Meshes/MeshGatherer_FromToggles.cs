using Character.Creator;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Character.Compositor
{
	public class MeshGatherer_FromToggles : MonoBehaviour, IMeshGathererMutator
	{
		private ICustomizationDataRepository _dataRepo;

		void Awake()
		{
			_dataRepo = this.GetComponentInParent<ICustomizationDataRepository>();
		}
		public void Mutate(ref ISet<MeshWithMaterial> set)
		{
			var toggles = _dataRepo.CustomizationData.ToggleData.Toggles.ToArray();
			foreach (var toggle in toggles)
			{
				foreach (var mesh in toggle.AddedMeshes)
				{
					set.Add(mesh);
				}
			}
		}
	}
}