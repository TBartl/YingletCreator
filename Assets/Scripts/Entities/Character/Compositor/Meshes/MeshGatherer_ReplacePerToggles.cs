using Character.Data;
using Reactivity;
using System.Collections.Generic;

namespace Character.Compositor
{
	public class MeshGatherer_ReplacePerToggles : ReactiveBehaviour, IMeshGathererMutator
	{
		private ICharacterToggleProvider _toggleProvider;
		EnumerableSetReflector<IToggleReplacesMesh> _computedSet;


		private void Awake()
		{
			_toggleProvider = this.GetComponentInParentSafe<ICharacterToggleProvider>();
			_computedSet = new EnumerableSetReflector<IToggleReplacesMesh>();
			AddReflector(ReflectSet);
		}

		private void ReflectSet()
		{
			var newSet = new HashSet<IToggleReplacesMesh>();
			var toggles = _toggleProvider.Toggles;
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