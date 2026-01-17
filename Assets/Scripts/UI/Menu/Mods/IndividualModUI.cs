using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IndividualModUI : MonoBehaviour
{
	[SerializeField] TMP_Text _titleText;
	[SerializeField] TMP_Text _descriptionText;
	[SerializeField] TMP_Text _resourceText;
	[SerializeField] Image _icon;
	public void Setup(ModDefinition mod)
	{
		_titleText.text = $"{mod.Title} by {mod.Author}";
		_descriptionText.text = mod.ShortDescription;
		_icon.sprite = mod.Icon;
		_resourceText.text = BuildResourceString(mod);
	}

	private string BuildResourceString(ModDefinition mod)
	{
		var counts = mod.CountAssetTypes();
		List<string> items = new();
		if (counts.Presets > 0) items.Add(NumberPrefixed(counts.Presets, "preset"));
		if (counts.Toggles > 0) items.Add(NumberPrefixed(counts.Toggles, "toggle"));
		if (counts.Poses > 0) items.Add(NumberPrefixed(counts.Poses, "pose"));
		return BuildEnglishList(items);
	}

	string NumberPrefixed(int count, string name)
	{
		var str = $"{count} {name}";
		if (count > 1) str += "s";
		return str;
	}

	string BuildEnglishList(List<string> items)
	{
		int count = items.Count;
		if (count == 0) return "";
		if (count == 1) return items.Single();

		string allButLast = string.Join(", ", items.Take(count - 1));
		return $"{allButLast} and {items.Last()}";
	}
}
