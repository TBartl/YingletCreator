using Character.Creator;
using Reactivity;

internal class ShowCustomPronounListIfApplicable : ReactiveBehaviour
{
	private ICustomizationSelectedDataRepository _dataRepo;

	private void Start()
	{
		_dataRepo = Singletons.GetSingleton<ICustomizationSelectedDataRepository>();

		AddReflector(Reflect);
	}

	private void Reflect()
	{
		var pronouns = _dataRepo.CustomizationData?.GenderData?.Pronouns?.Val;
		if (pronouns == null)
		{
			this.gameObject.SetActive(false);
		}

		this.gameObject.SetActive(pronouns == CharacterPronouns.Custom);
	}
}
