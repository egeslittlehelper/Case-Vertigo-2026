using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoCase.UI
{
    /// <summary>
    /// Displays one wheel slice — icon and amount text. Positioned by WheelView at build time.
    /// </summary>
    public sealed class WheelSliceView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _amountText;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Transform _animTarget;

        private Tween _highlightTween;

        public void Set(Sprite icon, string amountText)
        {
            if (_icon != null)
            {
                _icon.sprite = icon;
                _icon.enabled = icon != null;
            }

            if (_amountText != null)
                _amountText.text = amountText;
        }

        public void Highlight(float punchStrength, float punchDuration)
        {
            Transform target = _animTarget != null ? _animTarget : transform;
            _highlightTween?.Kill();
            _highlightTween = target
                .DOPunchScale(Vector3.one * punchStrength, punchDuration, 6, 0.5f)
                .SetLink(gameObject);
        }

        public void SetDim(float alpha, float duration)
        {
            EnsureCanvasGroup();
            if (_canvasGroup == null) return;
            _canvasGroup.DOFade(alpha, duration).SetLink(gameObject);
        }

        public void ResetVisual()
        {
            _highlightTween?.Kill();
            _highlightTween = null;

            Transform target = _animTarget != null ? _animTarget : transform;
            target.localScale = Vector3.one;

            if (_canvasGroup != null)
                _canvasGroup.alpha = 1f;
        }

        private void EnsureCanvasGroup()
        {
            if (_canvasGroup != null) return;
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        private void OnValidate()
        {
            if (_icon == null)
                _icon = ChildReference.FindChild<Image>(transform, "ui_image_slice_icon_value");
            if (_amountText == null)
                _amountText = ChildReference.FindChild<TMP_Text>(transform, "ui_text_slice_amount_value");
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            if (_animTarget == null)
            {
                foreach (Transform child in transform)
                {
                    if (child.name.StartsWith("ui_anim_"))
                    {
                        _animTarget = child;
                        break;
                    }
                }
            }
        }

        private void OnDisable()
        {
            _highlightTween?.Kill();
            _highlightTween = null;
        }
    }
}
