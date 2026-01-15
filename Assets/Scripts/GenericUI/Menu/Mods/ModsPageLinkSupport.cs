using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModsPageLinkSupport : MonoBehaviour, IPointerClickHandler
{
	private TMP_Text _text;
	private ISaveFolderProvider _folderProvider;

	void Awake()
	{
		_text = GetComponent<TMP_Text>();
		_folderProvider = Singletons.GetSingleton<ISaveFolderProvider>();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		int linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, eventData.position, null);

		if (linkIndex == -1)
		{
			return;

		}
		TMP_LinkInfo linkInfo = _text.textInfo.linkInfo[linkIndex];
		string linkId = linkInfo.GetLinkID();
		switch (linkId)
		{
			case "github":
				Application.OpenURL("https://github.com/TBartl/YingletCreator");
				break;
			case "folder":
				Character.Creator.UI.OpenFolderOnButtonClick.OpenFolder(_folderProvider.ModsFolderPath);
				break;
			case "workshop":
				Application.OpenURL("https://steamcommunity.com/app/3954540/workshop/");
				break;
			default:
				throw new Exception("Unexpected link: " + linkId);
		}
	}
}
