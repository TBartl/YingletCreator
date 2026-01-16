using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTransitioner : MonoBehaviour
{
	[SerializeField] private SoundEffect _soundEffect;
	[SerializeField] Vector2 _transitionRange = new Vector2(0, 1);

	private Coroutine _coroutine;
	private IScreenTransitionManager _screenTransitionManager;
	private IAudioPlayer _audioPlayer;
	private Image _image;

	IEnumerator Start()
	{
		_screenTransitionManager = Singletons.GetSingleton<IScreenTransitionManager>();
		_audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
		_image = this.GetComponent<Image>();
		_image.material = new Material(_image.material); // Instance so we can modify it

		_image.enabled = true;
		SetVal(1);

		_screenTransitionManager.OnStartTransitionToOpaque += TransitionToOpaque;

		yield return null;
		TransitionToClear();
	}

	private void OnDestroy()
	{
		_screenTransitionManager.OnStartTransitionToOpaque -= TransitionToOpaque;
	}

	void TransitionToClear()
	{
		_image.enabled = true;
		_image.material.SetFloat("_X", 1);
		_audioPlayer.Play(_soundEffect);

		float from = _transitionRange.y;
		float to = _transitionRange.x;

		this.StartEaseCoroutine(ref _coroutine, _screenTransitionManager.EaseSettings, Apply, OnComplete);
		void Apply(float p)
		{
			SetVal(Mathf.Lerp(from, to, p));
		}
		void OnComplete()
		{
			_image.enabled = false;
		}
	}
	void TransitionToOpaque()
	{
		_image.enabled = true;
		_image.material.SetFloat("_X", -1);
		_audioPlayer.Play(_soundEffect);

		float from = _transitionRange.x;
		float to = _transitionRange.y;


		this.StartEaseCoroutine(ref _coroutine, _screenTransitionManager.EaseSettings, Apply);
		void Apply(float p)
		{
			SetVal(Mathf.Lerp(from, to, p));
		}
	}

	private void SetVal(float v)
	{
		_image.material.SetFloat("_Value", v);
	}

}
