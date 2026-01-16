using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HideOnScrollExtent : MonoBehaviour
{
    [SerializeField] bool _topExtent;
    [SerializeField] SharedEaseSettings _easeSettings;

    private ScrollRect _scrollRect;
    private Image _image;

    private Color _startColor;
    private Coroutine _easeCoroutine;

    void Start()
    {
        _image = GetComponent<Image>();
        _scrollRect = GetComponentInParent<ScrollRect>();

        _startColor = _image.color;
        _scrollRect.onValueChanged.AddListener(CheckScrollbarExtent);
        StartCoroutine(SetAfterAFrame());
    }
    IEnumerator SetAfterAFrame()
    {
        // Bit of a hack, but for whatever reason the scroll rect isn't ready to give us proper values on frame 0
        yield return null;
        UpdateGraphic(_scrollRect.normalizedPosition, false);
    }

    void OnDestroy()
    {
        _scrollRect?.onValueChanged.RemoveListener(CheckScrollbarExtent);
    }

    void CheckScrollbarExtent(Vector2 vec)
    {
        UpdateGraphic(vec, true);
    }

    void UpdateGraphic(Vector2 vec, bool ease)
    {

        bool shouldBeActive = CheckShouldBeActive(vec.y);
        Color targetColor = shouldBeActive ? _startColor : new Color(_startColor.r, _startColor.g, _startColor.b, 0);
        if (ease)
        {
            Color startColor = _image.color;
            this.StartEaseCoroutine(ref _easeCoroutine, _easeSettings, p => _image.color = Color.Lerp(startColor, targetColor, p));
        }
        else
        {
            _image.color = targetColor;
        }
    }

    bool CheckShouldBeActive(float value)
    {
        if (!CheckIsScrollingNeededY())
        {
            return false;
        }
        bool atExtent = _topExtent
            ? value >= 0.999f
            : value <= 0.001f;
        return !atExtent;
    }

    bool CheckIsScrollingNeededY()
    {
        float viewportHeight = _scrollRect.viewport.rect.height;
        float contentHeight = _scrollRect.content.rect.height;

        return contentHeight > viewportHeight;
    }
}
