using Reactivity;
using System.Linq;

internal class MirrorAnimationOnPose : ReactiveBehaviour
{
	private IPoseYingDataRepository _dataRepo;
	private IMirrorAnimationJobBinder _mirrorBinder;

	private void Start()
	{
		_dataRepo = this.GetComponentSafe<IPoseYingDataRepository>();
		_mirrorBinder = this.GetComponentSafe<IMirrorAnimationJobBinder>();
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		bool mirror = _dataRepo.YingPoseData.Mirror;
		if (mirror && _dataRepo.YingPoseData.Pose.Props.Any())
		{
			mirror = false;
		}
		_mirrorBinder.SetMirror(mirror);
	}
}
