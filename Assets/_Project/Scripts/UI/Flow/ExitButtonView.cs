using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoCase.UI
{
    /// <summary>
    /// Exit/cash-out button — always visible, dims and disables when exit is not allowed.
    /// </summary>
    public sealed class ExitButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Animation")]
        [SerializeField] private float _fadeDuration = 0.25f;
        [SerializeField] [Range(0.2f, 0.6f)] private float _disabledAlpha = 0.35f;

        private Tween _fadeTween;
        private bool _interactable = true;

        public event Action Clicked;

        public void SetVisible(bool visible, bool animate)
        {
            if (_interactable == visible) return;
            _interactable = visible;

            _fadeTween?.Kill();

            float targetAlpha = visible ? 1f : _disabledAlpha;

            if (_canvasGroup != null)
            {
                _canvasGroup.interactable = visible;
                _canvasGroup.blocksRaycasts = visible;
            }

            if (!animate)
            {
                if (_canvasGroup != null)
                    _canvasGroup.alpha = targetAlpha;
                return;
            }

            if (_canvasGroup != null)
            {
                _fadeTween = _canvasGroup.DOFade(targetAlpha, _fadeDuration)
                    .SetEase(visible ? Ease.OutQuad : Ease.InQuad)
                    .SetLink(gameObject);
            }
        }

        private void OnValidate()
        {
            if (_button == null)
                _button = GetComponent<Button>();
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            if (_button != null)
                _button.onClick.AddListener(HandleClick);
        }

        private void OnDisable()
        {
            _fadeTween?.Kill();
            _fadeTween = null;
            if (_button != null)
                _button.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick() => Clicked?.Invoke();
    }
}
