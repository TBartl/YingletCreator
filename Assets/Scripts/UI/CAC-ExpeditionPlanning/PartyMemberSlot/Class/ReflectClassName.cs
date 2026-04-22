using Reactivity;
using TMPro;

public class ReflectClassName : ReactiveBehaviour
{
	private IClassReference _reference;
	private TMP_Text _text;

	void Start()
	{
		_reference = GetComponentInParent<IClassReference>();
		_text = GetComponent<TMP_Text>();
		AddReflector(Reflect);
	}

	private void Reflect()
	{
		var classId = _reference.Class;
		if (classId == null) return;
		_text.text = classId.name;
	}
}
