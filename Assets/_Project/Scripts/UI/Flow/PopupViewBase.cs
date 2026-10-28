using System;
using DG.Tweening;
using UnityEngine;

namespace VertigoCase.UI
{
    /// <summary>
    /// Shared show/hide tween logic for popups — fades CanvasGroup and scales the anim child.
    /// </summary>
    public abstract class PopupViewBase : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Transform _animTarget;

        [Header("Animation")]
        [SerializeField] private float _fadeDuration = 0.2f;
        [SerializeField] private float _scaleDuration = 0.3f;

        private Sequence _showSequence;
        private Sequence _hideSequence;

        public bool IsOpen { get; private set; }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            IsOpen = true;

            _hideSequence?.Kill();

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.blocksRaycasts = true;
            }

            if (_animTarget != null)
                _animTarget.localScale = Vector3.zero;

            _showSequence?.Kill();
            _showSequence = DOTween.Sequence();

            if (_canvasGroup != null)
                _showSequence.Append(_canvasGroup.DOFade(1f, _fadeDuration).SetEase(Ease.OutQuad));

            if (_animTarget != null)
            {
                _showSequence.Join(
                    _animTarget.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack));
            }

            _showSequence.SetLink(gameObject);
        }

        public virtual void Hide(Action onHidden = null)
        {
            _showSequence?.Kill();
            _hideSequence?.Kill();
            _hideSequence = DOTween.Sequence();

            if (_animTarget != null)
                _hideSequence.Append(_animTarget.DOScale(0f, _scaleDuration * 0.6f).SetEase(Ease.InBack));

            if (_canvasGroup != null)
                _hideSequence.Join(_canvasGroup.DOFade(0f, _fadeDuration).SetEase(Ease.InQuad));

            _hideSequence.OnComplete(() =>
            {
                IsOpen = false;
                if (_canvasGroup != null)
                    _canvasGroup.blocksRaycasts = false;
                gameObject.SetActive(false);
                onHidden?.Invoke();
            });

            _hideSequence.SetLink(gameObject);
        }

        protected virtual void OnValidate()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            if (_animTarget == null)
                _animTarget = ChildReference.FindChild<Transform>(transform, GetAnimTargetName());
        }

        protected abstract string GetAnimTargetName();

        protected virtual void OnDisable()
        {
            _showSequence?.Kill();
            _showSequence = null;
            _hideSequence?.Kill();
            _hideSequence = null;
        }
    }
}
