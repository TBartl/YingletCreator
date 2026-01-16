using UnityEngine;

public class ReflectAppVersionNumber : MonoBehaviour
{
	const string VersionPrefix = "Version ";
	void Start()
	{
		var text = this.GetComponent<TMPro.TMP_Text>();
		text.text = VersionPrefix + Application.version;
	}
}
