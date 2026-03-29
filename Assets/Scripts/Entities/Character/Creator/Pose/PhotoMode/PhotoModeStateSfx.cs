using UnityEngine;

namespace Character.Creator.UI
{
    public class PhotoModeStateSfx : MonoBehaviour
    {
        [SerializeField] SoundEffect _soundEffect;

        private IAudioPlayer _audioPlayer;
        private IPhotoModeChecker _photoModeState;

        private void Awake()
        {
            _audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
            _photoModeState = this.GetComponentInParent<IPhotoModeChecker>();
            _photoModeState.IsInPhotoMode.OnChanged += VisibilityControl_OnChanged;
        }

        private void OnDestroy()
        {
            _photoModeState.IsInPhotoMode.OnChanged -= VisibilityControl_OnChanged;
        }
        private void VisibilityControl_OnChanged(bool arg1, bool arg2)
        {
            _audioPlayer.Play(_soundEffect);
        }
    }
}