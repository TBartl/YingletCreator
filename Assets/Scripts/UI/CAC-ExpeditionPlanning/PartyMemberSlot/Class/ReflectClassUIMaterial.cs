using Reactivity;
using UnityEngine.UI;

public class ReflectClassUIMaterial : ReactiveBehaviour
{
	private IClassReference _reference;
	private Image _image;

	void Start()
	{
		_reference = GetComponentInParent<IClassReference>();
		_image = GetComponent<Image>();
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		var classId = _reference.Class;
		if (classId == null) return;
		_image.material = classId.UiOverlayMaterial;
		_image.SuperDirtyMaterial();
	}
}
