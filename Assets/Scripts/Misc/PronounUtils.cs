using Character.Creator;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public enum CharacterPronouns
{
	TheyThem = 0, // Displayed third in practice, but set as 0 for defaulting purposes
	HeHim = 1,
	SheHer = 2,
	ZheyZhem = 3,
	Custom = 100
}

public static class PronounUtils
{
	private static readonly Dictionary<CharacterPronouns, string[]> PronounSets = new()
	{
		[CharacterPronouns.HeHim] = new[]{
			"he",
			"him",
			"his",
			"his",
			"himself",
		},

		[CharacterPronouns.SheHer] = new[]
		{
			"she",
			"her",
			"her",
			"hers",
			"herself",
		},

		[CharacterPronouns.TheyThem] = new[]
		{
			"they",
			"them",
			"their",
			"theirs",
			"themself",
		},

		[CharacterPronouns.ZheyZhem] = new[]
		{
			"zhey",
			"zhem",
			"zheir",
			"zheirs",
			"zhemself",
		},
	};

	private static readonly Regex PronounRegex = new(
		@"\{(they|them|their|theirs|themself)\}",
		RegexOptions.IgnoreCase | RegexOptions.Compiled);

	/// <summary>
	/// Replaces pronoun placeholders in the given text.
	/// Supports: {They} {Them} {Their} {Theirs} {Themself}
	/// and preserves capitalization.
	/// </summary>
	public static string ReplacePronouns(this string text, ObservableCustomizationGenderData genderData)
	{
		if (string.IsNullOrEmpty(text))
			return text;

		var pronouns = genderData.Pronouns.Val;
		string[] set;
		if (pronouns == CharacterPronouns.Custom)
		{
			int count = 5;
			set = new string[count];
			for (int i = 0; i < count; i++)
			{
				set[i] = genderData.CustomPronouns.ElementAtOrDefault(i) ?? PronounSets[CharacterPronouns.TheyThem][i];
			}
		}
		else
		{
			bool success = PronounSets.TryGetValue(pronouns, out set);
			if (!success)
			{
				Debug.LogWarning($"Pronoun set for {pronouns} not found..");
				return text;
			}
		}

		return PronounRegex.Replace(text, match =>
		{
			string matched = match.Groups[1].Value;

			int index = matched.ToLowerInvariant() switch
			{
				"they" => 0,
				"them" => 1,
				"their" => 2,
				"theirs" => 3,
				"themself" => 4,
				_ => -1
			};

			return ApplyCapitalization(set[index], matched);
		});
	}

	private static string ApplyCapitalization(string replacement, string original)
	{
		if (original == original.ToUpperInvariant())
			return replacement.ToUpperInvariant();

		if (char.IsUpper(original[0]))
			return char.ToUpperInvariant(replacement[0]) + replacement[1..];

		return replacement;
	}
}