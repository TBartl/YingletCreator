using UnityEngine;

public class ReflectStatLabel : MonoBehaviour
{
	private void Start()
	{
		var reference = this.GetComponentInParentSafe<IStatReference>();
		var text = this.GetComponentSafe<TMPro.TMP_Text>();

		text.text = reference.Stat.ShortName;
		text.color = reference.Stat.Color;
	}
}
