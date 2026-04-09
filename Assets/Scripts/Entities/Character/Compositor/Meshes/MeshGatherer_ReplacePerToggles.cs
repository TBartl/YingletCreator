using Character.Creator;
using Character.Data;
using Reactivity;
using System.Collections.Generic;
using System.Linq;

namespace Character.Compositor
{
	public class MeshGatherer_ReplacePerToggles : ReactiveBehaviour, IMeshGathererMutator
	{
		ICustomizationDataRepository _dataRepository;
		EnumerableSetReflector<IToggleReplacesMesh> _computedSet;


		private void Awake()
		{
			_dataRepository = GetComponentInParent<ICustomizationDataRepository>();
			_computedSet = new EnumerableSetReflector<IToggleReplacesMesh>();
			AddReflector(ReflectSet);
		}

		private void ReflectSet()
		{
			var newSet = new HashSet<IToggleReplacesMesh>();
			var toggles = _dataRepository.CustomizationData.ToggleData.Toggles.ToArray();
			foreach (var toggle in toggles)
			{
				foreach (var component in toggle.Components)
				{
					if (component is IToggleReplacesMesh toggleReplacesMesh)
					{
						newSet.Add(toggleReplacesMesh);
					}
				}
			}

			_computedSet.Enumerate(newSet);
		}

		public void Mutate(ref ISet<MeshWithMaterial> set)
		{
			foreach (var replacer in _computedSet.Items)
			{
				if (set.Contains(replacer.ToReplace))
				{
					set.Remove(replacer.ToReplace);
					set.Add(replacer.Replacement);
				}
			}
		}
	}
}