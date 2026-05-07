using Reactivity;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Whenever this energy blip is added or removed, this class will flash the image with a color,
/// transition to transparent, and then set itself inactive.
/// </summary>
public class OverlayEnergyBlipOnChanged : ReactiveBehaviour
{
	[SerializeField] SharedEaseSettings _easeSettings;
	[SerializeField] Color _addedColor;
	[SerializeField] Color _removedColor;
	private ICommonGameplayAssets _assets;
	private Image _image;
	private Coroutine _transitionCoroutine;
	private IPartyMemberHUDReference _reference;
	private Computed<ICharacterResources> _characterResources;
	private Computed<int> _resourceCount;
	private int _parentSiblingIndex;

	private void Start()
	{
		_assets = Singletons.GetSingleton<ICommonGameplayAssets>();
		_image = this.GetComponentSafe<Image>();
		_parentSiblingIndex = this.transform.parent.GetSiblingIndex();
		_reference = this.GetComponentInParentSafe<IPartyMemberHUDReference>();
		_characterResources = CreateComputed(ComputeCharacterResources);
		_resourceCount = CreateComputed(() => _characterResources.Val?.GetResource(_assets.ResourceEnergy) ?? 0);
		_resourceCount.OnChanged += OnResourceCountChanged;
		this.gameObject.SetActive(false);
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		if (_resourceCount != null)
		{
			_resourceCount.OnChanged -= OnResourceCountChanged;
		}
	}

	private ICharacterResources ComputeCharacterResources()
	{
		var character = _reference.Character;
		if (character == null) return null;
		return character.GetComponentInChildrenSafe<ICharacterResources>();
	}

	private void OnResourceCountChanged(int fromVal, int toVal)
	{
		// Only react if this blip's index is being added or removed
		bool wasActive = fromVal > _parentSiblingIndex;
		bool isActive = toVal > _parentSiblingIndex;

		if (wasActive == isActive)
		{
			return;
		}

		var fromColor = isActive ? _addedColor : _removedColor;
		var toColor = new Color(fromColor.r, fromColor.g, fromColor.b, 0);
		this.gameObject.SetActive(true);
		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, Apply, OnComplete);

		void Apply(float p)
		{
			Color targetColor = Color.Lerp(fromColor, toColor, p);
			_image.color = targetColor;
		}
		void OnComplete()
		{
			this.gameObject.SetActive(false);
		}

	}
}
