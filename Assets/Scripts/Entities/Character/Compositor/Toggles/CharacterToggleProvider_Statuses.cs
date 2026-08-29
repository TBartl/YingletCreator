using Character.Creator;
using Character.Data;
using Reactivity;
using System.Collections.Generic;

namespace Assets.Scripts.Entities.Character.Compositor.Toggles
{
	internal class CharacterToggleProvider_Statuses : ReactiveBehaviour, ICharacterToggleProvider, IInitializable
	{
		public IReadOnlySet<CharacterToggleId> Toggles => _observableToggleSet;

		ObservableUpdateableSet<CharacterToggleId> _observableToggleSet = new();
		private ICharacterStatuses _statuses;

		public void Initialize()
		{
			_statuses = this.GetCharacterRootComponent<ICharacterStatuses>();

			AddReflector(Reflect);
		}

		private void Reflect()
		{
			var toggles = new HashSet<CharacterToggleId>();
			foreach (var status in _statuses.Statuses)
			{
				foreach (var statusEffect in status.StatusEffects)
				{
					if (statusEffect is StatusEffect_AddCharacterToggle addToggle)
					{
						toggles.FlipToggle(addToggle.Toggle);
					}
				}
			}
			_observableToggleSet.Update(toggles);
		}
	}
}
