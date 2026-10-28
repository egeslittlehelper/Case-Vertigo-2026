using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoCase.UI
{
    /// <summary>
    /// Exit/cash-out button — visible only in safe/super zones while idle.
    /// </summary>
    public sealed class ExitButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Animation")]
        [SerializeField] private float _fadeDuration = 0.25f;

        private Tween _fadeTween;
        private bool _visible;

        public event Action Clicked;

        public void SetVisible(bool visible, bool animate)
        {
            if (_visible == visible) return;
            _visible = visible;

            _fadeTween?.Kill();

            if (!animate)
            {
                if (_canvasGroup != null)
                {
                    _canvasGroup.alpha = visible ? 1f : 0f;
                    _canvasGroup.blocksRaycasts = visible;
                    _canvasGroup.interactable = visible;
                }
                gameObject.SetActive(visible);
                return;
            }

            if (visible)
            {
                gameObject.SetActive(true);
                if (_canvasGroup != null)
                {
                    _canvasGroup.alpha = 0f;
                    _canvasGroup.blocksRaycasts = true;
                    _canvasGroup.interactable = true;
                    _fadeTween = _canvasGroup.DOFade(1f, _fadeDuration)
                        .SetEase(Ease.OutQuad)
                        .SetLink(gameObject);
                }
            }
            else
            {
                if (_canvasGroup != null)
                {
                    _canvasGroup.interactable = false;
                    _fadeTween = _canvasGroup.DOFade(0f, _fadeDuration)
                        .SetEase(Ease.InQuad)
                        .OnComplete(() =>
                        {
                            _canvasGroup.blocksRaycasts = false;
                            gameObject.SetActive(false);
                        })
                        .SetLink(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
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
