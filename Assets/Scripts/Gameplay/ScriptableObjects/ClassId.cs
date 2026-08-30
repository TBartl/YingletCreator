using UnityEngine;

[System.Serializable]
public struct StatAllocation
{
	[SerializeField] public AssetReferenceT<StatId> Stat;
	[SerializeField] public int Value;
}

/// <summary>
/// Controls the preview portrait generated for characters
/// </summary>
[CreateAssetMenu(fileName = "Class", menuName = "Scriptable Objects/Gameplay/Class")]
public class ClassId : ScriptableObject, IHasUniqueAssetId, IOrderableScriptableObject
{
	[SerializeField, HideInInspector] string _uniqueAssetId;
	public string UniqueAssetID { get => _uniqueAssetId; set => _uniqueAssetId = value; }

	[SerializeField] Sprite _icon;
	public Sprite Icon => _icon;

	[field: SerializeField] public Color TextColor { get; private set; }

	[SerializeField] Material _uiOverlayMaterial;
	public Material UiOverlayMaterial => _uiOverlayMaterial;

	[field: SerializeField] public StatAllocation[] Stats { get; private set; }

	[SerializeField] int _orderIndex;
	public int OrderIndex => _orderIndex;

	private string _cachedColorHtml;
	public string TextColorHtml
	{
		get
		{
			if (string.IsNullOrWhiteSpace(_cachedColorHtml))
			{
				_cachedColorHtml = "#" + ColorUtility.ToHtmlStringRGB(TextColor);
			}
			return _cachedColorHtml;
		}
	}
}
