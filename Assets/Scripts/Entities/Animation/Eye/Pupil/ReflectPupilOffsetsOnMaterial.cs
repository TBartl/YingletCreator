using System.Linq;
using UnityEngine;

public interface IPupilOffsetMutator
{
	PupilOffsets Mutate(PupilOffsets input);
	bool enabled { get; }
}

public class ReflectPupilOffsetsOnMaterial : MonoBehaviour
{
	private IEyeGatherer _eyeGatherer;
	private IPupilOffsetMutator[] _offsetMutators;
	static readonly int PUPIL_OFFSET_X_PROP_ID = Shader.PropertyToID("_PupilOffsetX");
	static readonly int PUPIL_OFFSET_Y_PROP_ID = Shader.PropertyToID("_PupilOffsetY");

	void Awake()
	{
		_eyeGatherer = this.GetComponentInParent<IEyeGatherer>();
		_offsetMutators = this.GetComponentsSafe<IPupilOffsetMutator>().Where(c => c.enabled).ToArray();
		Update(); // Call once to rotate immediately. Needed for things like snapshotting
	}

	void Update()
	{
		PupilOffsets offsets = new PupilOffsets(0, 0, 0);
		foreach (var offsetProvider in _offsetMutators)
		{
			offsets = offsetProvider.Mutate(offsets);
		}
		SetMaterial(_eyeGatherer.EyeMaterials[0], offsets.GetLeftEyeOffsets());
		SetMaterial(_eyeGatherer.EyeMaterials[1], offsets.GetRightEyeOffsets());

		void SetMaterial(Material eyeMaterial, Vector2 offset)
		{
			eyeMaterial.SetFloat(PUPIL_OFFSET_X_PROP_ID, offset.x);
			eyeMaterial.SetFloat(PUPIL_OFFSET_Y_PROP_ID, offset.y);
		}
	}
}
