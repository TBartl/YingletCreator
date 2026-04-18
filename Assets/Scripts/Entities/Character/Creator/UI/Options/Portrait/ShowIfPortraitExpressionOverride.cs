using Reactivity;

namespace Character.Creator.UI
{
	public class ShowIfPortraitExpressionOverride : ReactiveBehaviour
	{
		void Start()
		{
			var dataRepo = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();
			AddReflector(() => this.gameObject.SetActive(dataRepo.CustomizationData.PortraitData.UseOverrideExpressions.Val));
		}
	}

}
