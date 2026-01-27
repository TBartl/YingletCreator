using System.Collections;
using UnityEngine;

public class ClipboardFreeFallParent : MonoBehaviour
{
	private Animation _animation;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_animation = this.GetComponent<Animation>();
		StartCoroutine(FreeFall());
	}
	IEnumerator FreeFall()
	{
		_animation.Play();
		yield return new WaitForSeconds(_animation.clip.length);
		_animation.Stop();

		EndFreeFall();
	}

	private void OnDisable()
	{
		if (!gameObject.scene.isLoaded) return; // Disable only

		EndFreeFall();
	}

	void EndFreeFall()
	{
		// Reparent everything
		foreach (Transform child in this.transform)
		{
			child.SetParent(this.transform.parent);
			child.gameObject.SetActive(false);
		}

		Destroy(this.gameObject);
	}
}
