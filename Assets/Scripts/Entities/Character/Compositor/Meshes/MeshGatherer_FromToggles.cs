using System.Collections.Generic;
using UnityEngine;

namespace Character.Compositor
{
	public class MeshGatherer_FromToggles : MonoBehaviour, IMeshGathererMutator
	{
		private ICharacterToggleProvider _toggleProvider;

		void Awake()
		{
			_toggleProvider = this.GetComponentInParentSafe<ICharacterToggleProvider>();
		}
		public void Mutate(ref ISet<MeshWithMaterial> set)
		{
			var toggles = _toggleProvider.Toggles;
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