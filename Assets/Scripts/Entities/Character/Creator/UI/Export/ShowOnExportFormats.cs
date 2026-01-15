
using Character.Creator.UI;
using Reactivity;
using System.Linq;
using UnityEngine;

internal sealed class ShowOnExportFormats : ReactiveBehaviour
{
	[SerializeField] ExportModelFormat[] FormatsToShowFor;

	private void Start()
	{
		var page = this.GetComponentInParent<Page>();
		var formatDropdown = page.GetComponentInChildren<ExportModelFormatDropdown>();
		AddReflector(() => this.gameObject.SetActive(FormatsToShowFor.Contains(formatDropdown.CurrentFormat)));
	}
}
