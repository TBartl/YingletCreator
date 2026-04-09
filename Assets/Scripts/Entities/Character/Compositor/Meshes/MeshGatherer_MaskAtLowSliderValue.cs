using Character.Creator;
using Character.Data;
using Reactivity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Character.Compositor
{

	public class MeshGatherer_MaskAtLowSliderValue : ReactiveBehaviour, IMeshGathererMutator
	{
		[SerializeField] AssetReferenceT<CharacterElementTag> _toRemoveReference;
		[SerializeField] AssetReferenceT<CharacterSliderId> _sliderReference;
		[SerializeField] float _minimumValue;

		ICustomizationDataRepository _dataRepository;
		private Computed<bool> _constrain;

		private void Awake()
		{
			_dataRepository = GetComponentInParent<ICustomizationDataRepository>();
			_constrain = CreateComputed(ComputeConstrain);
		}

		bool ComputeConstrain()
		{
			var sliderValue = _dataRepository.GetSliderValue(_sliderReference.LoadSync());
			return sliderValue < 0.01f;
		}

		public void Mutate(ref ISet<MeshWithMaterial> meshes)
		{
			if (_constrain.Val)
			{
				var toRemove = meshes.Where(m => m.Tags != null && m.Tags.Contains(_toRemoveReference.LoadSync())).ToList();
				foreach (var m in toRemove)
				{
					meshes.Remove(m);
				}
			}
		}
	}
}
