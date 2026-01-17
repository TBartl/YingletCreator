using System;
using System.Text;
using UnityEngine;

public class ReflectThirdPartyAssetsText : MonoBehaviour
{
	[SerializeField] TextAsset _textAsset;
	void Start()
	{
		var text = this.GetComponent<TMPro.TMP_Text>();

		// License text is kinda noisy, cut it off
		text.text = RemoveLicenseFromLines(_textAsset.text);
	}

	public static string RemoveLicenseFromLines(string input)
	{
		var lines = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
		var sb = new StringBuilder();

		foreach (var line in lines)
		{
			int index = line.IndexOf("-- License:", StringComparison.Ordinal);
			if (index >= 0)
			{
				sb.AppendLine(line.Substring(0, index).TrimEnd());
			}
			else
			{
				sb.AppendLine(line);
			}
		}

		return sb.ToString();
	}
}
