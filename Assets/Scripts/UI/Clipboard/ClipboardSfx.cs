using UnityEngine;

namespace Character.Creator.UI
{
    public sealed class ClipboardSfx : MonoBehaviour
    {
        [SerializeField] private SoundEffect _changePage;
        private IAudioPlayer _audioPlayer;
        private IClipboardSelection _selection;

        private void Awake()
        {
            _audioPlayer = Singletons.GetSingleton<IAudioPlayer>();
            _selection = this.GetComponent<IClipboardSelection>();

        }

        private void Start()
        {
            _selection.Selection.OnChanged += Selection_OnChanged;
        }

        private void OnDestroy()
        {
            _selection.Selection.OnChanged -= Selection_OnChanged;
        }
        private void Selection_OnChanged(ClipboardSelectionType from, ClipboardSelectionType to)
        {
            _audioPlayer.Play(_changePage);
        }
    }
}