using Character.Creator;
using UnityEngine;

public interface IPoseYingDataRepository
{
	CachedYingletReference Reference { get; }
	void Setup(CachedYingletReference reference);
	IYingPoseData YingPoseData { get; }
}

public class PoseYingDataRepository : MonoBehaviour, IPoseYingDataRepository, ICustomizationDataRepository
{
	public CachedYingletReference Reference { get; private set; }
	public ObservableCustomizationData CustomizationData { get; private set; }

	public IYingPoseData YingPoseData { get; private set; }

	public void Setup(CachedYingletReference reference)
	{
		var resourceLoader = Singletons.GetSingleton<ICompositeResourceLoader>();
		Reference = reference;
		CustomizationData = new ObservableCustomizationData(reference.CachedData, resourceLoader);

		var poseData = this.GetComponentInParent<IPoseData>();
		YingPoseData = poseData.Data[reference];
	}
}
