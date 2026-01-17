using UnityEngine;
using UnityEngine.UI;

namespace Character.Creator.UI
{
    public class OffsetOnButtonClicked : MonoBehaviour
    {
        [SerializeField] Vector3 _offsetAmount;
        [SerializeField] SharedEaseSettings _easeSettings;
        private Vector3 _originalPos;
        private RectTransform _rectTransform;
        private Button _button;
        private Coroutine _transitionCoroutine;

        private void Awake()
        {
            _rectTransform = this.GetComponent<RectTransform>();
            _originalPos = _rectTransform.anchoredPosition3D;
            _button = this.GetComponentInParent<Button>();
            _button.onClick.AddListener(Button_OnClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(Button_OnClick);
        }

        private void Button_OnClick()
        {
            Vector3 from = _originalPos + _offsetAmount;
            Vector3 to = _originalPos;
            this.StartEaseCoroutine(ref _transitionCoroutine, _easeSettings, p => _rectTransform.anchoredPosition3D = Vector3.LerpUnclamped(from, to, p));
        }
    }
}