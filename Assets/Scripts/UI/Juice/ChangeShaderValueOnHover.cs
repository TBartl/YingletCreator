using Reactivity;
using UnityEngine;
using UnityEngine.UI;

public class ChangeShaderValueOnHover : ReactiveBehaviour
{
	static readonly int VALUE = Shader.PropertyToID("_Value");

	[SerializeField] float _maxVal = 1;
	[SerializeField] SharedEaseSettings _easeSettings;
	private Image _image;
	private Material _material;
	private IHoverable _hoverable;
	private float _originalValue;
	private Coroutine _transitionCoroutine;

	void Start()
	{
		_image = this.GetComponent<Image>();
		_material = new Material(_image.material);
		_image.material = _material;
		_hoverable = this.GetComponentInParent<IHoverable>();
		_originalValue = _material.GetFloat(VALUE);
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		float from = _material.GetFloat(VALUE);
		float to = _hoverable.Hovered.Val ? _maxVal : _originalValue;
		this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => UpdateShaderVal(Mathf.LerpUnclamped(from, to, p)));
	}

	void UpdateShaderVal(float v)
	{
		_material.SetFloat(VALUE, v);
		_image.SuperDirtyMaterial();
	}
}
